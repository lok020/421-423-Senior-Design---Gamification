using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.SceneManagement;
using System;

//delegate
public delegate void strDele(string a, string b);
public class update : MonoBehaviour {
    public GameObject upgrade;
    public GameObject bonus;
    public GameObject dialog;
    public GameObject dialog_yes;
    public GameObject dialog_no;
    public GameObject dialog_okay;
    public GameObject mainScreen;
    public Slider bar_progress;
    public Slider bar_exp;
    public Slider bar_timeLeft;

    
    public Text txt_progress;
    public Text txt_exp;
    public Text txt_welcomeMsg;
    public Text txt_timeLeft;
    public Text txt_stamina;

    public Text txt_redeem_pc;
    public Text txt_redeem_ac;
    public Text txt_redeem_exp;

    public Text txt_mode;
    public Text txt_level;
    public Text txt_passiveCoin;
    public Text txt_activeCoin;
    public Text txt_message;
    public Text txt_messageTitle;
    public Text bonusCodeInput;
    public Text txt_exit;

    public Text playerInfoDisplay;

    public Button passiveActiveButton;
    public Image progressBarImage;
    public Text msglog;
    
    string nextScene;
    int timeLeftFrameCount;
    int frameCount;
    bool upgradeWindow;
    bool bonusWindow;
    dialogMode _dialogMode;
    string localSaveData;

    void Start()
    {
        //test purpose
        player.Incre.stamina.cur = 10;
        player.Incre.needTutorial = false;
        player.Incre.timeleft.cur = 60 * 60;
        sendMsg("Welcome " + player.Incre.username + ":D");
        updatePlayerInfo();
      //set upgrade window
        upgradeWindow = false;
        bonusWindow = false;
        upgrade.SetActive(upgradeWindow);
        bonus.SetActive(bonusWindow);
        timeLeftFrameCount = 0;
        frameCount = 100;
        if(!player.Incre.gameON)
        {
            player.Incre.passive = true;
        }
        else
        {
            player.Incre.passive = false;
        }
        if(player.Incre.timeleft.cur <= 0)
            showMessage("Please attend class to play more.\r\nThe minigame is closed. \r\nNo worry, your game was saved.", "Need to study.");
        // Vector3 newpos = new Vector3(0, 0, 0);
        //upgrade.transform.position = newpos;
        //Screen.SetResolution(1325, 895, false);
        
    }
    
	// Update is called once per frame
	void Update ()
    {
        updateModeButton();
        updateTimeBar();
        updateStamina();
        updateProgressBar();
        updateLv();
        updateCoin();
        updateRedeemText();
        updateExpBar();
        timeLeftFrameCount++;
        mainScreen.SetActive(!player.Incre.gameON);
        if (bar_progress.value <= 0.021f)
        {
            bar_progress.value = 0.021f;
        }
        if(bar_exp.value <= 0.021f)
        {
            bar_exp.value = 0.021f;
        }
        if (player.Incre.gameON)
            txt_exit.text = "Close Game";
        else
            txt_exit.text = "Exit Game";
        
    }

    public void sendMsg(string msg)
    {
        msglog.text = ">> " + msg;
    }

    void showMessage(string msg, string title)
    {
        dialog.active = true;
        dialog_yes.active = false;
        dialog_no.active = false;
        dialog_okay.active = true;
        txt_message.text = msg;
        txt_messageTitle.text = title;
    }

    void showMessage2(string msg, string title, dialogMode mode)
    {
        dialog.active = true;
        dialog_yes.active = true;
        dialog_no.active = true;
        dialog_okay.active = false;
        txt_message.text = msg;
        _dialogMode = mode;
        txt_messageTitle.text = title;
    }

    public void exitPressed()
    {
        if(player.Incre.gameON)
        {
            nextScene = "ui";
            player.Incre.nextGame = minigame.none;
            showMessage2("Do you want to close the minigame?", "Are you sure?", dialogMode.exitgame);
        }
        else
        {
            showMessage2("Do you want to close the whole game?", "Sure?", dialogMode.exitapplication);
        }
    }
    
    public void resetPressed()
    {
        showMessage2("Do you want to reset the entire game? All progress will be lost.", "Reset Game", dialogMode.resetGame);
    }

