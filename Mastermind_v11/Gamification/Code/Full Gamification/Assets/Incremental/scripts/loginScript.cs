using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class loginScript : MonoBehaviour {
    public Text id;
    public GameObject pass;
    public Text status;
    public NetworkManager network;
    bool tryLogin = false;
    EventSystem system;
    // Use this for initialization
    void Start ()
    {
        status.enabled = false;
        system = EventSystem.current;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            //else Debug.Log("next nagivation element not found");

        }
        if (tryLogin)
        {
            status.enabled = true;
            status.text = network.loginFlag.ToString();
            if (network.loginFlag == -1)
            {
                status.text = "Username not found.";
            }
            //password doesn't match
            else if (network.loginFlag == -2)
            {
                status.text = "Password doesn't match.";
            }
            //login successful
            else if (network.loginFlag == 1)
            {
                status.text = "Login successful.";
            }
            else if (network.loginFlag == 2)
            {
                status.text = "Loading player's data...";
            }
            else if (network.loginFlag == 3)
            {
                //network has all users data in string form;
                
                //_loginData[]
                status.text = "Loading Done.";
                //Debug.Log(NetworkManager.Instance.loaded_json);
                JsonStrings loadedjsonStrings = JsonConvert.DeserializeObject<JsonStrings>(NetworkManager.Instance.loaded_json);

                Debug.Log(loadedjsonStrings.Incremental);
                Debug.Log(loadedjsonStrings.Mastermind);
                Debug.Log(loadedjsonStrings.Conqueror);
                Debug.Log(loadedjsonStrings.Seeker);

                if (loadedjsonStrings.Incremental.Length > 0)
                {
                    try
                    {
                        player.Incre = JsonConvert.DeserializeObject<IncrementalData>(loadedjsonStrings.Incremental);
                        status.text = "Incremental Loading Done.";
                    }
                    catch
                    {
                        player.Incre = new IncrementalData();
                        status.text = "Incremental Loading ERROR.";
                    }
                }
                if(loadedjsonStrings.Mastermind.Length > 0)
                {
                    try
                    {
                        player.Sudoku = JsonConvert.DeserializeObject<sudokuData>(loadedjsonStrings.Mastermind);
                        status.text = "Mastermind Loading Done.";
                    }
                    catch
                    {
                        player.Sudoku = new sudokuData();
                        status.text = "Mastermind Loading Error.";
                    }
                }
                if (loadedjsonStrings.Conqueror.Length > 0)
                {
                    /*
                    try
                    {
                        player.Sudoku = JsonConvert.DeserializeObject<sudokuData>(loadedjsonStrings.Mastermind);
                        status.text = "Mastermind Loading Done.";
                    }
                    catch
                    {
                        player.Sudoku = new sudokuData();
                        status.text = "Mastermind Loading Error.";
                    }
                    */
                }
                if (loadedjsonStrings.Seeker.Length > 0)
                {
                    try
                    {
                        GlobalControl.Instance = JsonConvert.DeserializeObject<GlobalControl>(loadedjsonStrings.Seeker);
                        status.text = "Seeker Loading Done.";
                    }
                    catch
                    {
                        status.text = "Loading Seeker Error. Initialize Data.";
                        GlobalControl.Instance = new GlobalControl();
                    }
                }
                status.text = "Loading Done.";
                gameStart();
            }
            else if (network.loginFlag == 0)
            {
                status.text = "Server is not connected.";
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
    
    public void localLogin()
    {
        player.Incre.stamina.cur = 15;
        player.Incre.needTutorial = false;
        player.Incre.timeleft.cur = 60 * 60;
        gameStart();
    }

    public void gameStart()
    {
        if(player.Incre.startNew)
        {
            player.Incre.startNew = false;
        }
        else //continue user
        {
            updatePassiveProgress(player.Incre.lastLogOut);
            player.Incre.currentGame = minigame.none;
            player.Incre.gameON = false;
            player.Incre.passive = true;
            player.Incre.startMessageDisplayed = false;
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
        player.Incre.LogOutpassiveProgressGained = Convert.ToInt32(span.TotalSeconds);
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


public class JsonStrings
{
    public string Seeker;
    public string Mastermind;
    public string Incremental;
    public string Conqueror;
}
