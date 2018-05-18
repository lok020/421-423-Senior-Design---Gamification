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
    public Text debugText;
    public GameObject gameEnableDisable;
    public GameObject dialog_yes;
    public GameObject dialog_no;
    public GameObject dialog_okay;

    public Slider bar_progress;
    public Slider bar_exp;
    public Slider bar_stemina;
    public Slider bar_timeLeft;

    
    public Text txt_progress;
    public Text txt_exp;
    public Text txt_welcomeMsg;
    public Text txt_timeLeft;
    public Text txt_stemina;

    public Text txt_redeem_pc;
    public Text txt_redeem_ac;
    public Text txt_redeem_exp;

    public Text txt_mode;
    public Text txt_level;
    public Text txt_passiveCoin;
    public Text txt_activeCoin;
    public Text txt_message;
    public Text txt_messageTitle;
    public Button passiveActiveButton;
    public Image progressBarImage;
    //public Image idle_shade;
    //public Image aidle_shade;

    int steminaFrameCount;
    int timeLeftFrameCount;
    bool gameON;
    bool upgradeWindow;
    bool bonusWindow;
    string curScene;
    string nextScene;
    dialogMode _dialogMode;
    void Start()
    {
        Debug.Log("UI loaded");
        //update players passive progress
        //Debug.Log(DateTime.Now.ToString());
        //Debug.Log(DateTime.Now+100.ToString());

      //set upgrade window
      upgradeWindow = false;
        bonusWindow = false;
        upgrade.SetActive(upgradeWindow);
        bonus.SetActive(bonusWindow);
        
        //msg
        txt_welcomeMsg.text = string.Format("Welcome {0}!", player.username);
        steminaFrameCount = 0;
        timeLeftFrameCount = 0;

        if(!player.gameON)
        {
            player.passive = true;
        }

        // Vector3 newpos = new Vector3(0, 0, 0);
        //upgrade.transform.position = newpos;
        //Screen.SetResolution(1325, 895, false);
    }
    
	// Update is called once per frame
	void Update ()
    {
        debug();
        gameEnableDisableCheck();
        updateProgressBar();
        updateCoin();
        updateLv();
        updateExpBar();
        updateModeButton();
        updateRedeemText();
        updateTimeBar();
        updateSteminaBar();
        steminaFrameCount++;
        timeLeftFrameCount++;
    }


    void gameEnableDisableCheck()
    {
        if(player.gameON == true)
        {
            gameEnableDisable.active = false;
        }
        else
        {
            gameEnableDisable.active = true;
        }
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
        showMessage2("Do you want to close the whole game?", "Sure?", dialogMode.exitapplication);
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
        }
        else
        {
            
        }
        dialog.active = false;
    }

    //----------------------------------------------------------------------//
    void debug()
    {
        string progress = string.Format("progress: {0}/{1}", player.progress_cur, incremental.getMaxProgress());
        string exp = string.Format("exp: {0}/{1}", player.exp_cur, incremental.getMaxEXp());
        string money = string.Format("active/passive: {0}/{1}", player.coin_active, player.coin_passive);
        string stemina = string.Format("stemina: {0}/{1}", player.stemina_cur, incremental.getMaxStemina());
        string level = string.Format("lev: {0}", player.level);
        debugText.text = progress + "\r\n" + exp + "\r\n" +
            money + "\r\n" +
            stemina + "\r\n" +
            level + "\r\n";

        debugText.text += "scene_conquer: " + MySceneManager.scene_conquer + "\r\n";
        debugText.text += "scene_mastermind: " + MySceneManager.scene_mastermind + "\r\n";
        debugText.text += "scene_seeker: " + MySceneManager.scene_seeker + "\r\n";
        debugText.text += "activeGame: " + MySceneManager.activeGame.ToString() + "\r\n";

    }

    void updateRedeemText()
    {
        //passive mode
        if(player.passive)
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", incremental.getPassiveCoinBonus());
            txt_redeem_ac.text = string.Format("ac: +{0}", 0);
            txt_redeem_exp.text = string.Format("exp: +{0}({1}%)", incremental.getPassiveEXPRate(),
                incremental.getPassiveEXPRate()/incremental.getMaxEXp()*100);
        }
        else
        {
            txt_redeem_pc.text = string.Format("pc: +{0}", incremental.getPassiveCoinBonus());
            txt_redeem_ac.text = string.Format("ac: +{0}", incremental.getActiveCoinBonus());
            txt_redeem_exp.text = string.Format("exp: +{0}({1}%)", incremental.getActiveEXPRate(),
                 incremental.getActiveEXPRate() / incremental.getMaxEXp() * 100);
        }
    }


    void updateModeButton()
    {
        ColorBlock cb = passiveActiveButton.colors;
        Color c;
        if (player.passive)
        {
            c = new Color32(24, 159, 255, 255);
            cb.normalColor = c;
            cb.highlightedColor = c;
            passiveActiveButton.colors = cb;
        }
        else
        {
            c = new Color32(255, 88, 88, 255);
            cb.normalColor = c;
            cb.highlightedColor = c;
            passiveActiveButton.colors = cb;
        }
    }
    
    //returns true if it is full(100%)
    void updateProgressBar()
    {
        //change color
        if (player.passive)
        {
            Color32 c = new Color32(24, 159, 255, 255);
            progressBarImage.color = c;
        }
        else
        {
            Color32 c = new Color32(255, 88, 88, 255);
            progressBarImage.color = c;
        }


        //gets 100% ---> redeem coins and exp
        if (player.progress_cur >= incremental.getMaxProgress())
        {
            //Active MODE
            if(!player.passive)   //active gets both coins
            {
                if (isPlayerActive())
                {
                    //redeem both
                    earnPassiveCoin(incremental.getPassiveCoinBonus());
                    earnActiveCoin(incremental.getActiveCoinBonus());
                    earnExp(incremental.getActiveEXPRate());
                    player.progress_cur = 0;
                    //Debug.Log("pressed");
                }
            }
            else
            //Passive MODE
            {
                earnPassiveCoin(incremental.getPassiveCoinBonus());
                earnExp(incremental.getPassiveEXPRate());
                //set to zero
                player.progress_cur = 0;
            }
        }
        //not 100%
        else
        {
            if (player.passive == true)
            {
                //update idle progress bar
                txt_mode.text = "Passive MODE";
                player.progress_cur += incremental.getPassiveProgressBarRate();
            }
            else
            {
                //update anti idle progress bar
                txt_mode.text = "Active MODE";
                player.progress_cur += incremental.getActiveProgressBarRate();
            }
            bar_progress.value = player.progress_cur / incremental.getMaxProgress();

            updateProgNum(txt_progress, (int)player.progress_cur, incremental.getMaxProgress());
        }
    }

    void updateProgNum(Text txt, int cur, int max)
    {
        txt.text = string.Format("{0}/{1}", cur, max);
    }

    string changeToTime(float sec)
    {
        TimeSpan time = TimeSpan.FromSeconds(sec);
        return time.ToString().Substring(3);
    } 

    void updateSteminaBar()
    {
        //stemina decreases if and only if minigame is on and player is in active
        if(player.gameON && !player.passive)
        {
            if(steminaFrameCount > 60)
            {
                steminaFrameCount = 0;
                player.stemina_cur--;
                if(player.stemina_cur <= 0)
                {
                    player.gameON = false;
                    // pops up message
                    // exit game and do your homework!
                }
            }
        }
        bar_stemina.value = player.stemina_cur / incremental.getMaxStemina();
        txt_stemina.text = changeToTime(player.stemina_cur);
    }
    void updateTimeBar()
    {
        if (player.gameON)
        {
            if (timeLeftFrameCount > 60)
            {
                timeLeftFrameCount = 0;
                player.time_left--;
                if (player.time_left <= 0)
                {
                    showMessage("You cannot play any more. You don't have enough 'Time Left'. Please attend class to get more time.\r\nThe game mode has been changed to 'Passive Mode'"
                        , "Not enough Time Left!");
                    player.gameON = false;
                    player.passive = true;
                    // pops up message
                    // exit game and do your homework!
                }
            }
        }
        bar_timeLeft.value = player.time_left / incremental.getMaxTimeLeft();
        txt_timeLeft.text = changeToTime(player.time_left);
    }


    void updateCoin()
    {
        txt_passiveCoin.text = string.Format("PC: {0}", player.coin_passive);
        txt_activeCoin.text = string.Format("AC: {0}", player.coin_active);
    }
    void updateLv()
    {
        txt_level.text = player.level.ToString();
    }
    void updateExpBar()
    {
        bar_exp.value = player.exp_cur / incremental.getMaxEXp();
        updateProgNum(txt_exp, (int)player.exp_cur, incremental.getMaxEXp());
    }

    public void earnPassiveCoin(int amount)
    {
        player.coin_passive += amount;
    }

    public void earnActiveCoin(int amount)
    {
        player.coin_active += amount;
    }

    public void earnExp(double amount)
    {
        player.exp_cur += (float)amount;
        if(player.exp_cur >= incremental.getMaxEXp())
        {
            double remain = player.exp_cur - incremental.getMaxEXp();
            levelUP();
            updateLv();
            player.exp_cur = (float)remain;
        }
    }

    public void levelUP()
    {
        //visual effect will call in this function
        player.level++;

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
        if(player.time_left > 0 && player.stemina_cur > 0)
        {
            player.passive = !player.passive;
        }
        else if(player.time_left == 0)
        {
            showMessage("You don't have enough 'Time Left'!", "Error!");
        }
        else if(player.stemina_cur == 0)
        {
            player.passive = true;
            showMessage("You don't have enough 'Stamina'. Only Passive Mode can be ON", "Not Enough Stamina");
        }
        updateModeButton();
    }
    

    //when mini game tab is selected, stemina will start decreasing 
   
    public void achievement()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("achievement", UnityEngine.SceneManagement.LoadSceneMode.Additive);
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
        if(player.gameON == false && player.time_left > 0)
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
        else
        {
            nextScene = "ui";
            player.nextGame = minigame.none;
            showMessage2("Do you want to close the minigame?", "Are you sure?" ,dialogMode.exitgame);
        }
        
    }

    public void changeScenes(string newScene)
    {
        SceneManager.LoadScene(newScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void upgradeBar()
    {
        
    }
    public void updateUpgradeTxt()
    {
        
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
    
    public void resetTime()
    {
        player.stemina_cur = 1000;
        player.time_left = 1000;
    }

 
}

public enum dialogMode
{
    exitgame,
    changeGame,
    exitapplication
}