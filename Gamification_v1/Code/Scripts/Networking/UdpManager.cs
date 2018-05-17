using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class UdpManager {

    //Networking
    private Socket _udpSocket;          //Socket over which all communications take place
    private int _posRecvCtr, _txtRecvCtr, _rosRecvCtr;      //Receive counters
    private int _posSendCtr, _txtSendCtr;                   //Send counters

    //Message types:  0 = position  1 = text  2 = roster

    //Message frequency
    private static int _targetTimeBetweenTicks = 100;   //Try to send a message every 100ms

    //Thread cancellation variable
    private volatile bool _shouldExit = false;

    //Public variables, because some required functions are only allowed in the game thread
    public PlayerController playerController;
    public string levelName;
    public object levelLock = new object();
    public float xCoord, yCoord;
    public bool ServerUdpReady = false;

    //Virtual players! This class manages them
    private PlayerRoster _roster;

    //Etc
    private static Encoding _encoder = Encoding.UTF8;
    private string _uniqueId;
    private byte[] _uniqueIdBytes;

    // Constructor
    public UdpManager(IPAddress serverAddress, Int32 udpPort, string uniqueId, PlayerRoster roster)
    {
        levelName = "";
        _uniqueId = uniqueId;
        _uniqueIdBytes = Convert.FromBase64String(_uniqueId);
        //Debug.Log("Unique ID: " + uniqueId); 
        xCoord = 0;
        yCoord = 0;
        _roster = roster;

        //Local end point (let the OS pick a UDP port number for us)
        IPEndPoint localPoint = new IPEndPoint(IPAddress.Any, 0);
        IPEndPoint serverPoint = new IPEndPoint(serverAddress, udpPort);

        _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _udpSocket.Bind(localPoint);
        _udpSocket.Connect(serverPoint);

        //Start the sending and receiving tasks
        Thread sendingThread = new Thread(SendingTask);
        sendingThread.Start();

        Thread receivingThread = new Thread(ReceivingTask);
        receivingThread.Start();
    }

    //Connect to server, by transmitting a unique identifier until the server acknowledges via TCP
    private void ConnectToServer()
    {
        //Send a message with the unique ID every second until told to stop
        while(!ServerUdpReady && !_shouldExit)
        {
            _udpSocket.Send(_uniqueIdBytes);
            Thread.Sleep(1000);
        }

        if(!_shouldExit) Debug.Log("Connected to multiplayer server!");
    }

    //Receiving thread
    private void ReceivingTask()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (!_shouldExit)
            {
                //Receive message and trim it
                _udpSocket.Receive(buffer);
                byte[] data = TrimMessage(buffer);
                //Debug.Log("Received message! Sender: " + point + ", size: " + data.Length + " bytes");

                //Parse the message
                Parse(data);
            }
        }
        catch(Exception)
        {
        }
        //Restart if needed
        if (!_shouldExit) ReceivingTask();
    }

    //Sending thread
    private void SendingTask()
    {
        //Connect to the server
        ConnectToServer();

        //If this was killed early, exit now
        if (_shouldExit)
            return;

        //Prepare for main thread
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        long timeElapsed = 0;

        byte[] numBuf;
        byte[] positionBytes = new byte[16];
        string text;
        List<byte> encodedPositionMessage = new List<byte>();
        List<byte> encodedTextMessage = new List<byte>();
        
        //Now sit in an infinite loop of sending messages, roughly every 50ms
        while (!_shouldExit)
        {
            //This shouldn't fail, but if it does it will pick up where it left off
            try
            {
                //Start stopwatch
                timeElapsed = 0;
                watch.Reset();
                watch.Start();

                //Clear stuff
                Array.Clear(positionBytes, 0, 16);
                encodedPositionMessage.Clear();
                encodedTextMessage.Clear();

                //If player controller isn't created, wait
                if (playerController == null)
                {
                    Thread.Sleep(_targetTimeBetweenTicks);
                    continue;
                }
                //Do stuff
                //First get position info
                int sceneIndex = 0;
                lock (levelLock)
                {
                    sceneIndex = NetworkManager.GetLevelIndex(levelName);
                }
                //Create position array
                numBuf = BitConverter.GetBytes(sceneIndex);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                Array.Copy(numBuf, 0, positionBytes, 0, 4);
                numBuf = BitConverter.GetBytes(xCoord);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                Array.Copy(numBuf, 0, positionBytes, 4, 4);
                numBuf = BitConverter.GetBytes(yCoord);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                Array.Copy(numBuf, 0, positionBytes, 8, 4);
                //Send position
                encodedPositionMessage.AddRange(_uniqueIdBytes);
                encodedPositionMessage.AddRange(Encode(positionBytes, 0));
                byte[] epm = encodedPositionMessage.ToArray();
                _udpSocket.Send(epm);
                _posSendCtr++;
                //Debug.Log("Sent UDP pos update " + _posSendCtr + ", bytes: " + epm.Length);

                //Get and send text, if there is any
                text = playerController.GetTextMessage();
                if (!string.IsNullOrEmpty(text))
                {
                    encodedTextMessage.AddRange(_uniqueIdBytes);
                    encodedTextMessage.AddRange(Encode(_encoder.GetBytes(text), 1));
                    byte[] etm = encodedTextMessage.ToArray();
                    _udpSocket.Send(etm);
                    _txtSendCtr++;
                    //Debug.Log("Sent UDP txt update " + _txtSendCtr);
                }
            }
            catch (Exception) { }
            
            //Stop stopwatch and sleep until ready for next tick
            watch.Stop();
            timeElapsed = watch.ElapsedMilliseconds;
            if(timeElapsed < _targetTimeBetweenTicks)
            {
                //Debug.Log("Sleeping for " + (_targetTimeBetweenTicks - timeElapsed) + "ms");
                Thread.Sleep((int)(_targetTimeBetweenTicks - timeElapsed));
            }

        }
    }

    //Encode - adds a basic header to the message, indicating packet # and message type
    private byte[] Encode(byte[] message, int messageType)
    {
        //Create the header
        List<byte> encodedMessage = new List<byte>();
        //Message type
        byte[] numBuf = BitConverter.GetBytes(messageType);
        if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
        encodedMessage.AddRange(numBuf);
        //Packet #
        int packetNumber = 0;
        if (messageType == 0) packetNumber = _posSendCtr;
        else if (messageType == 1) packetNumber = _txtSendCtr;
        numBuf = BitConverter.GetBytes(packetNumber);
        if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
        encodedMessage.AddRange(numBuf);
        //Data length (only for text messages)
        if (messageType == 1)
        {
            numBuf = BitConverter.GetBytes(message.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
            encodedMessage.AddRange(numBuf);
        }
        //Data
        encodedMessage.AddRange(message);

        return encodedMessage.ToArray();
    }

    //Parse - as the name implies, parses all the received data
    //There are three kinds of messages to handle, each with their own parsing
    private void Parse(byte[] message)
    {
        //Get message type
        byte[] numBuf = new byte[4];
        Array.Copy(message, 0, numBuf, 0, 4);
        if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
        int messageType = BitConverter.ToInt32(numBuf, 0);
        //Get packet number
        Array.Copy(message, 4, numBuf, 0, 4);
        if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
        int packetNumber = BitConverter.ToInt32(numBuf, 0);
        //Data length
        Array.Copy(message, 8, numBuf, 0, 4);
        if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
        int dataLength = BitConverter.ToInt32(numBuf, 0);

        //Before parsing any more, make sure this isn't significantly out of order
        if ((messageType == 0 && _posRecvCtr - packetNumber > 2) ||
           (messageType == 1 && _txtRecvCtr - packetNumber > 2) ||
           (messageType == 2 && _rosRecvCtr - packetNumber > 2))
        {
            Debug.Log("! Out of order packet recv from server, type " + messageType);
            return;
        }

        //Debug.Log("Received UDP message, type " + messageType + ", size " + dataLength);

        //Parse position message
        if(messageType == 0)
        {
            int playerCount = 0;
            int offset = 12;        //Note that we've already read 12 bytes
            //Each player has 12 bytes of data: int32 id, float xCoord, float yCoord
            while(playerCount*12 < dataLength)
            {
                //Player ID
                Array.Copy(message, (playerCount * 12) + offset + 0, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int playerId = BitConverter.ToInt32(numBuf, 0);
                //X coord
                Array.Copy(message, (playerCount * 12) + offset + 4, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                float xCoord = BitConverter.ToSingle(numBuf, 0);
                //Y coord
                Array.Copy(message, (playerCount * 12) + offset + 8, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                float yCoord = BitConverter.ToSingle(numBuf, 0);

                //Debug.Log("Position of player " + playerId + ": " + xCoord + ", " + yCoord);
                _roster.QueuePositionUpdate(playerId, xCoord, yCoord);
                playerCount++;
            }
            _posRecvCtr++;
        }
        //Parse text message
        else if (messageType == 1)
        {
            int offset = 12;
            int counter = 0;
            //Each text message is of variable length
            while (counter < dataLength)
            {
                //Player ID
                Array.Copy(message, counter + offset + 0, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int playerId = BitConverter.ToInt32(numBuf, 0);
                //Text length
                Array.Copy(message, counter + offset + 4, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int textLength = BitConverter.ToInt32(numBuf, 0);
                //Name
                byte[] textBytes = new byte[textLength];
                Array.Copy(message, counter + offset + 8, textBytes, 0, textLength);
                string text = _encoder.GetString(textBytes);

                //Debug.Log("Text from player " + playerId + ": \"" + text + "\"");
                counter += 8 + textLength;

                _roster.QueueTextUpdate(playerId, text);
            }
            _txtRecvCtr++;
        }
        //Parse roster message
        else if(messageType == 2)
        {
            //Debug.Log("Roster update received, size: " + dataLength);
            int offset = 12;
            int counter = 0;
            //Each roster message is of variable length
            while(counter < dataLength)
            {
                //Player ID
                Array.Copy(message, counter + offset + 0, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int playerId = BitConverter.ToInt32(numBuf, 0);
                //Quality
                Array.Copy(message, counter + offset + 4, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int level = BitConverter.ToInt32(numBuf, 0);
                //Sprite ID
                Array.Copy(message, counter + offset + 8, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int spriteId = BitConverter.ToInt32(numBuf, 0);
                //Name length
                Array.Copy(message, counter + offset + 12, numBuf, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(numBuf);
                int nameLength = BitConverter.ToInt32(numBuf, 0);
                //Name
                byte[] nameBytes = new byte[nameLength];
                Array.Copy(message, counter + offset + 16, nameBytes, 0, nameLength);
                string name = _encoder.GetString(nameBytes);

                //Debug.Log("Info on player '" + name + "'");
                //Debug.Log("  ID: " + playerId);
                //Debug.Log("  Quality: " + level);
                //Debug.Log("  Sprite ID: " + spriteId);

                _roster.QueueRosterUpdate(playerId, level, spriteId, name);

                counter += 16 + nameLength;
            }
            _rosRecvCtr++;
        }
    }

    //End tasks and close everything
    public void Close()
    {
        _shouldExit = true;
        _udpSocket.Close();
    }

    private byte[] TrimMessage(byte[] fullMessage)
    {
        int i = fullMessage.Length - 1;
        while (fullMessage[i] == 0 && i >= 0) i--;
        if (i < 0) return fullMessage;
        byte[] trimmedMessage = new byte[i + 1];
        Array.Copy(fullMessage, trimmedMessage, i + 1);
        return trimmedMessage;
    }
}
