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
        return (30+ player.Incre.exp.A_upgradeLv*10)* player.Incre.permanentPerksLV;
    }
    public static double getPassiveEXPRate()
    {
        return (2.5+ player.Incre.exp.P_upgradeLv*5) * player.Incre.permanentPerksLV;
    }
    public static int getMaxEXP()
    {
        return (int)(expectedPlayingTime(player.Incre.lv) * 100);
    }


    //coins
    public static int getActiveCoinBonus()
    {
        return (player.Incre.coin.A_upgradeLv*5 + player.Incre.lv * 2) * player.Incre.permanentPerksLV;
    }
    public static int getPassiveCoinBonus()
    {
        return (player.Incre.coin.A_upgradeLv * 2 + player.Incre.lv * 2) * player.Incre.permanentPerksLV;
    }

    //progress
    public static int getMaxProgress()
    {
        return 600;  //means 60 seconds = 1 min
    }
    public static float getActiveProgressBarRate()
    {
        return (0.04f + player.Incre.progress.A_upgradeLv*0.0050f) * player.Incre.permanentPerksLV;
    }
    public static float getPassiveProgressBarRate()
    {
        return (0.002f + player.Incre.progress.P_upgradeLv*0.0002f) * player.Incre.permanentPerksLV;
    }





    //price

    public static int getPrice_ActiveCoinBonus()
    {
        return (player.Incre.coin.A_upgradeLv * 9) * player.Incre.permanentPerksLV;
    }
    public static int getPrice_ActiveProgressBarRate()
    {
        return (player.Incre.progress.A_upgradeLv * 15) * player.Incre.permanentPerksLV;
    }
    public static int getPrice_ActiveExpBonus()
    {
        return (player.Incre.exp.A_upgradeLv*10) * player.Incre.permanentPerksLV;
    }

    public static int getPrice_PassiveProgressBarRate()
    {
        return (player.Incre.progress.P_upgradeLv * 15) * player.Incre.permanentPerksLV;
    }
    public static int getPrice_PassiveCoinBonus()
    {
        return (player.Incre.coin.P_upgradeLv * 5) * player.Incre.permanentPerksLV;
    }
    public static int getPrice_PassiveExpBonus()
    {
        return (player.Incre.exp.P_upgradeLv*20) * player.Incre.permanentPerksLV;
    }
    

}