    public void buttonYesOrNoClicked(bool yesno)
    {
        if(yesno)
        {
            if(_dialogMode == dialogMode.changeGame)
            {
                player.Incre.currentGame = player.Incre.nextGame;
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            }
            if(_dialogMode == dialogMode.exitgame)
            {
                player.Incre.currentGame = minigame.none;
                player.Incre.gameON = false;
                player.Incre.passive = true;
                SceneManager.LoadScene("ui", LoadSceneMode.Single);
            }
            if(_dialogMode == dialogMode.exitapplication)
            {
                Application.Quit();
            }
            if(_dialogMode == dialogMode.resetGame)
            {
                GlobalControl.Instance.ResetGame();
                showMessage("The game has been reset.", "Game Reset");
            }
        }
        else
        {
            
        }
        dialog.active = false;
    }

    void updatePlayerInfo()
    {
        playerInfoDisplay.text = string.Format("-player info-\r\nid: {0}\r\nreset: {1}", player.Incre.username, player.Incre.permanentPerksLV);
    }

    void updateRedeemText()
    {
        //passive mode
        if(player.Incre.passive)
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", bal.getPassiveCoinBonus()* player.Incre.coin.boosterRate);
            txt_redeem_ac.text = string.Format("ac: +{0}", 0);
            txt_redeem_exp.text = string.Format("exp: +{0}", bal.getPassiveEXPRate() * player.Incre.exp.boosterRate);
        }
        else
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", bal.getPassiveCoinBonus() * player.Incre.coin.boosterRate);
            txt_redeem_ac.text = string.Format("ac: +{0}", bal.getActiveCoinBonus() * player.Incre.coin.boosterRate);
            txt_redeem_exp.text = string.Format("exp: +{0}", bal.getActiveEXPRate() * player.Incre.exp.boosterRate);
        }
    }


    void updateModeButton()
    {
        ColorBlock cb = passiveActiveButton.colors;
        Color c;
        if (player.Incre.passive)
        {
            c = new Color32(16, 203, 255, 255);
            cb.normalColor = c;
            cb.highlightedColor = c;
            passiveActiveButton.colors = cb;
        }
        else 
        {
            c = new Color32(255, 216, 0, 255);
            cb.normalColor = c;
            cb.highlightedColor = c;
            passiveActiveButton.colors = cb;
        }
    }
    
    //returns true if it is full(100%)
    void updateProgressBar()
    {
        //check over Progress
        double gainedExp = 0;
        if(player.Incre.debugging)
        {
            while (player.Incre.progress.cur > bal.getMaxProgress())
            {
                if (player.Incre.passive)
                {
                    earnPassiveCoin(bal.getPassiveCoinBonus());
                    earnExp(bal.getPassiveEXPRate());
                    player.Incre.progress.cur -= bal.getMaxProgress();
                    gainedExp += bal.getPassiveEXPRate();           //debug
                }
                else  //---------------below is ONLY for debuging purpose
                {
                    earnActiveCoin(bal.getActiveCoinBonus());
                    earnPassiveCoin(bal.getPassiveCoinBonus());
                    earnExp(bal.getActiveEXPRate());
                    player.Incre.progress.cur -= bal.getMaxProgress();
                    gainedExp += bal.getActiveEXPRate();           //debug
                }
            }
            player.Incre.debugging = false;
        }
        //change color depends on mode
        if (player.Incre.passive)
        {
            Color32 c = new Color32(16, 203, 255, 255);
            progressBarImage.color = c;
        }
        else
        {
            Color32 c = new Color32(255, 216, 0, 255);
            progressBarImage.color = c;
        }

        //gets 100% ---> redeem coins and exp
        if (player.Incre.progress.cur >= bal.getMaxProgress())
        {
            //Active MODE
            if(!player.Incre.passive)   //active gets both coins
            {
                if (isPlayerActive())
                {
                    //redeem both
                    earnPassiveCoin(bal.getPassiveCoinBonus()*(int)player.Incre.coin.boosterRate);
                    earnActiveCoin(bal.getActiveCoinBonus() * (int)player.Incre.coin.boosterRate);
                    earnExp(bal.getActiveEXPRate()*player.Incre.exp.boosterRate);
                    player.Incre.progress.cur = 0;
                }
            }
            else
            //Passive MODE
            {
                earnPassiveCoin(bal.getPassiveCoinBonus()* (int)player.Incre.coin.boosterRate);
                earnExp(bal.getPassiveEXPRate() * player.Incre.exp.boosterRate);
                //set to zero
                player.Incre.progress.cur = 0;
            }
        }
        //not 100%
        else
        {
            if (player.Incre.passive == true)
            {
                txt_mode.text = "Passive MODE";
                player.Incre.progress.cur += bal.getPassiveProgressBarRate()*player.Incre.progress.boosterRate;
            }
            else
            {
                txt_mode.text = "Active MODE";
                player.Incre.progress.cur += bal.getActiveProgressBarRate() * player.Incre.progress.boosterRate;
            }
            bar_progress.value = (float)player.Incre.progress.cur / bal.getMaxProgress();
            double progressRate;
            if (player.Incre.passive)
                progressRate = bal.getPassiveProgressBarRate() * player.Incre.progress.boosterRate;
            else
                progressRate = bal.getActiveProgressBarRate() * player.Incre.progress.boosterRate;

            if (frameCount >= 10)
            {
                txt_progress.text = string.Format("Prog {0}/100(+{1})", (player.Incre.progress.cur / bal.getMaxProgress() * 100).ToString("N2")
            , (progressRate / bal.getMaxProgress() * 10000).ToString("N3"));
                frameCount = 0;
            }
            else
                frameCount++;
            
        }
    }
    string changeToTime(float sec)
    {
        TimeSpan time = TimeSpan.FromSeconds(sec);
        return time.ToString();
    } 

    void updateStamina()
    {
        //stamina decreases when player enters some area in minigames 
        txt_stamina.text = String.Format("STAMINA: {0}/{1}", player.Incre.stamina.cur, player.Incre.stamina.max);
    }
    void updateTimeBar()
    {
        if (!player.Incre.passive)
        {
            if (timeLeftFrameCount > 60)
            {
                timeLeftFrameCount = 0;
                player.Incre.timeleft.cur--;
                if (player.Incre.timeleft.cur <= 0)
                {
                    player.Incre.gameON = false;
                    player.Incre.passive = true;
                    player.Incre.nextGame = minigame.none;
                    SceneManager.LoadScene("ui", LoadSceneMode.Single);
                }
            }
        }
        bar_timeLeft.value = (float)(player.Incre.timeleft.cur / player.Incre.timeleft.max);
        txt_timeLeft.text = changeToTime((float)player.Incre.timeleft.cur);
    }


    void updateCoin()
    {
        txt_passiveCoin.text = player.Incre.coin.passive.ToString();
        txt_activeCoin.text = player.Incre.coin.active.ToString();
    }
    void updateLv()
    {
        txt_level.text = player.Incre.lv.ToString();
    }
    void updateExpBar()
    {
        bar_exp.value = (float)(player.Incre.exp.cur / bal.getMaxEXP());
        txt_exp.text = string.Format("EXP {0}/{1}", (int)player.Incre.exp.cur, (int)bal.getMaxEXP());
        if (player.Incre.exp.cur >= bal.getMaxEXP())
        {
            double remain = player.Incre.exp.cur - bal.getMaxEXP();
            levelUP();
            updateLv();
            player.Incre.exp.cur = (float)remain;
        }
    }

    public void earnPassiveCoin(int amount)
    {
        sendMsg("You earned " + amount + " PC!");
        player.Incre.coin.passive += amount;
    }

    public void earnActiveCoin(int amount)
    {
        sendMsg("You earned " + amount + " AC!");
        player.Incre.coin.active += amount;
    }

    public void earnExp(double amount)
    {
        sendMsg("You earned " + amount + " EXP!");
        player.Incre.exp.cur += (float)amount;
    }

    public void levelUP()
    {
        //visual effect will call in this function
        sendMsg("Level up!!!");
        player.Incre.lv++;

        //calculate new incremental values here
        //calculateIncremental();
        
    }

    public void upgradeButtonPressed()
    {
        upgradeWindow = !upgradeWindow;
        upgrade.SetActive(upgradeWindow);
        //upgrade.SendMessage("updateTitle");
        //upgrade.SendMessage("updatePrice");
    }
    public void bonusButtonPressed()
    {
        bonusWindow = !bonusWindow;
        bonus.SetActive(bonusWindow);
    }

    public void changeMODE()
    {
        if(!player.Incre.gameON)
        {
            showMessage("Minigame is not running. You can't change it to active mode!", "Minigame Not Running");
            player.Incre.passive = true;
            return;
        }
        //have enough time and stemina
        if(player.Incre.timeleft.cur > 0 && player.Incre.stamina.cur > 0)
        {
            player.Incre.progress.cur = 0; //set to zero when switching between active and passive mode
            player.Incre.passive = !player.Incre.passive;
        }
        else if(player.Incre.timeleft.cur == 0)
        {
            showMessage("You don't have enough 'Time Left'!", "Error!");
        }
        updateModeButton();
    }

    
    public void minigameStart(int whichGame)
    {
        Debug.Log(player.Incre.currentGame.ToString());
        string sceneName = "";
        minigame selectedGame = minigame.none;
        switch(whichGame)
        {
            //seeker
            case 1:
                sceneName = "Mode_Choose";
                selectedGame = minigame.seeker;
                break;

            //mastermind
            case 2:
                sceneName = "Mastermind_Menu";
                selectedGame = minigame.mastermind;
                break;

            //conquere
            case 3:
                sceneName = "Menu";
                selectedGame = minigame.conquer;
                break;
        }
        
        //if game is not on
        if(player.Incre.gameON == false && player.Incre.timeleft.cur > 0)
        {
            player.Incre.gameON = true;
            player.Incre.currentGame = selectedGame;
            Debug.Log(player.Incre.currentGame.ToString());
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else if(player.Incre.gameON == true && player.Incre.currentGame != selectedGame) //game is on
        {
            nextScene = sceneName;
            player.Incre.nextGame = selectedGame;
            showMessage2("If you change the minigame, you will lose the data. Continue to change the game?", "Warning!", dialogMode.changeGame);
        }
        else if (player.Incre.gameON == false && player.Incre.timeleft.cur <= 0)
        {
            showMessage("Not enough Time to play game", "You need to study to play more.");
        }
        else
        {
            nextScene = "ui";
            player.Incre.nextGame = minigame.none;
            showMessage2("Do you want to close the minigame?", "Are you sure?", dialogMode.exitgame);
        }
    }
    public void changeScenes(string newScene)
    {
        SceneManager.LoadScene(newScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void closeMessageBox()
    {
        dialog.active = false;
    }

    private bool isPlayerActive()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("pressed");
            return true;
        }
        if(Input.GetAxis("Mouse X") != 0)
        {
            return true;
        }
        if(Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        if(Input.GetKeyDown("up") || Input.GetKeyDown("down") || Input.GetKeyDown("right") || Input.GetKeyDown("left") ||
            Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d") || Input.GetKeyDown("w"))
        {
            return true;
        }
        if(player.Incre.debugging)
        {
            return true;
        }
        return false;
    }
    
    public void onClickGetBonus()
    {
        strDele fun = new strDele(showMessage);
        bonusCode.analyzeCode(bonusCodeInput.text, fun);
    }
    
    public void resetTime()
    {
        player.Incre.stamina.cur = 10;
        player.Incre.stamina.cur = 10;
    }
    public void debug_lvUp()
    {
        player.Incre.lv++;
    }
    public void debug_lvDown()
    {
        player.Incre.lv--;
    }
    public void debug_noTime()
    {
        player.Incre.timeleft.cur -= 60*5;
    }
    public void debug_yesTime()
    {
        player.Incre.timeleft.cur += 60 * 5;
    }

    public void debug_init()
    {
        loginScript.createNewData();
        player.Incre.needTutorial = false;
    }
    public void debug_addDay(int i)
    {
        loginScript.updatePassiveProgress(DateTime.Now.AddDays(-i));
        player.Incre.debugging = true;
        player.Incre.passivePlayingTimeLeft -= 7;
        Debug.Log("remain passive days = " + player.Incre.passivePlayingTimeLeft);
    }
    public void debug_playHour(int i)
    {
        loginScript.updateActiveProgress(DateTime.Now.AddHours(-i));
        player.Incre.debugging = true;
        Debug.Log("remain active playing time = " + player.Incre.activePlayingTimeLeft--);
    }
    public void debug_gameON()
    {
        player.Incre.gameON = true;
    }

    public void debug_save()
    {
        player.JsonStr = player.getJsonStr(game.incremental);
        //Debug.Log(NetworkManager.Instance.);

        NetworkManager.Instance.QueueMessage(new List<string>() { "Incremental", player.JsonStr});
        
    }
    public void debug_load()
    {
        

        if (player.JsonStr != "")
            player.load(player.JsonStr, game.incremental);
    }
}

public enum dialogMode
{
    exitgame,
    changeGame,
    exitapplication,
    resetGame
}

