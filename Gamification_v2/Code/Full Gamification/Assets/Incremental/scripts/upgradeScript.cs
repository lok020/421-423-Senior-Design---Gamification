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

    double passivePriceRate;



    // Use this for initialization
    void Start()
    {
        passivePriceRate = 3;
        updateTitle();
        updatePrice();
    }

    // Update is called once per frame
    void Update()
    {

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
                    if(player.active_CoinBonusLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_ActiveCoinBonus()))
                        {
                            player.active_CoinBonusLV++;
                        }
                    }
                    break;
                case upgrademode2.expBonus:
                    if(player.active_expBonusLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_ActiveExpBonus()))
                        {
                            player.active_expBonusLV++;
                        }
                    }
                    break;
                case upgrademode2.progressBarRate:
                    if (player.active_progressBarRateLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_ActiveProgressBarRate()))
                        {
                            player.active_progressBarRateLV++;
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
                    if (player.passive_CoinBonusLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_PassiveCoinBonus()))
                        {
                            player.passive_CoinBonusLV++;
                        }
                    }
                    break;
                case upgrademode2.expBonus:
                    if (player.passive_expBonusLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_PassiveExpBonus()))
                        {
                            player.passive_expBonusLV++;
                        }
                    }
                    break;
                case upgrademode2.progressBarRate:
                    if (player.passive_progressBarRateLV < incremental.upgradeMaxLevel)
                    {
                        if (purchase(coin, incremental.getPrice_PassiveProgressBarRate()))
                        {
                            player.passive_progressBarRateLV++;
                        }
                    }
                    break;
            }
        }
        updateTitle();
        updatePrice();
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
        int max = incremental.upgradeMaxLevel;
        //---passive ---
        //Passive Progress Bar Rate
        
        passive_progress.text = string.Format("Passive Progress Bar Rate(LV.{0}->LV.{1})",
           level2String(player.passive_progressBarRateLV, max),
           level2String(player.passive_progressBarRateLV + 1, max));

        //Passive Exp Bonus
        passive_exp.text = string.Format("Passive Exp Bonus(LV.{0}->LV.{1})",
          level2String(player.passive_expBonusLV, max),
          level2String(player.passive_expBonusLV + 1, max));

        //Passive Coin Bonus
        passive_coin.text = string.Format("Passive Coin Bonus(LV.{0}->LV.{1})",
           level2String(player.passive_CoinBonusLV, max),
           level2String(player.passive_CoinBonusLV + 1, max));

        

        //---active ---
        //Active Progress Bar Rate
        active_progress.text = string.Format("Active Progress Bar Rate(LV.{0}->LV.{1})",
           level2String(player.active_progressBarRateLV, max),
           level2String(player.active_progressBarRateLV + 1, max));
        //Active Exp Bonus
        active_exp.text = string.Format("Active Exp Bonus(LV.{0}->LV.{1})",
           level2String(player.active_expBonusLV, max),
           level2String(player.active_expBonusLV + 1, max));
        //Active Coin Bonus
        active_coin.text = string.Format("Active Coin Bonus(LV.{0}->LV.{1})",
           level2String(player.active_CoinBonusLV, max),
           level2String(player.active_CoinBonusLV + 1, max));



    }
    public void updatePrice()
    {
        //passive
        price1.text = string.Format("PC: {0}", incremental.getPrice_PassiveProgressBarRate() * passivePriceRate);
        price2.text = string.Format("AC: {0}", incremental.getPrice_PassiveProgressBarRate());
        price3.text = string.Format("PC: {0}", incremental.getPrice_PassiveExpBonus() * passivePriceRate);
        price4.text = string.Format("AC: {0}", incremental.getPrice_PassiveExpBonus());
        price5.text = string.Format("PC: {0}", incremental.getPrice_PassiveCoinBonus() * passivePriceRate);
        price6.text = string.Format("AC: {0}", incremental.getPrice_PassiveCoinBonus());

        //active
        price7.text = string.Format("PC: {0}", incremental.getPrice_ActiveProgressBarRate() * passivePriceRate); 
        price8.text = string.Format("AC: {0}", incremental.getPrice_ActiveProgressBarRate());
        price9.text = string.Format("PC: {0}", incremental.getPrice_ActiveExpBonus() * passivePriceRate);
        price10.text = string.Format("AC: {0}", incremental.getPrice_ActiveExpBonus());
        price11.text = string.Format("PC: {0}", incremental.getPrice_ActiveCoinBonus() * passivePriceRate);
        price12.text = string.Format("AC: {0}", incremental.getPrice_ActiveCoinBonus());
        //price13.text = string.Format("Passive Coin: {0}", incremental.getPrice_ActiveCoinBonus() * passivePriceRate);
        //price14.text = string.Format("Active Coin: {0}", incremental.getPrice_ActiveCoinBonus());
        //price15.text = string.Format("Passive Coin: {0}", "test");
        //price16.text = string.Format("Active Coin: {0}", "empty");
    }

   

    private bool purchase(purchaseBy coin, int price)
    {
        if(coin == purchaseBy.activeCoin)
        {
            if(player.coin_active >= price)
            {
                player.coin_active -= price;
                return true;
            }
        }
        else if(coin == purchaseBy.passiveCoin)
        {
            if(player.coin_passive >= price * 3)
            {
                player.coin_passive -= price * 3;
                return true;
            }
        }
        return false;
    }
}
