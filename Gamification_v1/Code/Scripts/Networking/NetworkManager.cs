﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using SimpleJSON;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.IO;
using System.Net;

public class NetworkManager : MonoBehaviour
{
    // Public variables

    // Daily limits
    //public int AchievementRewardCount = 0;          //Number of achievement rewards collected
    //public const int AchievementRewardLimit = 3;    //Number allowed per day
    //public int CraftsAvailable = 0;                 //Number of crafts available

    // Town board
    //public double TB_Progress;              //Progress for progress bar, in range [0, 1]
    //public string TB_ProgressTitle;         //Progress bar title
    //public string TB_ProgressDesc;          //Progress bar desc
    //public List<string>[] TB_Pages;         //First string is title, all others are for text

    // Content lock
    public int ContentLock;

    //Private variables

    // Version info
    private static string _clientVersion = "2.2"; //Client version, hard coded for each release
    private string _serverVersion;                  //Will be transmitted upon server connection

    // Networking
    private static int BLKSIZE = 4096;              //There's no need to touch this value. Does not strictly need to be same as server side
    private static Int32 _tcpPort = 60001;          //TCP port for game communication
    //private static Int32 _socketPolicyPort = 60002; //TCP port for server socket policy, (silly) requirement of web player
    private static string _serverAddress = "ccain.eecs.wsu.edu";                //Self explanatory
    private static IPAddress _serverIP = IPAddress.Parse("69.166.48.217");      //Currently hard coded for efficiency
    //private TcpClient _tcpClient = null;            //Used for all TCP communications
	private ClientWebSocket _webClient = null;
    private string _loginKey = "";                  /* Obtained during login process, used by server to verify login session
                                                     * Each login session gets a unique key to append to each message. On the server,
                                                     * if the login key given does not match the current login, that means you are in
                                                     * and old session, so you are disconnected and notified of such. */

    // Multiplayer
    /*/private UdpManager _udp;                //Used for all UDP communications
    private static Int32 _udpPort = 60001;  //UDP port for multiplayer communication
    private string _udpKey = "";            //Similar to login key, minus duplicate checking (it would be redundant to do it on both)
    private PlayerRoster _roster;           //Player roster, check the PlayerRoster class for more info/*/

    // Encryption
    private byte[] _key = new byte[32];         //Generated in Handshake()
    private byte[] _authKey = new byte[32];     //Retrieved from the server in Handshake()
    private byte[] _iv = new byte[32];          //Not constant, but changed in a thread safe way. 256 block size / 8 = 32 bytes
    private byte[] _auth = new byte[32];        //Same as above, except size is determined by the auth algorithm, in this case SHA256, hence 32 bytes
    private Encoding _encoder = Encoding.ASCII; //All text is ASCII and therefore 7-bit, making parsing much easier.
                                                //No demonstrable need to change this to UTF-8 or another character set

    // Local data
    private string _username;                   //All of these should be self explanatory
    //private JSONArray _bonusCodes = null;       //JSON Arrays
    /*/
    private JSONArray _equipment = null;
    private JSONArray _inventory = null;
    private JSONArray _quests = null;
    private JSONArray _skills = null;
    private JSONNode _playerStats = null;       //JSON Nodes
    private JSONNode _achievements = null;
    private JSONNode _achievementStats = null;
    private JSONNode _achievementMilestones = null;
    private JSONNode _recipes = null;//*/

    private JSONNode _playersSeeker;
    private JSONNode _playersConqueror;
    private JSONNode _playersMastermind;
    private JSONNode _playersIncremental;
    private JSONArray _seekerInventory;
    private JSONArray _seekerStorage;
    private JSONNode _seekerQuestLog;
    private JSONNode _seekerStats;
    //private JSONNode _seekerStatsXP;
    private int _seekerStory;
    private int _seekerPopulation;
    private JSONNode _conquerorStats;
    private JSONNode _conquerorUnlocked;
    private JSONArray _conquerorRecipes;
    private JSONArray _conquerorInventory;
    private JSONArray _conquerorUpgrades;
    private JSONNode _conquerorEquipped;
    private JSONNode _incQuestLog;
    private JSONNode _incAchievements;
    private JSONNode _incUpgrades;
    

    // Timing and message sending
    private float _updateTick = 5;              //How frequently to send TCP messages, currently every 5 seconds
    private float _timer = 0;                   //Simple timer variable
    private List<List<string>> _messagesToSend = new List<List<string>>();  //All messages are queued then sent as a chunk
    private static object _lock = new object();     //Lock object, used by UpdateDB()
    private float _timeSinceLastMessageReceipt = 0; //Used to check for connection drops or timeouts

    // Miscellaneous
    public bool RunWithoutServer = false;       //Testing variable. Will run if set to true, but a lot of stuff will be broken
    //public string EnteredBonusCode = null;      //Stores bonus code entered in login screen
    private int _status = 0;                    //Status code of last received TCP message. Redundant, but still in use
    private bool _loggedInElsewhere = false;    //Logged in elsewhere. If true, spams user with message urging them to close the window
    private bool _timedOut = false;             //Connection dropped or timed out. Also spams user, this time to reload their webpage
    private bool _awaitingPing = false;         //If no TCP activity in past 15 seconds, send one ping to the server.


    /********************************************************************************
    ------------------------------- UNITY SECTION -----------------------------------
    *********************************************************************************
    -This section contains all functions used by the Unity Engine
    ********************************************************************************/

    //Start
    private void Start()
    {
        //Get socket policy
        //Security.PrefetchSocketPolicy(_serverIP.ToString(), _socketPolicyPort);

        //Show version number on client
        GameObject.Find("Version Number").GetComponent<Text>().text = _clientVersion;

        //Create WebSocket
        try
        {
            //_tcpClient = new TcpClient(_serverAddress, _tcpPort);
			webClient = new ClientWebSocket();
			
            GameObject.Find("Error Message").GetComponent<Text>().text = "Connecting to server...";
            Handshake();
            GameObject.Find("Error Message").GetComponent<Text>().text = "";

            if (_serverVersion != _clientVersion)
            {
                GameObject.Find("Error Message").GetComponent<Text>().text = "Error: client is out of date.";
            }
        }
        //Could not connect to database
        catch (Exception e)
        {
            GameObject.Find("Error Message").GetComponent<Text>().text = "Error: unable to connect to database";
            Debug.Log(e);
        }

        //Make DatabaseManager persist across scenes
        DontDestroyOnLoad(this);
    }

