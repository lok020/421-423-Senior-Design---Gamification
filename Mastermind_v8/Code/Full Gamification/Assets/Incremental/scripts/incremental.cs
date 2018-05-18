using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//balance class
public static class bal{
    //level range: 1~53
    public static double expectedPlayingTime(int level)
    {
        return Math.Pow(1.06, level) * 3;
    }

    //exps
    public static double getActiveEXPRate()
    {
        return 50+player.exp.A_upgradeLv*10;
    }
    public static double getPassiveEXPRate()
    {
        return 2.5+player.exp.P_upgradeLv*5;
    }
    public static int getMaxEXP()
    {
        return (int)(expectedPlayingTime(player.lv) * 100);
    }


    //coins
    public static int getActiveCoinBonus()
    {
        return player.coin.A_upgradeLv*5 + player.lv * 2;
    }
    public static int getPassiveCoinBonus()
    {
        return player.coin.A_upgradeLv * 2 + player.lv * 2;
    }

    //progress
    public static int getMaxProgress()
    {
        return 600;  //means 60 seconds = 1 min
    }
    public static float getActiveProgressBarRate()
    {
        return 0.02f + player.progress.A_upgradeLv*0.010f;
    }
    public static float getPassiveProgressBarRate()
    {
        return 0.001f + player.progress.P_upgradeLv*0.0002f;
    }





    //price

    public static int getPrice_ActiveCoinBonus()
    {
        return player.coin.A_upgradeLv * 9;
    }
    public static int getPrice_ActiveProgressBarRate()
    {
        return player.progress.A_upgradeLv * 15;
    }
    public static int getPrice_ActiveExpBonus()
    {
        return player.exp.A_upgradeLv*10;
    }

    public static int getPrice_PassiveProgressBarRate()
    {
        return player.progress.P_upgradeLv * 15;
    }
    public static int getPrice_PassiveCoinBonus()
    {
        return player.coin.P_upgradeLv * 5;
    }
    public static int getPrice_PassiveExpBonus()
    {
        return player.exp.P_upgradeLv*20;
    }
    

}
