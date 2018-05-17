using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class player{
    
    public static int level
    {
        set {PlayerPrefs.SetInt("level", value);}
        get{return PlayerPrefs.GetInt("level", 1);}
    }
    public static string username
    {
        set { PlayerPrefs.SetString("username", value); }
        get { return PlayerPrefs.GetString("username", "default"); }
    }
    public static int coin_passive
    {
        set { PlayerPrefs.SetInt("coin_passive", value); }
        get { return PlayerPrefs.GetInt("coin_passive"); }
    }
    public static int coin_active
    {
        set { PlayerPrefs.SetInt("coin_active", value); }
        get { return PlayerPrefs.GetInt("coin_active"); }
    }

    //info of bars
    public static float progress_cur
    {
        set { PlayerPrefs.SetFloat("progress_cur", value); }
        get { return PlayerPrefs.GetFloat("progress_cur"); }
    }
    public static float exp_cur
    {
        set { PlayerPrefs.SetFloat("exp_cur", value); }
        get { return PlayerPrefs.GetFloat("exp_cur"); }
    }
    public static float stemina_cur
    {
        set { PlayerPrefs.SetFloat("stemina_cur", value); }
        get { return PlayerPrefs.GetFloat("stemina_cur"); }
    }
    public static float time_left
    {
        set { PlayerPrefs.SetFloat("time_left", value); }
        get { return PlayerPrefs.GetFloat("time_left"); }
    }

  
    //upgrade status (passive)
    public static int passive_progressBarRateLV
    {
        set { PlayerPrefs.SetInt("lv_passive_progress", value); }
        get { return PlayerPrefs.GetInt("lv_passive_progress"); }
    }
    public static int passive_CoinBonusLV
    {
        set { PlayerPrefs.SetInt("lv_passive_coinBonus", value); }
        get { return PlayerPrefs.GetInt("lv_passive_coinBonus"); }
    }
    public static int passive_expBonusLV
    {
        set { PlayerPrefs.SetInt("lv_passive_exp", value); }
        get { return PlayerPrefs.GetInt("lv_passive_exp"); }
    }

    //upgrade status (active)
    public static int active_progressBarRateLV
    {
        set { PlayerPrefs.SetInt("lv_active_progress", value); }
        get { return PlayerPrefs.GetInt("lv_active_progress"); }
    }
    public static int active_CoinBonusLV
    {
        set { PlayerPrefs.SetInt("lv_active_coinBonus", value); }
        get { return PlayerPrefs.GetInt("lv_active_coinBonus"); }
    }
    public static int active_expBonusLV
    {
        set { PlayerPrefs.SetInt("lv_active_exp", value); }
        get { return PlayerPrefs.GetInt("lv_active_exp"); }
    }

    public static string last_logout
    {
        set { PlayerPrefs.SetString("last_logout", value); }
        get { return PlayerPrefs.GetString("last_logout"); }
    }

    public static bool passive = false;
    public static bool gameON = false;
    public static minigame currentGame;
    public static minigame nextGame;
}
