using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum upgrademode
{
    passive = 1, active = 2
}

public enum upgrademode2
{
    progressBarRate = 1,
    coinBonus = 2,
    expBonus = 3
}

public enum purchaseBy
{
    passiveCoin = 1, activeCoin = 2
}

public class upgradeScript : MonoBehaviour {
    //text title
    public Text passive_progress;
    public Text passive_exp;
    public Text passive_coin;
    public Text active_progress;
    public Text active_exp;
    public Text active_coin;

    //text coin
    public Text price1;
    public Text price2;
    public Text price3;
    public Text price4;
    public Text price5;
    public Text price6;
    public Text price7;
    public Text price8;
    public Text price9;
    public Text price10;
    public Text price11;
    public Text price12;
    //public Text price13;
    //public Text price14;
    //public Text price15;
    //public Text price16;

    static int passivePriceRate = 10;

    int upgradeMaxLevel = 10;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateTitle();
        updatePrice();
    }

    //upgrademode mode1, upgrademode2 mode2, purchaseBy coin
    public void exec_upgrade(string param)
    {

        upgrademode mode1 = upgrademode.active;
        upgrademode2 mode2 = upgrademode2.coinBonus;
        purchaseBy coin = purchaseBy.activeCoin;

        if(param.Length != 3)
        {
            Debug.Log("param error!");
            return;
        }

        param = param.ToLower();
        char subParam1 = param[0]; //passive or active --> p, a
        char subParam2 = param[1]; //exe function --> c, e, p
        char subParam3 = param[2]; //coin mode ---> p, a

        if (subParam1 == 'p')
        {
            mode1 = upgrademode.passive;
        }
        if(subParam1 == 'a')
        {
            mode1 = upgrademode.active;
        }

        if(subParam2 == 'c')
        {
            mode2 = upgrademode2.coinBonus;
        }
        if (subParam2 == 'e')
        {
            mode2 = upgrademode2.expBonus;
        }
        if (subParam2 == 'p')
        {
            mode2 = upgrademode2.progressBarRate;
        }
        if (subParam3 == 'p')
        {
            coin = purchaseBy.passiveCoin;
        }
        if (subParam3 == 'a')
        {
            coin = purchaseBy.activeCoin;
        }


        //active
        if (mode1 == upgrademode.active)
        {
            switch (mode2)
            {
                case upgrademode2.coinBonus:
                    if(player.coin.A_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_ActiveCoinBonus()))
                        {
                            player.coin.A_upgradeLv++;
                        }
                    }
                    break;
                case upgrademode2.expBonus:
                    if(player.exp.A_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_ActiveExpBonus()))
                        {
                            player.exp.A_upgradeLv++;
                        }
                    }
                    break;
                case upgrademode2.progressBarRate:
                    if (player.progress.A_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_ActiveProgressBarRate()))
                        {
                            player.progress.A_upgradeLv++;
                        }
                    }
                    break;
            }
        }

        //passive
        else
        {
            switch (mode2)
            {
                case upgrademode2.coinBonus:
                    if (player.coin.P_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_PassiveCoinBonus()))
                        {
                            player.coin.P_upgradeLv++;
                        }
                    }
                    break;
                case upgrademode2.expBonus:
                    if (player.exp.P_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_PassiveExpBonus()))
                        {
                            player.exp.P_upgradeLv++;
                        }
                    }
                    break;
                case upgrademode2.progressBarRate:
                    if (player.progress.P_upgradeLv < upgradeMaxLevel)
                    {
                        if (purchase(coin, bal.getPrice_PassiveProgressBarRate()))
                        {
                            player.progress.P_upgradeLv++;
                        }
                    }
                    break;
            }
        }
    }
    private string level2String(int lv, int maxLv)
    {
        if(lv < maxLv)
        {
            return lv.ToString();
        }
        else
        {
            return "MAX";
        }
    }

    public void updateTitle()
    {
        //update text for title of each upgrades 
        int max = upgradeMaxLevel;
        //---passive ---
        //Passive Progress Bar Rate

        passive_progress.text = string.Format("Progress Bar Rate(LV.{0})",
           level2String(player.progress.P_upgradeLv, max));

        //Passive Exp Bonus
        passive_exp.text = string.Format("Exp Bonus(LV.{0})",
          level2String(player.exp.P_upgradeLv, max));

        //Passive Coin Bonus
        passive_coin.text = string.Format("Coin Bonus(LV.{0})",
           level2String(player.coin.P_upgradeLv, max));



        //---active ---
        //Active Progress Bar Rate
        active_progress.text = string.Format("Progress Bar Rate(LV.{0})",
           level2String(player.progress.A_upgradeLv, max));
        //Active Exp Bonus
        active_exp.text = string.Format("Exp Bonus(LV.{0})",
           level2String(player.exp.A_upgradeLv, max));
        //Active Coin Bonus
        active_coin.text = string.Format("Coin Bonus(LV.{0})",
           level2String(player.coin.A_upgradeLv, max));



    }
    public void updatePrice()
    {
        //passive
        price1.text = price2String("PC", bal.getPrice_PassiveProgressBarRate()* passivePriceRate, player.progress.P_upgradeLv, upgradeMaxLevel);
        price2.text = price2String("AC", bal.getPrice_PassiveProgressBarRate(), player.progress.P_upgradeLv, upgradeMaxLevel);

        price3.text = price2String("PC", bal.getPrice_PassiveExpBonus()* passivePriceRate, player.exp.P_upgradeLv, upgradeMaxLevel);
        price4.text = price2String("AC", bal.getPrice_PassiveExpBonus(), player.exp.P_upgradeLv, upgradeMaxLevel);

        price5.text = price2String("PC", bal.getPrice_PassiveCoinBonus()* passivePriceRate, player.coin.P_upgradeLv, upgradeMaxLevel);
        price6.text = price2String("AC", bal.getPrice_PassiveCoinBonus(), player.coin.P_upgradeLv, upgradeMaxLevel);
        //active

        price7.text = price2String("PC", bal.getPrice_ActiveProgressBarRate()* passivePriceRate, player.progress.A_upgradeLv, upgradeMaxLevel);
        price8.text = price2String("AC", bal.getPrice_ActiveProgressBarRate(), player.progress.A_upgradeLv, upgradeMaxLevel);

        price9.text = price2String("PC", bal.getPrice_ActiveExpBonus()* passivePriceRate, player.exp.A_upgradeLv, upgradeMaxLevel);
        price10.text = price2String("AC", bal.getPrice_ActiveExpBonus(), player.exp.A_upgradeLv, upgradeMaxLevel);

        price11.text = price2String("PC", bal.getPrice_ActiveCoinBonus()* passivePriceRate, player.coin.A_upgradeLv, upgradeMaxLevel);
        price12.text = price2String("AC", bal.getPrice_ActiveCoinBonus(), player.coin.A_upgradeLv, upgradeMaxLevel);

        //price13.text = string.Format("Passive Coin: {0}", incremental.getPrice_ActiveCoinBonus() * passivePriceRate);
        //price14.text = string.Format("Active Coin: {0}", incremental.getPrice_ActiveCoinBonus());
        //price15.text = string.Format("Passive Coin: {0}", "test");
        //price16.text = string.Format("Active Coin: {0}", "empty");
    }

    private string price2String(string coin, int price, int lv, int max)
    {
        if (lv >= max)
        {
            return "MAX";
        }
        else
            return string.Format("{0} -{1}", coin, price);
    }

    private bool purchase(purchaseBy coin, int price)
    {
        if(coin == purchaseBy.activeCoin)
        {
            if(player.coin.active >= price)
            {
                player.coin.active -= price;
                return true;
            }
        }
        else if(coin == purchaseBy.passiveCoin)
        {
            if(player.coin.passive >= price * passivePriceRate)
            {
                player.coin.passive -= price * passivePriceRate;
                return true;
            }
        }
        return false;
    }
}