    //Database will check for changes every 5 seconds. If any are made, an asynchronous update is made
    private void Update()
    {
        _timer += Time.deltaTime;
        _timeSinceLastMessageReceipt += Time.deltaTime;
        //Debug.Log("Timer: " + _timer + ", time since last message: " + _timeSinceLastMessageReceipt);
        if (_timer > _updateTick)
        {
            //First, if connection is no longer active (due to being killed), repeatedly annoy the player about it
            if (_webClient == null && (_loggedInElsewhere || _timedOut)) {
                //Notify player (every 5 seconds)
                var moc = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
                moc.EnqueueMessage("ERROR: " + (_loggedInElsewhere ? "logged in elsewhere" : "connection timed out"));
                moc.EnqueueMessage("No longer saving progress!");
                moc.EnqueueMessage("To reconnect, restart your client");
                moc.EnqueueMessage(" ");
                _timer = _updateTick - 5;
                return;
            }
            //If player is logged in elsewhere or the connection has timed out, kill the connection
            if (_loggedInElsewhere || _timedOut)
            {
                //Kill connection (if not already dead)
                //if (_tcpClient != null) _tcpClient.Close();
				if (_webClient != null)
				{
					//Task res = 
					_webClient.CloseAsync(WebSocketCloseStatus.Empty,"Player is logged in elsewhere or connection has timed out",CancellationToken.None);
				}					
                //if (_udp != null) _udp.Close();
                //_tcpClient = null;
				_webClient = null;
                return;
            }
            //If client is null, try to connect
            if (_webClient == null && !RunWithoutServer)
            {
                try
                {
                    _webClient = new ClientWebSocket();//TcpClient(_serverAddress, _tcpPort);
                    GameObject.Find("Error Message").GetComponent<Text>().text = "Connecting to server...";
                    Handshake();
                    GameObject.Find("Error Message").GetComponent<Text>().text = "";

                    if (_serverVersion != _clientVersion)
                    {
                        GameObject.Find("Error Message").GetComponent<Text>().text = "Error: client is out of date.";
                    }
                }
                //Count not connect to database
                catch (Exception e)
                {
                    //Do nothing
                    Debug.Log(e);
                }
                finally
                {
                    _timer = 0;
                }
                return;
            }
            //If 15 seconds have passed without any communication, ping the server
            if (_timeSinceLastMessageReceipt >= 15 && !_awaitingPing)
            {
                QueueMessage(new List<string>() { "PING" });
                _awaitingPing = true;
            }
            //If 30 seconds have passed, the connection has died
            if (_timeSinceLastMessageReceipt >= 30)
            {
                Debug.Log("Timed out! :(");
                _timedOut = true;
                return;
            }
            //Send any messages if any are queued
            if (_messagesToSend.Count > 0)
            {
                //Make copy of list of messages
                List<List<string>> messages = new List<List<string>>(_messagesToSend);
                //Create thread to asynchronously update
                Thread updateThread = new Thread(() => UpdateDB(messages));
                updateThread.Start();
                //Clear the message queue
                _messagesToSend.Clear();
            }
            _timer = 0;
        }
    }

    //Close the TCP and UDP clients when the game exits
    private void OnApplicationQuit()
    {
        //Send closing message then close connection if it hasn't been killed already
        if (_webClient != null)
        {
            List<string> killCommand = new List<string>() { "QUIT" };
            _messagesToSend.Add(killCommand);
            SendMessage(_messagesToSend);
            //_tcpClientlient.Close();
			_webClient.CloseAsync(WebSocketCloseStatus.Empty,"Client application quit.",CancellationToken.None);
        }
        //if (_udp != null) _udp.Close();
    }




    /********************************************************************************
    ------------------------ NETWORK AND ENCRYPTION SECTION -------------------------
    *********************************************************************************
    -This section contains all networking functions, including encryption/decryption
    ********************************************************************************/

    //Receive message in byte format, then decrypt it and translate it into a string
    //Format: int32 size, int32 status, byte[16] auth, byte[16] IV, byte[size] data
    private string ReceiveMessage()
    {
        //Exit if the client has not been initialized
        if (_webClient == null || RunWithoutServer) return null;

        //Do not crash the client if a faulty message is received
        try {
            //Initialize variables
            byte[] buffer = new byte[BLKSIZE];
            int size = 1;
            _status = 1;
            int startIndex = 0;
            int bytesRead = 0;
            int bytesToRead = 0;
            bool firstBlockRead = false;
            int readToBuffer = 0;
            MemoryStream byteBuffer = new MemoryStream();
            ArraySegment<byte> bufferSeg = new ArraySegment<byte>();
            Task<WebSocketReceiveResult> received_info = _webClient.ReceiveAsync(bufferSeg, CancellationToken.None);

            //Read size and status code, then read the remaining data into the byte buffer
            while (((readToBuffer = received_info.Result.Count) != 0) && bytesRead < size)
            {
                Array.Copy(bufferSeg.Array, buffer);
                startIndex = 0;
                //Read header
                if (!firstBlockRead)
                {
                    //Size comes first
                    byte[] intBuf = new byte[4];
                    Array.Copy(buffer, 0, intBuf, 0, 4);
                    if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                    size = BitConverter.ToInt32(intBuf, 0);
                    //Then read the status
                    Array.Clear(intBuf, 0, 4);
                    Array.Copy(buffer, 4, intBuf, 0, 4);
                    if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                    _status = BitConverter.ToInt32(intBuf, 0);
                    //Authentication code
                    Array.Copy(buffer, 8, _auth, 0, 32);
                    //Get the IV
                    Array.Copy(buffer, 40, _iv, 0, 32);
                    //Update values
                    startIndex = 72;
                    firstBlockRead = true;
                    //Debug.Log("Receiving " + size + " bytes");
                }
                //Calculate how many bytes are remaining then read them
                bytesToRead = (size - bytesRead + startIndex < BLKSIZE ? size - bytesRead : BLKSIZE - startIndex);
                bytesToRead = (bytesToRead > (readToBuffer - startIndex) ? (readToBuffer - startIndex) : bytesToRead);
                byteBuffer.Write(buffer, startIndex, bytesToRead);
                bytesRead += bytesToRead;
                if (bytesRead >= size) break;
                Array.Clear(_buffer, 0, BLKSIZE); //line wasn't in Alex's version of file
                received_info = _webClient.ReceiveAsync(bufferSeg, CancellationToken.None);

            }

            //Convert encrypted blob into string
            byte[] encrypted = byteBuffer.ToArray();

            //Verify the integrity of this message
            bool isValid = VerifyMessage(encrypted, size, _status);

            //If not valid, throw an exception
            if (!isValid)
            {
                _status = -7;
                throw new Exception("Data failed verification");
            }

            //Valid message received
            _timeSinceLastMessageReceipt = 0;
            _awaitingPing = false;

            //Enable UDP if ready
            //if (_status == 5) _udp.ServerUdpReady = true;

            //Decrypt it and convert it into a string
            return _encoder.GetString(Decrypt(encrypted));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            if (_status != -7) _status = -5;    //Flag as regular error
        }
        //Return an error message if a problem was encountered
        return "ERROR: INVALID MESSAGE";
    }

