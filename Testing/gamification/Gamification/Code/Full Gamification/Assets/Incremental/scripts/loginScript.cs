using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loginScript : MonoBehaviour {
    

    public Text id;
    public GameObject pass;
    public Text error;
	// Use this for initialization
	void Start ()
    {
        error.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void login()
    {
        if(validUser())
        {
            //if valid, get user information and store to global variable
            //to share between ALL SCENES
            UnityEngine.SceneManagement.SceneManager.LoadScene("incremental", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            error.enabled = true;
        }
    }

    private bool validUser()
    {
        if (id.text == "11415016" && pass.GetComponent<InputField>().text == "123123")
            return true;
        else
            return false;
    }

    public void loginPass()
    {
        gameStart();
    }



    public void gameStart()
    {
        //player.localHasData = 0;
        if (player.localHasData == 0)
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


