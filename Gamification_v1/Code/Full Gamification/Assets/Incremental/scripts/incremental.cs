using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class incremental{

    

    //===============INCREMENTAL PART =====================//
    //These function will calculate the output based on 
    //user's level
    public static int upgradeMaxLevel = 20;

    //exps
    public static float getActiveEXPRate()
    {
        return getMaxEXp()*0.05f + getPrice_ActiveExpBonus() * 0.1f;
        //return getMaxEXp()*0.06f * (1 + player.active_expBonusLV * 0.05f);
    }
    public static float getPassiveEXPRate()
    {
        return getMaxEXp() * 0.01f + getPrice_PassiveExpBonus() * 0.1f;
        //return getMaxEXp()*0.03f * (1 + player.passive_expBonusLV * 0.01f);
    }
    public static int getMaxEXp()
    {
        return player.level * 100 + 2000;
    }
    public static int getPrice_PassiveExpBonus()
    {
        return (int)(power(1 + player.passive_expBonusLV * 0.8, 2) / 10);
    }
    public static int getPrice_ActiveExpBonus()
    {
        return (int)(power(1 + player.active_expBonusLV * 0.8, 2) / 10);
    }

    //coins
    public static int getActiveCoinBonus()
    {
        return (int)(getPrice_ActiveCoinBonus() * 0.1)+player.level;
        //return player.active_CoinBonusLV * 3 + player.level * 100;
    }
    public static int getPassiveCoinBonus()
    {
        return (int)(getPrice_PassiveCoinBonus() * 0.1) + player.level;
        //return player.passive_CoinBonusLV * 3 + player.level * 10;
    }
    public static int getPrice_PassiveCoinBonus()
    {
        return (int)(power(1 + player.passive_CoinBonusLV * 0.8, 2) / 10);
    }
    public static int getPrice_ActiveCoinBonus()
    {
        return (int)(power(1 + player.active_CoinBonusLV * 0.8, 2) / 10);
    }

    //progress
    public static int getMaxProgress()
    {
        return 2000;
    }
    public static float getActiveProgressBarRate()
    {
        return 0.1f + (player.active_progressBarRateLV * 0.2f);
    }
    public static float getPassiveProgressBarRate()
    {
        return 0.05f + (player.passive_progressBarRateLV * 0.2f);
    }
    public static int getPrice_ActiveProgressBarRate()
    {
        return (int)(power(1+player.active_progressBarRateLV*0.8, 2)/10);
    }
    public static int getPrice_PassiveProgressBarRate()
    {
        return (int)(power(1 + player.passive_progressBarRateLV * 0.8, 2) / 10);
    }

    //Extra functionss
    private static double power(double i, int n)
    {
        if(n > 0)
        {
            return power(i * i, n - 1);
        }
        return i;
    }

    public static int getMaxStemina()
    {
        return 1000;
    }
    public static int getMaxTimeLeft()
    {
        return 1000;
    }
}
