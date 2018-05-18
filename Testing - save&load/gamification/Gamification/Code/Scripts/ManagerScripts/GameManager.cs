using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

//*
public class GameManager : MonoBehaviour
{
    public GameObject DatabaseManager, UsernameBox, PasswordBox, BonusCodeBox, ErrorMessage;
    public GameObject PlayerPrefab, MessageOverlayPrefab;

    //Log user in
    public void Login()
    {
        //Get entered username and password
        string username = UsernameBox.GetComponent<InputField>().text;
        string password = PasswordBox.GetComponent<InputField>().text;
        string bonusCode = BonusCodeBox.GetComponent<InputField>().text;
        //Attempt user login
        int status = DatabaseManager.GetComponent<NetworkManager>().PlayerLogin(username, password, bonusCode);
        //Log user in if successful (status == 0)
        if (status == 0)
        {
            StartGame();
            return;
        }
        //Else print error code
        //Get error message
        if (status == -1)    //Username not found
        {
            ErrorMessage.GetComponent<Text>().text = "Error: Username not found";
            ErrorMessage.GetComponent<Text>().color = Color.red;
        }
        else if (status == -2) //Password incorrect
        {
            ErrorMessage.GetComponent<Text>().text = "Error: Password incorrect";
            ErrorMessage.GetComponent<Text>().color = Color.red;
        }
    }

    public void StartGame()
    {
        string sceneName = "Town";
        float xPos = -12.4f;
        float yPos = -0.6f;
        string sceneManagerName = string.Empty;
        //Get the player's last location from the database
        var loc = DatabaseManager.GetComponent<NetworkManager>().GetPlayerLocation();
        if (loc != null)
        {
            sceneName = loc[0];
            xPos = float.Parse(loc[1]);
            yPos = float.Parse(loc[2]);
            sceneManagerName = loc[3];
        }
        //Run without database
        else
        {
            DatabaseManager.GetComponent<NetworkManager>().RunWithoutServer = true;
        }
        //Create player
        GameObject player = Instantiate(PlayerPrefab) as GameObject;
        //Create message overlay
        Instantiate(MessageOverlayPrefab);
        //Load scene manager if going to a scene with one
        if (sceneManagerName.Length > 0)
        {
            string fullResourcePath = "Scenes/" + sceneManagerName;
            GameObject manager = Instantiate(Resources.Load(fullResourcePath)) as GameObject;
            player.GetComponent<PlayerController>().nextLevel = new SceneManagerInstance(manager.GetComponent<SceneManager>());
        }
        //Load level
        player.transform.position = new Vector3(xPos, yPos, 0);
        DontDestroyOnLoad(player);
        Application.LoadLevel(sceneName);
    }
}//*/

/*
public class GameManager : NetworkManager 
{
    public GameObject DatabaseManager, UsernameBox, PasswordBox, ErrorMessage;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //Vector3 spawnPos = Vector3.right * conn.connectionId;
        GameObject player = (GameObject)Instantiate(base.playerPrefab, new Vector3(5,0,0), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    //Log user in
    public void Login()
    {
        //Get entered username and password
        string username = UsernameBox.GetComponent<InputField>().text;
        string password = PasswordBox.GetComponent<InputField>().text;
        //Attempt user login
        int status = DatabaseManager.GetComponent<DatabaseManager>().PlayerLogin(username, password);
        //Log user in if successful (status == 0)
        if(status == 0)
        {
            Debug.Log(NetworkManager.singleton.networkAddress);
            Debug.Log(NetworkManager.singleton.networkPort);
            NetworkManager.singleton.StartHost();
            return;
        }
        //Else print error code
        //Get error message
        if(status == -1)    //Username not found
        {
            ErrorMessage.GetComponent<Text>().text = "Error: Username not found";
        }
        else if(status == -2) //Password incorrect
        {
            ErrorMessage.GetComponent<Text>().text = "Error: Password incorrect";
        }
    }

    public void StartGame()
    {
        Debug.Log(NetworkManager.singleton.networkAddress);
        Debug.Log(NetworkManager.singleton.networkPort);
        NetworkManager.singleton.StartClient();
    }

    public new void StartServer()
    {
        Debug.Log(NetworkManager.singleton.networkAddress);
        Debug.Log(NetworkManager.singleton.networkPort);
        NetworkManager.singleton.StartServer(); //change back to StartHost to get Pre-Server functionality
        Application.LoadLevel("Lobby");
    }
	
    public new void StartHost()
    {
        Debug.Log(NetworkManager.singleton.networkAddress);
        Debug.Log(NetworkManager.singleton.networkPort);
        NetworkManager.singleton.StartHost();
    }
}
//*/
