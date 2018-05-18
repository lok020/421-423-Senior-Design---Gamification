using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loginScript : MonoBehaviour {
    

    public Text id;
    public GameObject pass;
    public Text error;

	// Use this for initialization
	void Start () {
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
            storePlayerInfo();
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
        storePlayerInfo();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ui", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    //test. maybe pass json file to build variables 
    public void _storePlayerInfo(string jsonFile)
    {

    }
    public void storePlayerInfo()
    {
        player.level = 1;
        player.coin_passive = 1000;
        player.coin_active = 1000;
        player.progress_cur = 1;
        player.exp_cur = 1;
        player.stemina_cur = 1000;
        player.passive_progressBarRateLV = 1;
        player.passive_CoinBonusLV = 1;
        player.passive_expBonusLV = 1;
        player.active_progressBarRateLV = 1;
        player.active_CoinBonusLV = 1;
        player.active_expBonusLV = 1;
        player.username = "Test user";

        player.stemina_cur = 1000;
        player.time_left = 1000;

        /*
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("coin_passive", coin_passive);
        PlayerPrefs.SetInt("coin_active", coin_active);
        PlayerPrefs.SetFloat("progress_cur", progress_cur);
        PlayerPrefs.SetFloat("exp_cur", exp_cur);
        PlayerPrefs.SetFloat("stemina_cur", stemina_cur);
        PlayerPrefs.SetInt("lv_passive_progress", lv_passive_progress);
        PlayerPrefs.SetInt("lv_passive_coinBonus", lv_passive_coinBonus);
        PlayerPrefs.SetInt("lv_passive_exp", lv_passive_exp);
        PlayerPrefs.SetInt("lv_active_progress", lv_active_progress);
        PlayerPrefs.SetInt("lv_active_coinBonus", lv_active_coinBonus);
        PlayerPrefs.SetInt("lv_active_exp", lv_active_exp);
        PlayerPrefs.SetString("username", username);
        */
    }
}


