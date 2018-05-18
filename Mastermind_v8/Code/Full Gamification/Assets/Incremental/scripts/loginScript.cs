using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class loginScript : MonoBehaviour {
    

    public Text id;
    public GameObject pass;
    public Text error;
    public NetworkManager network;
    public string username;
    public string password;
	// Use this for initialization
	void Start ()
    {
        error.enabled = true;
        //network = NetworkManager.Instance;
        //network.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(network.enabled)
        {
            error.text = network.loginFlag.ToString();
            if (network.loginFlag == -1)
            {
                error.text = "Username not found.";
            }
            //password doesn't match
            else if (network.loginFlag == -2)
            {
                error.text = "Password doesn't match.";
            }
            //login successful
            else if (network.loginFlag == 1)
            {
                error.text = "Login successful.";
            }
            else if (network.loginFlag == 2)
            {
                error.text = "Loading player's data...";
            }
            else if (network.loginFlag == 3)
            {
                error.text = "Loading Done.";
            }
            else if (network.loginFlag == 0)
            {
                error.text = "Server is not connected.";
            }
        }
    }

    public void login()
    {
        if(tryLogin())
        {
            //if valid, get user information and store to global variable
            //to share between ALL SCENES
            //while (!NetworkManager.Instance._isLoggedIn) { }
            GameObject.Find("PlayerData").GetComponent<GlobalControl>().enabled = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("ui", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            error.enabled = true;
        }
    }

    private bool tryLogin()
    {
        //network.StartConnection(id.text, pass.GetComponent<InputField>().text);
        Debug.Log("Start connection...");

        //network.enabled = true;
        GameObject.Find("networkManager").GetComponent<NetworkManager>().enabled = true;

        Debug.Log("Logging into server...");
        /*
         -1 is username not found
         -2 is password doesn't match
         1 is login successful
         2 is loading data
         */

        return network.SetLoginCredentials(id.text, pass.GetComponent<InputField>().text);
    }

    public void loginPass()
    {
        gameStart();
    }



    public void gameStart()
    {
        if(player.localHasData == 0)
        {
            createNewData();
        }
        else
        {
            updatePassiveProgress(DateTime.Now);
        }
        //title collection
        //not yet implemented
        UnityEngine.SceneManagement.SceneManager.LoadScene("ui", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void createNewData()
    {
        player.localHasData = 1;
        player.lv = 1;
        player.coin.active = 0;
        player.coin.passive = 0;
        player.progress.cur = 0;
        player.exp.cur = 0;
        player.stamina.cur = 10;
        player.stamina.max = 10;        //max stamina changed to 10
        player.progress.P_upgradeLv = 1;
        player.coin.P_upgradeLv = 1;
        player.exp.P_upgradeLv = 1;
        player.progress.A_upgradeLv = 1;
        player.coin.A_upgradeLv = 1;
        player.exp.A_upgradeLv = 1;
        player.username = "New User";
        player.timeleft.cur = 3599;
        player.timeleft.max = 3600;
        //booster
        player.coin.numBooster = 5;
        player.exp.numBooster = 5;
        player.progress.numBooster = 5;
        //
    }

    public static int updatePassiveProgress(DateTime lastLogOut)
    {
        TimeSpan span = DateTime.Now.Subtract(lastLogOut);
        Debug.Log(span.TotalSeconds);
        player.progress.cur += bal.getPassiveProgressBarRate() * span.TotalSeconds * 60;
        return 0;
    }

    //this function is ONLY for debuging purpose
    public static int updateActiveProgress(DateTime time)
    {
        TimeSpan span = DateTime.Now.Subtract(time);
        Debug.Log(span.TotalSeconds);
        player.progress.cur += bal.getActiveProgressBarRate() * span.TotalSeconds * 60;
        return 0;
    }

    //test. maybe pass json file to build variables 
    public void _storePlayerInfo(string jsonFile)
    {

    }
}