    //Used by Handshake() to receive plaintext
    private byte[] ReceiveMessageHandshake(ref int keySize, ref int authSize, /*ref int uniqueIdSize,*/ ref int playerIdSize, ref int versionSize)
    {
        //Initialize variables
        byte[] buffer = new byte[BLKSIZE];
        int size = 1;
        int startIndex = 0;
        int bytesRead = 0;
        int bytesToRead = 0;
        bool firstBlockRead = false;
        MemoryStream byteBuffer = new MemoryStream();
        ArraySegment<byte> bufferSeg = new ArraySegment<byte>();
        Task<WebSocketReceiveResult> received_info = _webClient.ReceiveAsync(bufferSeg, CancellationToken.None);

        //Read size and status code, then read the remaining data into the byte buffer
        while ((/*(_tcpClient.GetStream().Read(buffer, 0, buffer.Length)*/ received_info.Result.Count != 0) && bytesRead < size)
        {
            Array.Copy(bufferSeg.Array, buffer);
            startIndex = 0;
            //Read header
            if (!firstBlockRead)
            {
                //Encryption key size
                byte[] intBuf = new byte[4];
                Array.Copy(buffer, 0, intBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                keySize = BitConverter.ToInt32(intBuf, 0);
                //Authentication key size
                Array.Copy(buffer, 4, intBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                authSize = BitConverter.ToInt32(intBuf, 0);
                //Unique UDP ID size
                //Array.Copy(buffer, 8, intBuf, 0, 4);
                //if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                //uniqueIdSize = BitConverter.ToInt32(intBuf, 0);
                //Player ID size
                Array.Copy(buffer, 12, intBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                playerIdSize = BitConverter.ToInt32(intBuf, 0);
                //Server version size
                Array.Copy(buffer, 16, intBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
                versionSize = BitConverter.ToInt32(intBuf, 0);
                //Total size
                size = keySize + authSize /*+ uniqueIdSize*/ + playerIdSize + versionSize;
                //Update values
                startIndex = 20;
                firstBlockRead = true;
                //Debug.Log("Receiving " + size + " bytes");
            }
            //Calculate how many bytes are remaining then read them
            bytesToRead = (size - bytesRead + startIndex < BLKSIZE ? size - bytesRead : BLKSIZE - startIndex);
            byteBuffer.Write(buffer, startIndex, bytesToRead);
            bytesRead += bytesToRead;
            if (bytesRead >= size) break;
            received_info = _webClient.ReceiveAsync(bufferSeg, CancellationToken.None);
        }

        //Debug.Log("Read " + bytesRead + " bytes");
        return byteBuffer.ToArray();
    }

    /* Converts list of messages into binary, then sends it off to the server in BLKSIZE chunks
     * Message format is in the following form:
     * (0x01)CMD(0x02)var1(0x02)var2...(0x02)varX(0x01)KEY(0x03)         single command
     * (0x01)CMD(0x02)var1...(0x02)varX(0x01)CMD(0x02)var1...(0x01)KEY(0x03)  multiple commands
     * 0x01 separates command blocks (signals start of next command and end of previous)
     * 0x02 separates variables within command blocks
     * 0x03 signals EOF
     * All commands require a valid key, with the exception of the LOGIN command
     * Entire transmitted data is in the following format:
     * int32 size, byte[16] auth, byte[16] iv, byte[size] data
     * This message format is why all text is sent as ASCII - no risk of using 0x01, 0x02, or 0x03
     */
    private void SendMessage(List<List<string>> messages)
    {
        //Exit if the client has not been initialized
        if (_webClient == null || RunWithoutServer) return;

        //Create buffer - although I could calculate the size beforehand, I don't really care to
        List<byte> buffer = new List<byte>();
        //Add all the messages to the buffer, including delimiters
        foreach (var message in messages)
        {
            for (int i = 0; i < message.Count; i++)
            {
                if (i == 0) buffer.Add((byte)1);
                else buffer.Add((byte)2);
                if (string.IsNullOrEmpty(message[i]))
                {
                    //Empty string, because I'm lazy
                    message[i] = "~><~>~<~><~";
                }
                buffer.AddRange(_encoder.GetBytes(message[i]));
            }
        }
        //Add the key
        buffer.Add((byte)1);
        buffer.AddRange(_encoder.GetBytes(_loginKey));
        buffer.Add((byte)3);

        //Encrypt this buffer (returns the message itself and the IV)
        byte[] encryptedMessage = Encrypt(buffer.ToArray());

        //Hash the encrypted text and IV
        _auth = SignMessage(encryptedMessage, encryptedMessage.Length);
        //Debug.Log("Length: " + _auth.Length);

        //Create the header
        List<byte> header = new List<byte>();
        //Message size
        byte[] intBuf = BitConverter.GetBytes(encryptedMessage.Length);
        if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
        header.AddRange(intBuf);
        //HMAC, used to verify integrity of message
        header.AddRange(_auth);
        //Initialization vector, used for decryption
        header.AddRange(_iv);

        //Now send this to the server
        SendMessage(header.ToArray(), encryptedMessage);
    }

    //Sends the actual byte arrays to the server
    private void SendMessage(byte[] header, byte[] data)
    {
        byte[] buffer = new byte[BLKSIZE];
        int startIndex = 0;
        int s = 0;
        bool headerSent = false;
        int headerSize = header.Length;

        while (startIndex < data.Length)
        {
            s = 0;
            //Calculate number of bytes to send in this chunk
            int toSend = data.Length - startIndex;
            if (!headerSent && toSend > BLKSIZE - headerSize) toSend = BLKSIZE - headerSize;
            if (toSend > BLKSIZE) toSend = BLKSIZE;
            //If header hasn't been sent, buffer it
            if (!headerSent)
            {
                Array.Copy(header, 0, buffer, 0, headerSize);
                Array.Copy(data, 0, buffer, headerSize, toSend);
                s = headerSize;
                headerSent = true;
                //Debug.Log("Sending " + data.Length + " bytes");
            }
            //Else just buffer the next block
            else
            {
                Array.Copy(data, startIndex, buffer, 0, toSend);
            }
            //Write buffer to stream
            //_tcpClient.GetStream().Write(buffer, 0, toSend + s);
			_webClient.SendAsync(new ArraySegment<byte>(buffer,0, toSend + s), CancellationToken.None);
            //Increment counter and clear the buffer
            startIndex += toSend;
            Array.Clear(buffer, 0, BLKSIZE);
        }
    }

    //Run when the clients first connect. The client generates an RSA key pair and sends it to the server, then the server sends a new key and auth key
    private void Handshake()
    {
        //Generate a public-private key pair
        var csp = new RSACryptoServiceProvider(1024);
        string publicKey = csp.ToXmlString(false);
        //Get public key as byte array
        byte[] publicKeyBytes = _encoder.GetBytes(publicKey);

        //First send the public key (as plaintext) directly to the server
        //Header is four bytes: int32 size
        byte[] header = BitConverter.GetBytes(publicKeyBytes.Length);
        if (BitConverter.IsLittleEndian) Array.Reverse(header);
        SendMessage(header, publicKeyBytes);

        //Receive the new keys
        int keySize = 0;
        int authSize = 0;
        //int udpIdSize = 0;
        int playerIdSize = 0;
        int versionSize = 0;

        byte[] received = ReceiveMessageHandshake(ref keySize, ref authSize, /*/ref udpIdSize,*/ ref playerIdSize, ref versionSize);
        byte[] keyBuf = new byte[keySize];
        byte[] authBuf = new byte[authSize];
        //byte[] udpIdBuf = new byte[udpIdSize];
        byte[] playerIdBuf = new byte[playerIdSize];
        byte[] versionBuf = new byte[versionSize];

        Array.Copy(received, 0, keyBuf, 0, keySize);
        _key = csp.Decrypt(keyBuf, false);
        Array.Copy(received, keySize, authBuf, 0, authSize);
        _authKey = csp.Decrypt(authBuf, false);
        //Array.Copy(received, keySize + authSize, udpIdBuf, 0, udpIdSize);
        //_udpKey = _encoder.GetString(csp.Decrypt(udpIdBuf, false));
        Array.Copy(received, keySize + authSize /*+ udpIdSize*/, playerIdBuf, 0, playerIdSize);
        byte[] idBuf = csp.Decrypt(playerIdBuf, false);
        if (BitConverter.IsLittleEndian) Array.Reverse(idBuf);
        int playerId = BitConverter.ToInt32(idBuf, 0);
        Array.Copy(received, keySize + authSize /*+ udpIdSize*/ + playerIdSize, versionBuf, 0, versionSize);
        _serverVersion = _encoder.GetString(csp.Decrypt(versionBuf, false));

        //Get roster
        //_roster = GetComponentInParent<PlayerRoster>();
        //_roster.SetPlayerId(playerId);

        //Now that we have a UDP key, start the UDP manager
        //_udp = new UdpManager(_serverIP, _udpPort, _udpKey, _roster);

    }

    //Crypto - used to transform a byte array with a Crypto Stream, used by Decrypt() and Encrypt()
    //Code courtesy of http://stackoverflow.com/a/24903689
    private byte[] Crypto(ICryptoTransform cryptoTransform, byte[] data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }
    }

    //Decrypts a given byte array, assumes that the correct _iv has already been loaded before calling this function
    private byte[] Decrypt(byte[] data)
    {
        using (var rm = new RijndaelManaged())
        {
            rm.BlockSize = 256;
            var decryptor = rm.CreateDecryptor(_key, _iv);
            return Crypto(decryptor, data);
        }
    }

    //Encrypts a given byte array
    private byte[] Encrypt(byte[] data)
    {
        using (var rm = new RijndaelManaged())
        {
            rm.BlockSize = 256;
            rm.GenerateIV();
            _iv = rm.IV;
            var encryptor = rm.CreateEncryptor(_key, _iv);
            return Crypto(encryptor, data);
        }
    }

    //Signs a file with _authKey. Follows encrypt-then-MAC
    private byte[] SignMessage(byte[] data, Int32 size)
    {
        byte[] intBuf = BitConverter.GetBytes(size);
        if (BitConverter.IsLittleEndian) Array.Reverse(intBuf);
        using (HMACSHA256 hmac = new HMACSHA256(_authKey))
        {
            byte[] buffer = new byte[intBuf.Length + _iv.Length + data.Length];
            Array.Copy(intBuf, 0, buffer, 0, intBuf.Length);
            Array.Copy(_iv, 0, buffer, intBuf.Length, _iv.Length);
            Array.Copy(data, 0, buffer, intBuf.Length + _iv.Length, data.Length);
            return hmac.ComputeHash(buffer);
        }
    }

    //Verifies a file, returns true or false. "False" means the message should be ignored, because it has been tampered with
    private bool VerifyMessage(byte[] data, Int32 size, Int32 status)
    {
        bool IsValid = true;
        byte[] intBuf1 = BitConverter.GetBytes(size);
        if (BitConverter.IsLittleEndian) Array.Reverse(intBuf1);
        byte[] intBuf2 = BitConverter.GetBytes(status);
        if (BitConverter.IsLittleEndian) Array.Reverse(intBuf2);
        using (HMACSHA256 hmac = new HMACSHA256(_authKey))
        {
            byte[] buffer = new byte[intBuf1.Length + intBuf2.Length + _iv.Length + data.Length];
            Array.Copy(intBuf1, 0, buffer, 0, intBuf1.Length);
            Array.Copy(intBuf2, 0, buffer, intBuf1.Length, intBuf2.Length);
            Array.Copy(_iv, 0, buffer, intBuf1.Length + intBuf2.Length, _iv.Length);
            Array.Copy(data, 0, buffer, intBuf1.Length + intBuf2.Length + _iv.Length, data.Length);
            byte[] computedHash = hmac.ComputeHash(buffer);
            //Check all values even if an error is found. This prevents timing attacks
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (_auth[i] != computedHash[i])
                {
                    IsValid = false;
                }
            }
            return IsValid;
        }
    }

    //Queue message
    private void QueueMessage(List<string> message)
    {
        _messagesToSend.Add(message);
    }

    //Send messages to database and receive results
    //This is technically threaded, but it's meant to be asynchronous, not multithreaded
    //Because of this, if the connection drops and multiple messages are queued, we want them processed in order
    private void UpdateDB(List<List<string>> messages)
    {
        lock (_lock)
        {
            SendMessage(messages);
            ReceiveMessage();
            switch (_status)
            {
                case 5:     //UDP is ready to go
                    _udp.ServerUdpReady = true;
                    break;
                case -7:    //Faulty message received, could be evidence of tampering with network
                    break;
                case -8:    //Player is logged in elsewhere, so kill the current client session
                    Debug.Log("Logged in elsewhere!");
                    _loggedInElsewhere = true;
                    _timer = _updateTick;   //Immediately notify player they have disconnected
                    break;
            }
            _timeSinceLastMessageReceipt = 0;
            _awaitingPing = false;
        }
    }



    /********************************************************************************
    ------------------------------- LOGIN SECTION -----------------------------------
    *********************************************************************************
    -This section is responsible for handling the initialization of the DatabaseManager
    object, and player logins.
    ********************************************************************************/


    //Verifies a login. Returns an int depending on what occurred. If verified, it will load the player inventory, quests, stats, and playerstats
    //  0   success
    // -1   username not found
    // -2   password does not match
    public int PlayerLogin(string username, string password)//, string bonusCode)
    {
        //Exit if the client has not been initialized
        if (_webClient == null) return -4;

        //Exit if the version numbers don't match
        if (_serverVersion != _clientVersion)
        {
            GameObject.Find("Error Message").GetComponent<Text>().text = "Error: client is out of date.";
            return -4;
        }

        //Notify the player that they are currently logging in
        var text = GameObject.Find("Error Message").GetComponent<Text>();
        text.text = "Logging in...";
        text.color = Color.white;

        //Send login request
        //SendMessage(new List<List<string>>() { new List<string>() { "LOGIN", username, password, _udp.GetPublicIP(), _udp.recvPort.ToString(), _udp.sendPort.ToString() } });
        SendMessage(new List<List<string>>() { new List<string>() { "LOGIN", username, password } });

        //Get result. If status code != 0, exit now
        string response = ReceiveMessage();

        if (_status != 0) return _status;


        //If login was successful, server responded with the login key, so save it
        _loginKey = response;
        _username = username;

        //Request all login data and store the results
        SendMessage(new List<List<string>>() { new List<string>() { "LOGIN_GETDATA" } });

        JSONNode loginData = JSON.Parse(ReceiveMessage());
        //_achievements = loginData["Achievements"];
        //_achievementMilestones = loginData["AchievementMilestones"];
        //_achievementStats = loginData["AchievementStats"];
        //AchievementRewardCount = loginData["AchievementRewardsCollectedToday"].AsInt;
        //_bonusCodes = loginData["BonusCodes"].AsArray;

        _playersSeeker = loginData["Seeker"];
        _playersConqueror = loginData["Conqueror"];
        _playersMastermind = loginData["Mastermind"];
        _playersIncremental = loginData["Incremental"];
        _seekerInventory = _playersSeeker["Inventory"].AsArray;
        _seekerStorage = _playersSeeker["Storage"].AsArray;
        _seekerQuestLog = _playersSeeker["QuestLog"];
        _seekerStats = _playersSeeker["Stats"];
        //_seekerStatsXP = _playersSeeker["StatsXP"];
        _conquerorStats = _playersConqueror["Stats"];
        _conquerorUnlocked = _playersConqueror["UnlockedWeapons"];
        _conquerorRecipes = _playersConqueror["Recipes"];
        _conquerorInventory = _playersConqueror["Inventory"].AsArray;
        _conquerorUpgrades = _playersConqueror["Upgrades"].AsArray;
        _conquerorEquipped = _playersConqueror["Equipped"];
        //_incQuestLog = ;
        //_incAchievements;
        _incUpgrades = _playersIncremental["Upgrades"];

        /*/
        _equipment = loginData["Equipment"].AsArray;
        _inventory = loginData["Inventory"].AsArray;
        _playerStats = loginData["PlayerStats"];
        _quests = loginData["Quests"].AsArray;
        _skills = loginData["Skills"].AsArray;
        _recipes = loginData["Recipes"];//*/



        //CraftsAvailable = _playerStats["CraftsAvailable"].AsInt;

        //EnteredBonusCode = bonusCode;

        ContentLock = loginData["ContentLock"].AsInt;

        //Process Town Board
        //TB_ParseProgressBar(loginData["ProgressBar"]);
        //TB_ParsePageText(loginData["TownBoardPages"].AsArray);

        return 0;
    }


    /********************************************************************************
    --------------------------- ACHIEVEMENTS SECTION --------------------------------
    *********************************************************************************
    - Achievements and milestones are tracked here
    ********************************************************************************/

    /*/
    public int GetAchievementStatus(int id)
    {
        if (_achievements[id.ToString()] == null)
        {
            _achievements[id.ToString()].Value = "0";
            UpdateAchievement(id, 0);
        }
        return _achievements[id.ToString()].AsInt;
    }

    public int GetMilestoneStatus(int id)
    {
        if (_achievementMilestones[id.ToString()] == null)
        {
            _achievementMilestones[id.ToString()].Value = "0";
            UpdateMilestone(id, 0);
        }
        return _achievementMilestones[id.ToString()].AsInt;
    }

    public int GetAchievementStatStatus(int id)
    {
        if (_achievementStats[id.ToString()] == null)
        {
            _achievementStats[id.ToString()].Value = "0";
            UpdateAchievementStat(id, 0);
        }
        return _achievementStats[id.ToString()].AsInt;
    }

    //Updates an achievement
    public void UpdateAchievement(int id, int state)
    {
        QueueMessage(new List<string>() { "ACHIEVEMENT_UPDATE", id.ToString(), state.ToString() });
    }

    //Updates a milestone
    public void UpdateMilestone(int id, int state)
    {
        QueueMessage(new List<string>() { "ACHIEVEMENT_MILESTONE_UPDATE", id.ToString(), state.ToString() });
    }

    //Updates achievement stat
    public void UpdateAchievementStat(int id, int value)
    {
        QueueMessage(new List<string>() { "ACHIEVEMENT_STAT_UPDATE", id.ToString(), value.ToString() });
        //Debug.Log("Updating achievement stat, id: " + id + ", value: " + value);
    }
    //*/
     
    /********************************************************************************
    ---------------------------- BONUS CODES SECTION --------------------------------
    *********************************************************************************
    -This section will be expanded upon in the future
    ********************************************************************************/

    //Gets bonus from code as List<string>, which will be parsed by BonusManager
	/*/
    public List<string> BonusGetBonus(string code)
    {
        foreach (JSONClass bonus in _bonusCodes)
        {
            if (bonus["Code"].Value.ToLower() == code.ToLower())
            {
                //"Bonus" is an array of strings, make it into a List<string>
                List<string> reward = new List<string>();
                foreach (JSONNode str in bonus["Bonus"].AsArray)
                {
                    reward.Add(str.Value);
                }
                return reward;
            }
        }
        return null;
    }

    //Marks code as activated
    public void BonusCodeActivate(string code)
    {
        QueueMessage(new List<string>() { "BONUSCODE_ACTIVATE", code });
        //Add 8 crafting points as well
        CraftsAvailable += 8;
        QueueMessage(new List<string>() { "PLAYERSTAT_UPDATE", "CraftsAvailable", CraftsAvailable.ToString() });
    }
	//*/

    /********************************************************************************
    ---------------------------- EQUIPMENT MANIPULATION -----------------------------
    *********************************************************************************
    -This section is responsible for handling inventory related database interaction.
    ********************************************************************************/

    //retreives player equipment for conqueror game
    public void RetrieveConquerorEquipment(PlayerController player)
    {
        if (_webClient == null) return;
    }


    //Retrieves player equipment, pretty straightforward
    public void RetrievePlayerEquipment(PlayerController player)
    {
        //If client is null exit now
        if (_webClient == null) return;

        Item item;
        Enchantment enchantment;

        foreach (JSONClass e in _equipment)
        {
            //Load the inventory item
            item = Instantiate(Resources.Load(Item.Path[e["ID"].AsInt]) as GameObject).GetComponent<Item>();
            item.transform.parent = transform;
            item.gameObject.SetActive(false);
            DontDestroyOnLoad(item.gameObject);
            //Set all the details
            item.Slot = e["Slot"].AsInt;
            item.Count = e["Count"].AsInt;
            item.ItemRarity = (Item.Rarity)e["ItemRarity"].AsInt;
            item.Quality = e["Quality"].AsInt;
            item.Creator = e["Creator"];
            foreach (JSONClass ench in e["Enchantments"].AsArray)
            {
                enchantment = Instantiate(Resources.Load(Enchantment.Path[ench["ID"].AsInt]) as GameObject).GetComponent<Enchantment>();
                enchantment.Quality = ench["Quality"].AsInt;
                enchantment.transform.parent = transform;
                enchantment.gameObject.SetActive(false);
                item.Enchantments.Add(enchantment);
            }
            player.EquipFromDatabase(item);
        }
    }

    //Adds equipment
    public void AddEquipment(Item item)
    {
        List<string> addEquipmentCommand = new List<string>();
        addEquipmentCommand.Add("EQUIPMENT_ADD");
        addEquipmentCommand.Add(item.Slot.ToString());
        addEquipmentCommand.Add(item.ID.ToString());
        addEquipmentCommand.Add(item.Count.ToString());
        addEquipmentCommand.Add(item.Creator);
        addEquipmentCommand.Add(((int)item.ItemRarity).ToString());
        addEquipmentCommand.Add(item.Quality.ToString());
        //Add enchantments, if any
        addEquipmentCommand.Add(item.Enchantments.Count.ToString());
        for (int i = 0; i < item.Enchantments.Count; i++)
        {
            addEquipmentCommand.Add(item.Enchantments[i].ID.ToString());
            addEquipmentCommand.Add(item.Enchantments[i].Quality.ToString());
        }
        QueueMessage(addEquipmentCommand);
    }

    //Removes equipment
    public void RemoveEquipment(int slotNumber)
    {
        QueueMessage(new List<string>() { "EQUIPMENT_REMOVE", slotNumber.ToString() });
    }




    /********************************************************************************
    ---------------------------- INVENTORY MANIPULATION -----------------------------
    *********************************************************************************
    -This section is responsible for handling inventory related database interaction.
    ********************************************************************************/

    //this function initializes the players inventory to reflect the inventory they have stored on the database
    //use this function only after the player object and inventory have been instantiated, pass in the
    //GameObject that represents the player.

    public void RetrieveSeekerInventory(int[] inventory)
    {
        if (_webClient == null) return;
        List<int> tmp = new List<int>();
        foreach(JSONNumber i in _seekerInventory)
        {
            tmp.Add((int)(i.AsDouble));
        }
        inventory = tmp.ToArray();
    }

    //this function initializes the players inventory to reflect the inventory they have stored on the database
    //use this function only after the player object and inventory have been instantiated, pass in the
    //GameObject that represents the player.
    /*
    public void RetrievePlayerInventory(Inventory inventory)
    {
        //If client is null exit now
        if (_tcpClient == null) return;

        Item item;
        Enchantment enchantment;

        foreach (JSONClass i in _inventory)
        {
            //Load the item
            item = Instantiate((Resources.Load(Item.Path[i["ID"].AsInt]) as GameObject)).GetComponent<Item>();
            item.transform.parent = transform;
            item.gameObject.SetActive(false);
            DontDestroyOnLoad(item.gameObject);
            //Set all the details
            item.Slot = i["Slot"].AsInt;
            item.Count = i["Count"].AsInt;
            item.ItemRarity = (Item.Rarity)i["ItemRarity"].AsInt;
            item.Quality = i["Quality"].AsInt;
            item.Creator = i["Creator"];
            foreach (JSONClass ench in i["Enchantments"].AsArray)
            {
                enchantment = Instantiate(Resources.Load(Enchantment.Path[ench["ID"].AsInt]) as GameObject).GetComponent<Enchantment>();
                enchantment.Quality = ench["Quality"].AsInt;
                enchantment.transform.parent = transform;
                enchantment.gameObject.SetActive(false);
                item.Enchantments.Add(enchantment);
            }
            inventory.AddItemToInventoryFromDB(item);
        }
    }//*/

    //adds seeker inventory item to db
    public void AddSeekerInventoryItem(string itemID)
    {
        //add item
        List<string> addItemCmd = new List<string>();
        addItemCmd.Add("SEEKER_INVENTORY_ADD");
        addItemCmd.Add(itemID);

        QueueMessage(addItemCmd);
    }

    //adds seeker storage item to db
    public void AddSeekerStorageItem(string itemID)
    {
        //add item
        List<string> addItemCmd = new List<string>();
        addItemCmd.Add("SEEKER_STORAGE_ADD");
        addItemCmd.Add(itemID);

        QueueMessage(addItemCmd);

    }

    //Adds inventory item
    /*public void AddInventoryItem(Item item)
    {
        //Add item
        List<string> addItemCommand = new List<string>();
        addItemCommand.Add("ITEM_ADD");
        //Basic info
        addItemCommand.Add(item.Slot.ToString());
        addItemCommand.Add(item.ID.ToString());
        addItemCommand.Add(item.Count.ToString());
        addItemCommand.Add(item.Creator);
        //The following only applies to gear
        if (item.ItemType == Item.Type.Gear)
        {
            addItemCommand.Add(((int)item.ItemRarity).ToString());
            addItemCommand.Add(item.Quality.ToString());
            //Add enchantments, if any
            addItemCommand.Add(item.Enchantments.Count.ToString());
            for (int i = 0; i < item.Enchantments.Count; i++)
            {
                addItemCommand.Add(item.Enchantments[i].ID.ToString());
                addItemCommand.Add(item.Enchantments[i].Quality.ToString());
            }
        }
        QueueMessage(addItemCommand);
    }*/

    //Removes item from inventory. If more are removed than are in inventory, all will be removed (so no negative inventory counts)
    /*
    public void RemoveInventoryItem(int slotNumber, int count)
    {
        QueueMessage(new List<string>() { "ITEM_REMOVE", slotNumber.ToString(), count.ToString() });
    }//*/

    //removes item from seeker inventory
    public void RemoveSeekerInventoryItem(string itemID)
    {
        QueueMessage(new List<string>() { "SEEKER_INVENTORY_REMOVE". itemID });
    }

    public void RemoveSeekerStorageItem(string itemID)
    {
        QueueMessage(new List<string>() { "SEEKER_STORAGE_REMOVE", itemID });
    }

    /****************************************************************************
    ----------------------- SEEKER STORY  MANIPULATION -----------------------
    *****************************************************************************
    -This section holds all the functions that interact with the stats of a player.
    ****************************************************************************/

    //updates the story progress in seeker game
    public void SeekerStoryUpdate(int progress)
    {
        QueueMessage(new List<string>() { "SEEKER_STORY_UPDATE", progress.ToString() });
    }

    //retrieves story progress for seeker game
    public void RetrieveStoryProgress(ref int progress)
    {
        if (_webClient == null) return;
        progress = (_playersSeeker["StoryProgress"].AsInt);
    }

    //retrieves population for seeker game
    public void RetrievePopulation(ref int population)
    {
        if (_webClient == null) return;
        population = (_playersSeeker["TownPopulation"].AsInt);
    }

    //updates population in seeker game
    public void SeekerPopUpdate(int population)
    {
        QueueMessage(new List<string>() { "SEEKER_POPULATION_UPDATE", population.ToString() });

    }


    /****************************************************************************
    ----------------------- PLAYER CHARACTER MANIPULATION -----------------------
    *****************************************************************************
    -This section holds all the functions that interact with the stats of a player.
    ****************************************************************************/




    //Initializes player stats (and gold count). Pass in reference to the player to make it work
    public void RetrievePlayerStats(GameObject PlayerObject)
    {
        //If client is null exit now
        if (_tcpClient == null) return;

        PlayerStats playerStats = PlayerObject.GetComponent<PlayerStats>();
        Inventory inventory = PlayerObject.GetComponent<Inventory>();
        PlayerController controller = PlayerObject.GetComponent<PlayerController>();

        int patk = _playerStats["PAtk"].AsInt;
        int matk = _playerStats["MAtk"].AsInt;
        int pdef = _playerStats["PDef"].AsInt;
        int mdef = _playerStats["MDef"].AsInt;
        int speed = _playerStats["Speed"].AsInt;
        int xp = _playerStats["XP"].AsInt;
        int playerLevel = _playerStats["Level"].AsInt;
        int skillPoints = _playerStats["SkillPoints"].AsInt;
        int statPoints = _playerStats["StatPoints"].AsInt;
        playerStats.InitFromDB(playerLevel, xp, skillPoints, statPoints, patk, matk, pdef, mdef, speed);

        inventory.Gold = _playerStats["Gold"].AsInt;
        //Set default PlayerName if needed
        string playerName = _playerStats["PlayerName"];
        if (string.IsNullOrEmpty(playerName))
        {
            controller.SetUsername(_username);
            UpdatePlayerStat("PlayerName", _username);
        }
        else
        {
            controller.SetUsername(_playerStats["PlayerName"]);
        }
    }

    public List<string> GetPlayerLocation()
    {
        if (_playerStats == null) return null;
        List<string> returner = new List<string>();

        returner.Add(_playerStats["CurrentScene"]);
        returner.Add(_playerStats["CurrentPositionX"]);
        returner.Add(_playerStats["CurrentPositionY"]);
        returner.Add(_playerStats["CurrentSceneManager"]);
        if (returner[3] == null) returner[3] = "";

        return returner;
    }

    //Call this to update any stat. Convert the value to a string first
    public void UpdatePlayerStat(string stat, string value)
    {
        QueueMessage(new List<string>() { "PLAYERSTAT_UPDATE", stat, value });
    }




    /****************************************************************************
    ---------------------------- RECIPES SECTION --------------------------------
    *****************************************************************************
    -This section is for both saving and loading recipes
    ****************************************************************************/

    /*/
	public int GetRecipeStatus(int id, int defaultState)
    {
        if (_recipes[id.ToString()] == null)
        {
            _recipes[id.ToString()].AsInt = defaultState;
            UpdateRecipe(id, defaultState);
        }
        return _recipes[id.ToString()].AsInt;
    }

    //Updates an achievement
    public void UpdateRecipe(int id, int state)
    {
        QueueMessage(new List<string>() { "RECIPE_UPDATE", id.ToString(), state.ToString() });
    }
	//*/



    /****************************************************************************
    ----------------------- PLAYER SKILLS SECTION -------------------------------
    *****************************************************************************
    -This section is for both saving and loading player skills
    ****************************************************************************/

    //Called from HotbarGUI
    public void EquipPlayerSkills(HotbarGUI hotbar, bool ActiveSkills)
    {
        foreach (JSONNode skill in _skills)
        {
            int id = skill["ID"].AsInt;
            int slot = skill["Slot"].AsInt;

            if (slot >= 0)
            {
                //Slots 0-9 are actives, 10-14 are passives
                if (ActiveSkills && slot < 10)
                    hotbar.EquipSkill(id, slot);
                else if (!ActiveSkills && slot >= 10)
                    hotbar.EquipSkill(id, slot - 10);
            }
        }
    }

    //Called from SkillGUI
    public bool SkillUnlocked(int skillID)
    {
        foreach (JSONNode skill in _skills)
        {
            if (skill["ID"].AsInt == skillID)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateSkill(int skillID, int slotNumber, bool activeSkillbar)
    {
        //Actives are the first 10 slots, passives are the next 5 slots
        if (!activeSkillbar && slotNumber >= 0)
        {
            slotNumber += 10;
        }
        //Update skills
        QueueMessage(new List<string>() { "SKILLS_UPDATE", skillID.ToString(), slotNumber.ToString() });
        //Update local array as well
        foreach (JSONNode skill in _skills)
        {
            if (skill["ID"].AsInt == skillID)
            {
                skill["Slot"] = slotNumber.ToString();
                return;
            }
        }
        //If not found, create new entry
        JSONClass newNode = new JSONClass();
        newNode["ID"] = skillID.ToString();
        newNode["Slot"] = slotNumber.ToString();
        _skills.Add(newNode);
    }



    /****************************************************************************
    ------------------- STATISTIC COLLECTION SECTION ----------------------------
    *****************************************************************************
    -This section holds a collection of methods that are responsible for interacting
    with the database for the purpose of collecting statistics.
    ****************************************************************************/

    //All of these have the same basic format. For all that accept parameters, pass how many were just <action>ed
    //Do NOT pass the total number of the action, or the server will save incorrect data

    public void DBPickupItems(int NumberOfItemsPickedUp)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "ItemsPickedUp", NumberOfItemsPickedUp.ToString() });
    }

    public void DBGetGold(int AmountOfGold)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "GoldCollected", AmountOfGold.ToString() });
    }

    public void DBRemoveGold(int AmountOfGold)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "GoldRemoved", AmountOfGold.ToString() });
    }

    public void DBCraftItem(int NumberOfItems)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "ItemsCrafted", NumberOfItems.ToString() });
        //Decrement number of crafts available
        CraftsAvailable--;
        if (CraftsAvailable < 0) CraftsAvailable = 0;
        QueueMessage(new List<string>() { "PLAYERSTAT_UPDATE", "CraftsAvailable", CraftsAvailable.ToString() });
    }

    public void DBGetQuestRewards(int NumberOfRewards)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "QuestRewardsEarned", NumberOfRewards.ToString() });
    }

    public void DBGetCombatRewards(int NumberOfRewards)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "CombatRewardsEarned", NumberOfRewards.ToString() });
    }

    public void DBDefeatEnemies(int NumberOfEnemiesDefeated)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "EnemeiesDefeated", NumberOfEnemiesDefeated.ToString() });
    }

    public void DBWinBattle()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "NumberOfBattles", 1.ToString() });
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "BattlesWon", 1.ToString() });
    }

    public void DBLoseBattle()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "NumberOfBattles", 1.ToString() });
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "BattlesLost", 1.ToString() });
    }

    public void DBRunAway()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "NumberOfBattles", 1.ToString() });
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "BattlesRanFrom", 1.ToString() });
    }

    public void DBExecuteAttack()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "AttacksExecuted", 1.ToString() });
    }

    public void DBExecuteHeal()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "HealsExecuted", 1.ToString() });
    }

    public void DBExecuteBuff()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "BuffsExecuted", 1.ToString() });
    }

    public void DBGainXP(int AmountOfXP)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "XPGained", AmountOfXP.ToString() });
    }

    public void DBGainLevel()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "LevelsGained", 1.ToString() });
    }

    public void DBSwitchInstance()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "InstanceSwitches", 1.ToString() });
    }

    public void DBClick()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "NumberOfClicks", 1.ToString() });
    }

    public void DBCompleteQuest()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "QuestsCompleted", 1.ToString() });
    }

    public void DBShortcutUsed(string shortcutName)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "ShortcutsUsed", 1.ToString(), shortcutName });
    }

    public void DBBulletinBoardAccessed()
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "BulletinBoardsAccessed", 1.ToString() });
    }

    public void DBAchievementCompleted(int id)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "AchievementCompleted", id.ToString() });
    }

    public void DBMilestoneCompleted(int id)
    {
        QueueMessage(new List<string>() { "STATISTIC_UPDATE", "AchievementMilestoneCompleted", id.ToString() });
    }

    /**************************************************************************
    ------------------------ QUEST SECTION ------------------------------------
    ***************************************************************************
    -This section is for loading the player's current quest progress and saving
    any quest changes (objective completions, quest completions, etc) to the
    database
    **************************************************************************/

    //Sets player's current quest progress
    public void RetrievePlayerQuestProgress(QuestManager qm)
    {
        //If client is null exit now
        if (_tcpClient == null) return;

        //Load list of quests in player document
        foreach (JSONClass quest in _quests)
        {
            //Parse document into id, state, num completions, and lists of active and complete objectives
            double id = quest["ID"].AsDouble;
            int state = quest["State"].AsInt;
            int numCompletions = quest["NumberOfCompletions"].AsInt;
            List<double> active = new List<double>();
            foreach (JSONData objective in quest["ActiveObjectives"].AsArray)
            {
                active.Add(objective.AsDouble);
            }
            List<double> completed = new List<double>();
            foreach (JSONData objective in quest["CompletedObjectives"].AsArray)
            {
                completed.Add(objective.AsDouble);
            }
            //Tell the quest manager to update this quest with this info
            qm.UpdateQuestFromDB(id, state, numCompletions, active, completed);
        }
    }

    //Update quest state
    public void QuestUpdateState(double questId, int state)
    {
        QueueMessage(new List<string>() { "QUEST_UPDATESTATE", questId.ToString(), state.ToString() });
    }

    //Update completion count
    public void QuestUpdateCompletionCount(double questId, int numCompletions)
    {
        QueueMessage(new List<string>() { "QUEST_UPDATECOMPLETIONCOUNT", questId.ToString(), numCompletions.ToString() });
    }

    //Update quest objective
    public void QuestUpdateObjective(double questId, double objectiveId, char from, char to)
    {
        QueueMessage(new List<string>() { "QUEST_UPDATEOBJECTIVE", questId.ToString(), objectiveId.ToString(), from.ToString(), to.ToString() });
    }

    //Clear ActiveObjectives and CompletedObjectives lists
    public void QuestClearObjectives(double questId)
    {
        QueueMessage(new List<string>() { "QUEST_CLEAROBJECTIVES", questId.ToString() });
    }

    /**************************************************************************
    ------------------------ TOWN BOARD SECTION -------------------------------
    ***************************************************************************
    Town board storage. This is sent as part of the login document and parsed
    and stored here for use by the TownBoard class.
    **************************************************************************/

    private void TB_ParseProgressBar(JSONNode progressBar)
    {
        TB_Progress = progressBar["Progress"].AsDouble;
        TB_ProgressTitle = progressBar["Title"];
        TB_ProgressDesc = progressBar["Desc"];
    }

    private void TB_ParsePageText(JSONArray pages)
    {
        //Six pages
        TB_Pages = new List<string>[6];

        int index = 0;

        foreach(JSONClass page in pages)
        {
            List<string> newPage = new List<string>();

            //Set title
            newPage.Add(page["Title"]);
            //Set text
            foreach(JSONData text in page["Pages"].AsArray)
            {
                newPage.Add(text);
            }
            TB_Pages[index] = newPage;
            index++;
        }
    }

    /**************************************************************************
    ------------------------- UDP SECTION -------------------------------------
    ***************************************************************************
    -Handful of functions related to multiplayer functionality
    **************************************************************************/



    //UDP - link player's transform to UdpManager
	/*/
    public void UdpLinkPlayerController(PlayerController controller)
    {
        if(_udp.playerController == null)
        {
            _udp.playerController = controller;
        }
    }

    public void UdpUpdateLevelName(string levelName)
    {
        lock(_udp.levelLock)
        {
            _udp.levelName = levelName;
        }
    }

    public void UdpUpdatePosition(float xCoord, float yCoord)
    {
        _udp.xCoord = xCoord;
        _udp.yCoord = yCoord;
    }

    //Gets level index from level name
    //REMEMBER to update the "worldCount" variable in World.cs on the server when changing this list
    public static int GetLevelIndex(string levelName)
    {
        if (levelName == "Town")    return 1;
        if (levelName == "Forest")  return 2;
        if (levelName == "Mountain") return 3;
        if (levelName == "Forest 2") return 4;
        if (levelName == "Forest 3") return 5;
        return 0;
    }
	//*/
}
