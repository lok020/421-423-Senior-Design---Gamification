using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public struct coin
{
    public int A_upgradeLv
    {
        get{return PlayerPrefs.GetInt("incremental1", 1);}
        set{PlayerPrefs.SetInt("incremental1", value);}
    }
    public int P_upgradeLv
    {
        get { return PlayerPrefs.GetInt("incremental2", 1); }
        set { PlayerPrefs.SetInt("incremental2", value); }
    }
    public int active
    {
        get { return PlayerPrefs.GetInt("incremental3", 0); }
        set { PlayerPrefs.SetInt("incremental3", value); }
    }
    public int passive
    {
        get { return PlayerPrefs.GetInt("incremental4", 0); }
        set { PlayerPrefs.SetInt("incremental4", value); }
    }
    public int numBooster
    {
        get { return PlayerPrefs.GetInt("incremental5", 5); }
        set { PlayerPrefs.SetInt("incremental5", value); }
    }
    public int boosterRemain
    {
        get { return PlayerPrefs.GetInt("incremental6", 1); }
        set { PlayerPrefs.SetInt("incremental6", value); }
    }
    public double boosterRate
    {
        get { return (double)PlayerPrefs.GetFloat("incremental7", 1); }
        set { PlayerPrefs.SetFloat("incremental7", (float)value); }
    }
}
public struct exp
{
    public int A_upgradeLv
    {
        get { return PlayerPrefs.GetInt("incremental8", 1); }
        set { PlayerPrefs.SetInt("incremental8", value); }
    }
    public int P_upgradeLv
    {
        get { return PlayerPrefs.GetInt("incremental9", 1); }
        set { PlayerPrefs.SetInt("incremental9", value); }
    }
    public double cur
    {
        get { return PlayerPrefs.GetFloat("incremental10", 1); }
        set { PlayerPrefs.SetFloat("incremental10", (float)value); }
    }
    public int numBooster
    {
        get { return PlayerPrefs.GetInt("incremental11", 1); }
        set { PlayerPrefs.SetInt("incremental11", value); }
    }
    public int boosterRemain
    {
        get { return PlayerPrefs.GetInt("incremental12", 1); }
        set { PlayerPrefs.SetInt("incremental12", value); }
    }
    public double boosterRate
    {
        get { return (double)PlayerPrefs.GetFloat("incremental13", 1); }
        set { PlayerPrefs.SetFloat("incremental13", (float)value); }
    }
   //public double max;
}

public struct prog
{
    public int A_upgradeLv
    {
        get { return PlayerPrefs.GetInt("incremental14", 1); }
        set { PlayerPrefs.SetInt("incremental14", value); }
    }
    public int P_upgradeLv
    {
        get { return PlayerPrefs.GetInt("incremental15", 1); }
        set { PlayerPrefs.SetInt("incremental15", value); }
    }
    public double cur
    {
        get { return (double)PlayerPrefs.GetFloat("incremental16", 1); }
        set { PlayerPrefs.SetFloat("incremental16", (float)value); }
    }
    public double max
    {
        get { return (double)PlayerPrefs.GetFloat("incremental7", 1); }
        set { PlayerPrefs.SetFloat("incremental7", (float)value); }
    }
    public int numBooster
    {
        get { return PlayerPrefs.GetInt("incremental18", 1); }
        set { PlayerPrefs.SetInt("incremental18", value); }
    }
    public int boosterRemain
    {
        get { return PlayerPrefs.GetInt("incremental19", 1); }
        set { PlayerPrefs.SetInt("incremental19", value); }
    }
    public double boosterRate
    {
        get { return (double)PlayerPrefs.GetFloat("incrementa20", 1); }
        set { PlayerPrefs.SetFloat("incrementa20", (float)value); }
    }
}

public struct stam
{
    public double cur
    {
        get { return (double)PlayerPrefs.GetFloat("incrementa21", 1); }
        set { PlayerPrefs.SetFloat("incrementa21", (float)value); }
    }
    public double max
    {
        get { return (double)PlayerPrefs.GetFloat("incrementa22", 1); }
        set { PlayerPrefs.SetFloat("incrementa22", (float)value); }
    }
}

public struct timeLeft
{
    public double cur
    {
        get { return (double)PlayerPrefs.GetFloat("incrementa23", 60*60); }
        set { PlayerPrefs.SetFloat("incrementa23", (float)value); }
    }
    public double max
    {
        get { return (double)PlayerPrefs.GetFloat("incrementa24", 60*60); }
        set { PlayerPrefs.SetFloat("incrementa24", (float)value); }
    }
}



public static class player
{
    public static int localHasData
    {
        get { return PlayerPrefs.GetInt("localHasData", 0); }
        set { PlayerPrefs.SetInt("localHasData", value); }
    }
    public static coin coin;
    public static exp exp;
    public static prog progress;
    public static stam stamina;
    public static timeLeft timeleft;
    public static bool needTutorial
    {
        get { return PlayerPrefsX.GetBool("incrementalTutorial", true); }
        set { PlayerPrefsX.SetBool("incrementalTutorial", value); }
    }
    public static int lv
    {
        get { return PlayerPrefs.GetInt("incremental25", 0); }
        set { PlayerPrefs.SetInt("incremental25", value); }
    }
    public static string username
    {
        get { return PlayerPrefs.GetString("incremental26", "New User"); }
        set { PlayerPrefs.SetString("incremental26", value); }
    }
    public static DateTime lastLogOut
    {
        get
        {
            string dateTimeStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
            DateTime dt = Convert.ToDateTime(PlayerPrefs.GetString("incremental27", dateTimeStr));
            return dt;
        }
        set
        {
            string dateTimeStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
            PlayerPrefs.SetString("incremental27", dateTimeStr);
        }
    }
    public static bool passive = false;
    public static bool gameON = false;
    public static minigame currentGame;
    public static minigame nextGame;
    public static bool debugPassive = true;
    public static int activePlayingTimeLeft = 16; //16 hours
    public static int passivePlayingTimeLeft = 16*7; //16weeks
    public static string titleString
    {
        get { return PlayerPrefs.GetString("incremental28", ""); }
        set { PlayerPrefs.SetString("incremental28", value); }
    }
    public static string titleCollectionStr
    {
        get { return PlayerPrefs.GetString("incremental29", ""); }
        set { PlayerPrefs.SetString("incremental29", value); }
    }
    public static bool[] titleCollection = new bool[50];
}
