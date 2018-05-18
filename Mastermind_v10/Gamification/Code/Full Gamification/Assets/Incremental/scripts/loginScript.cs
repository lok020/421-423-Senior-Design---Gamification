using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class loginScript : MonoBehaviour {
    public Text id;
    public GameObject pass;
    public Text error;
    public NetworkManager network;

    bool tryLogin = false;

	// Use this for initialization
	void Start ()
    {
        error.enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(tryLogin)
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
                //network has all users data in string form;
                error.text = "Loading Done.";
                //if valid, get user information and store to global variable
                //to share between ALL SCENES
                gameStart();
            }
            else if (network.loginFlag == 0)
            {
                error.text = "Server is not connected.";
            }
        }
    }

    //Enter button pressed 
    public void login()
    {
        network.StartConnection(id.text, pass.GetComponent<InputField>().text);
        tryLogin = true;
        Debug.Log("Start connection...");
    }
    
    public void gameStart()
    {
        if(player.Incre.startNew)
        {
            player.Incre.startNew = false;
        }
        else
        {
            updatePassiveProgress(DateTime.Now);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("ui", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void createNewData()
    {

        /*
        player.Incre.localHasData = 1;
        player.Incre.lv = 1;
        player.Incre.coin.active = 0;
        player.Incre.coin.passive = 0;
        player.Incre.progress.cur = 0;
        player.Incre.exp.cur = 0;
        player.Incre.stamina.cur = 10;
        player.Incre.progress.P_upgradeLv = 1;
        player.Incre.coin.P_upgradeLv = 1;
        player.Incre.exp.P_upgradeLv = 1;
        player.Incre.progress.A_upgradeLv = 1;
        player.Incre.coin.A_upgradeLv = 1;
        player.Incre.exp.A_upgradeLv = 1;
        player.Incre.username = "New User";
        player.Incre.timeleft.cur = 3599;
        //booster
        player.Incre.coin.numBooster = 5;
        player.Incre.exp.numBooster = 5;
        player.Incre.progress.numBooster = 5;
        player.Incre.permanentPerksLV = 1;
        */
        //
    }
    public static int updatePassiveProgress(DateTime lastLogOut)
    {
        TimeSpan span = DateTime.Now.Subtract(lastLogOut);
        Debug.Log(span.TotalSeconds);
        player.Incre.progress.cur += bal.getPassiveProgressBarRate() * span.TotalSeconds * 60;
        player.Incre.debugging = true;
        return 0;
    }

    //this function is ONLY for debuging purpose
    public static int updateActiveProgress(DateTime time)
    {
        TimeSpan span = DateTime.Now.Subtract(time);
        Debug.Log(span.TotalSeconds);
        player.Incre.progress.cur += bal.getActiveProgressBarRate() * span.TotalSeconds * 60;
        player.Incre.debugging = true;
        return 0;
    }
}


