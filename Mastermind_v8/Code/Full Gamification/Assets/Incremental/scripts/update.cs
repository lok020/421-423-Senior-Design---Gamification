using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.SceneManagement;
using System;

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

    public Button passiveActiveButton;
    public Image progressBarImage;
    public Text msglog;
    

    string nextScene;
    int timeLeftFrameCount;
    int frameCount;
    bool upgradeWindow;
    bool bonusWindow;
    dialogMode _dialogMode;
    void Start()
    {
      //set upgrade window
        upgradeWindow = false;
        bonusWindow = false;
        upgrade.SetActive(upgradeWindow);
        bonus.SetActive(bonusWindow);
        //msg
        txt_welcomeMsg.text = string.Format("Welcome {0}!", player.username);
        timeLeftFrameCount = 0;
        frameCount = 100;
        if(!player.gameON)
        {
            player.passive = true;
        }
        else
        {
            player.passive = false;
        }
        if(player.timeleft.cur <= 0)
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
        mainScreen.SetActive(!player.gameON);
        if (bar_progress.value <= 0.021f)
        {
            bar_progress.value = 0.021f;
        }
        if(bar_exp.value <= 0.021f)
        {
            bar_exp.value = 0.021f;
        }
        if (player.gameON)
            txt_exit.text = "Close Game";
        else
            txt_exit.text = "Exit Game";
    }

    public void sendMsg(string msg)
    {
        msglog.text += "\n>" + msg;
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
        if(player.gameON)
        {
            nextScene = "ui";
            player.nextGame = minigame.none;
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
                player.currentGame = player.nextGame;
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            }
            if(_dialogMode == dialogMode.exitgame)
            {
                player.currentGame = minigame.none;
                player.gameON = false;
                player.passive = true;
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



    void updateRedeemText()
    {
        //passive mode
        if(player.passive)
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", bal.getPassiveCoinBonus()*player.coin.boosterRate);
            txt_redeem_ac.text = string.Format("ac: +{0}", 0);
            txt_redeem_exp.text = string.Format("exp: +{0}", bal.getPassiveEXPRate() * player.exp.boosterRate);
        }
        else
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", bal.getPassiveCoinBonus() * player.coin.boosterRate);
            txt_redeem_ac.text = string.Format("ac: +{0}", bal.getActiveCoinBonus() * player.coin.boosterRate);
            txt_redeem_exp.text = string.Format("exp: +{0}", bal.getActiveEXPRate() * player.exp.boosterRate);
        }
    }


    void updateModeButton()
    {
        ColorBlock cb = passiveActiveButton.colors;
        Color c;
        if (player.passive)
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
        bool debuging = false;
        while(player.progress.cur > bal.getMaxProgress())
        {
            debuging = true;
            if(player.debugPassive)
            {
                earnPassiveCoin(bal.getPassiveCoinBonus());
                earnExp(bal.getPassiveEXPRate());
                player.progress.cur -= bal.getMaxProgress();
                gainedExp += bal.getPassiveEXPRate();           //debug
            }
            else
            {
                earnActiveCoin(bal.getActiveCoinBonus());
                earnPassiveCoin(bal.getPassiveCoinBonus());
                earnExp(bal.getActiveEXPRate());
                player.progress.cur -= bal.getMaxProgress();
                gainedExp += bal.getActiveEXPRate();           //debug
            }
        }
        if(debuging)
        {
            Debug.Log(gainedExp);
            sendMsg("You gained " + gainedExp + " EXP.");
        }

        //change color depends on mode
        if (player.passive)
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
        if (player.progress.cur >= bal.getMaxProgress())
        {
            //Active MODE
            if(!player.passive)   //active gets both coins
            {
                if (isPlayerActive())
                {
                    //redeem both
                    earnPassiveCoin(bal.getPassiveCoinBonus()*(int)player.coin.boosterRate);
                    earnActiveCoin(bal.getActiveCoinBonus() * (int)player.coin.boosterRate);
                    earnExp(bal.getActiveEXPRate()*player.exp.boosterRate);
                    player.progress.cur = 0;
                }
            }
            else
            //Passive MODE
            {
                earnPassiveCoin(bal.getPassiveCoinBonus()* (int)player.coin.boosterRate);
                earnExp(bal.getPassiveEXPRate() * player.exp.boosterRate);
                //set to zero
                player.progress.cur = 0;
            }
        }
        //not 100%
        else
        {
            if (player.passive == true)
            {
                txt_mode.text = "Passive MODE";
                player.progress.cur += bal.getPassiveProgressBarRate()*player.progress.boosterRate;
            }
            else
            {
                txt_mode.text = "Active MODE";
                player.progress.cur += bal.getActiveProgressBarRate() * player.progress.boosterRate;
            }
            bar_progress.value = (float)player.progress.cur / bal.getMaxProgress();
            double progressRate;
            if (player.passive)
                progressRate = bal.getPassiveProgressBarRate() * player.progress.boosterRate;
            else
                progressRate = bal.getActiveProgressBarRate() * player.progress.boosterRate;

            if (frameCount >= 10)
            {
                txt_progress.text = string.Format("{0}/100(+{1})", (player.progress.cur / bal.getMaxProgress() * 100).ToString("N2")
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
        return time.ToString().Substring(3);
    } 

    void updateStamina()
    {
        //stamina decreases when player enters some area in minigames 
        txt_stamina.text = String.Format("STAMINA: {0}/{1}", player.stamina.cur, player.stamina.max);
    }
    void updateTimeBar()
    {
        if (!player.passive)
        {
            if (timeLeftFrameCount > 60)
            {
                timeLeftFrameCount = 0;
                player.timeleft.cur--;
                if (player.timeleft.cur <= 0)
                {
                    player.gameON = false;
                    player.passive = true;
                    player.nextGame = minigame.none;
                    SceneManager.LoadScene("ui", LoadSceneMode.Single);
                }
            }
        }
        bar_timeLeft.value = (float)(player.timeleft.cur / player.timeleft.max);
        txt_timeLeft.text = changeToTime((float)player.timeleft.cur);
    }


    void updateCoin()
    {
        txt_passiveCoin.text = player.coin.passive.ToString();
        txt_activeCoin.text = player.coin.active.ToString();
    }
    void updateLv()
    {
        txt_level.text = player.lv.ToString();
    }
    void updateExpBar()
    {
        bar_exp.value = (float)(player.exp.cur / bal.getMaxEXP());
        txt_exp.text = string.Format("{0}/{1}", (int)player.exp.cur, (int)bal.getMaxEXP());
        if (player.exp.cur >= bal.getMaxEXP())
        {
            double remain = player.exp.cur - bal.getMaxEXP();
            levelUP();
            updateLv();
            player.exp.cur = (float)remain;
        }
    }

    public void earnPassiveCoin(int amount)
    {
        player.coin.passive += amount;
        
    }

    public void earnActiveCoin(int amount)
    {
        player.coin.active += amount;
    }

    public void earnExp(double amount)
    {
        player.exp.cur += (float)amount;
    }

    public void levelUP()
    {
        //visual effect will call in this function
        player.lv++;

        //calculate new incremental values here
        //calculateIncremental();
        
    }

    public void upgradeButtonPressed()
    {
        upgradeWindow = !upgradeWindow;
        upgrade.SetActive(upgradeWindow);
    }
    public void bonusButtonPressed()
    {
        bonusWindow = !bonusWindow;
        bonus.SetActive(bonusWindow);
    }

    public void changeMODE()
    {
        if(!player.gameON)
        {
            showMessage("Minigame is not running. You can't change it to active mode!", "Minigame Not Running");
            player.passive = true;
            return;
        }
        //have enough time and stemina
        if(player.timeleft.cur > 0 && player.stamina.cur > 0)
        {
            player.progress.cur = 0; //set to zero when switching between active and passive mode
            player.passive = !player.passive;
        }
        else if(player.timeleft.cur == 0)
        {
            showMessage("You don't have enough 'Time Left'!", "Error!");
        }
        updateModeButton();
    }

    
    public void minigameStart(int whichGame)
    {
        Debug.Log(player.currentGame.ToString());
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
        if(player.gameON == false && player.timeleft.cur > 0)
        {
            player.gameON = true;
            player.currentGame = selectedGame;
            Debug.Log(player.currentGame.ToString());
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else if(player.gameON == true && player.currentGame != selectedGame) //game is on
        {
            nextScene = sceneName;
            player.nextGame = selectedGame;
            showMessage2("If you change the minigame, you will lose the data. Continue to change the game?", "Warning!", dialogMode.changeGame);
        }
        else if (player.gameON == false && player.timeleft.cur <= 0)
        {
            showMessage("Not enough Time to play game", "You need to study to play more.");
        }
        else
        {
            nextScene = "ui";
            player.nextGame = minigame.none;
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
        return false;
    }
    
    public void onClickGetBonus()
    {
        bonusCode.analyzeCode(bonusCodeInput.text);
    }












    public void resetTime()
    {
        player.stamina.cur = 10;
        player.stamina.cur = 10;
    }
    public void debug_lvUp()
    {
        player.lv++;
    }
    public void debug_lvDown()
    {
        player.lv--;
    }
    public void debug_noTime()
    {
        player.timeleft.cur -= 60*5;
    }
    public void debug_yesTime()
    {
        player.timeleft.cur += 60 * 5;
    }

    public void debug_init()
    {
        player.needTutorial = true;
    }
    public void debug_addDay(int i)
    {
        loginScript.updatePassiveProgress(DateTime.Now.AddDays(-i));
        player.debugPassive = true;
        player.passivePlayingTimeLeft -= 7;
        Debug.Log("remain passive days = " + player.passivePlayingTimeLeft);
    }
    public void debug_playHour(int i)
    {
        loginScript.updateActiveProgress(DateTime.Now.AddHours(-i));
        player.debugPassive = false;
        Debug.Log("remain active playing time = " + player.activePlayingTimeLeft--);
    }
    public void debug_gameON()
    {
        player.gameON = true;
    }
}

public enum dialogMode
{
    exitgame,
    changeGame,
    exitapplication,
    resetGame
}

