using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Newtonsoft.Json;


#region structs
public struct coin
{
    public int A_upgradeLv;
    public int P_upgradeLv;
    public int active;
    public int passive;
    public int numBooster;
    public int boosterRemain;
    public double boosterRate;
}
public struct exp
{
    public int A_upgradeLv;
    public int P_upgradeLv;
    public double cur;
    public int numBooster;
    public int boosterRemain;
    public double boosterRate;
}

public struct prog
{
    public int A_upgradeLv;
    public int P_upgradeLv;
    public double cur;
    public double max;
    public int numBooster;
    public int boosterRemain;
    public double boosterRate;
}

public struct stam
{
    public static double _cur = 15;
    public static double _max = 15;
    public double cur
    {
        get
        {
            return _cur;
        }
        set
        {
            if (value >= _max)
            {
                _cur = _max;
            }
            else
                _cur = value;
        }
    }
    public double max
    {
        get
        {
            return _max;
        }
    }
}

public struct timeLeft
{
    public static double _cur = 60;
    public static double _max = 60 * 60;

    public double cur
    {
        get
        {
            return _cur;
        }
        set
        {
            if (value >= _max)
            {
                _cur = _max;
            }
            else
                _cur = value;
        }
    }
    public double max
    {
        get
        {
            return _max;
        }
    }
}

#endregion
public class IncrementalData
{
    public bool startNew = true;
    public bool needTutorial = true;
    public coin coin = new coin { active = 0, A_upgradeLv = 1, boosterRate = 1, boosterRemain = 0, numBooster = 0, passive = 0, P_upgradeLv = 1 };
    public exp exp = new exp { A_upgradeLv = 1, boosterRate = 1, boosterRemain = 0, numBooster = 0, P_upgradeLv = 1 };
    public prog progress = new prog { A_upgradeLv = 1, boosterRate = 1, boosterRemain = 0, numBooster = 0, P_upgradeLv = 1 };
    public stam stamina;
    public timeLeft timeleft;
    public int lv = 1;
    public string username = "Username";
    public int permanentPerksLV = 1;
    string _DateTimeStr = "";
    public bool startMessageDisplayed = false;
    public DateTime lastLogOut
    {
        get
        {
            string now;
            if (_DateTimeStr == "")
            {
                now = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                return Convert.ToDateTime(now);
            }
            else
            {
                DateTime dt = Convert.ToDateTime(_DateTimeStr);
                return dt;
            }
        }
        set
        {
            string dateTimeStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _DateTimeStr = dateTimeStr;

        }
    }
    public bool passive = false;
    public bool gameON = false;
    public minigame currentGame;
    public minigame nextGame;
    public bool debugging = false;
    public int activePlayingTimeLeft = 16; //16 hours
    public int passivePlayingTimeLeft = 16 * 7; //16weeks
    public string titleString = "";
    public string titleCollectionStr = "";
    public bool[] titleCollection = new bool[50];
    public int LogOutpassiveProgressGained = 0;
    public List<string> usedCode = new List<string>(); 
}

public enum game
{
    incremental, seeker, mastermind, conquer
}

public static class player
{
    public static bool isLocal = true;
    public static string localIncreData
    {
        get
        {
            return PlayerPrefs.GetString("localIncreData", JsonConvert.SerializeObject(new IncrementalData()));
        }
        set
        {
            PlayerPrefs.SetString("localIncreData", value);
        }
    }
    public static string localMasterData
    {
        get
        {
            return PlayerPrefs.GetString("localMasterData", JsonConvert.SerializeObject(new sudokuData()));
        }
        set
        {
            PlayerPrefs.SetString("localMasterData", value);
        }
    }
    public static string localSeekerData
    {
        get
        {
            return PlayerPrefs.GetString("localSeekerData", JsonConvert.SerializeObject(new GlobalControl()));
        }
        set
        {
            PlayerPrefs.SetString("localSeekerData", value);
        }
    }
    public static string localConquerData
    {
        get
        {
            return PlayerPrefs.GetString("localConquerData", "");
        }
        set
        {
            PlayerPrefs.SetString("localConquerData", value);
        }
    }
    
    public static string JsonStr = "";

    public static IncrementalData Incre = new IncrementalData();
    public static sudokuData Sudoku = new sudokuData();
    public static Player Conqueror = new Player();
    public static seeker seekerData = new seeker();

    public static string getJsonStr(game g)
    {
        if (g == game.incremental)
            return JsonConvert.SerializeObject(Incre);
        else if (g == game.mastermind)
            return JsonConvert.SerializeObject(Sudoku);
        else if (g == game.seeker)
            return JsonConvert.SerializeObject(seekerData);
        else if (g == game.conquer)
            return JsonConvert.SerializeObject(Conqueror);
        return "";
    }

    public static void localLoad()
    {
        if (!isLocal)
            return;

        try
        {
            Debug.Log(localIncreData);
            Incre = JsonConvert.DeserializeObject<IncrementalData>(localIncreData);
        }
        catch
        {
            Incre = new IncrementalData();
        }
        try
        {
            Sudoku = JsonConvert.DeserializeObject<sudokuData>(localMasterData);
        }
        catch
        {
            Sudoku = new sudokuData();
        }
        try
        {
            GlobalControl.Instance = JsonConvert.DeserializeObject<GlobalControl>(localSeekerData);
        }
        catch
        {
            GlobalControl.Instance = new GlobalControl();
        }
    }

    public static void load()
    {
        JsonStrings loadedjsonStrings = JsonConvert.DeserializeObject<JsonStrings>(NetworkManager.Instance.loaded_json);
        Debug.Log(loadedjsonStrings.Incremental);
        Debug.Log(loadedjsonStrings.Mastermind);
        Debug.Log(loadedjsonStrings.Conqueror);
        Debug.Log(loadedjsonStrings.Seeker);
        if (loadedjsonStrings.Incremental.Length > 0)
        {
            try
            {
                Incre = JsonConvert.DeserializeObject<IncrementalData>(loadedjsonStrings.Incremental);
            }
            catch
            {
                Incre = new IncrementalData();
            }
        }
        if (loadedjsonStrings.Mastermind.Length > 0)
        {
            try
            {
                Sudoku = JsonConvert.DeserializeObject<sudokuData>(loadedjsonStrings.Mastermind);
            }
            catch
            {
                Sudoku = new sudokuData();
            }
        }
        if (loadedjsonStrings.Conqueror.Length > 0)
        {
            try
            {
                Conqueror = JsonConvert.DeserializeObject<Player>(loadedjsonStrings.Conqueror);
            }
            catch
            {
                Conqueror = new Player();
            }
        }
        if (loadedjsonStrings.Seeker.Length > 0)
        {
            try
            {
                seekerData = JsonConvert.DeserializeObject<seeker>(loadedjsonStrings.Seeker);
            }
            catch
            {
                seekerData = new seeker();
            }
        }
    }


}
