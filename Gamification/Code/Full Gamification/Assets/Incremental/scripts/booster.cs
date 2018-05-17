using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class booster : MonoBehaviour {

    public Text itemAmount1;
    public Text itemAmount2;
    public Text itemAmount3;
    public Text itemAmount4;

    public Image itemCooldown1;
    public Image itemCooldown2;
    public Image itemCooldown3;
    public Image itemCooldown4;

    static int maxProgressBooster = 120*60;
    static int maxExpBooster = 120*60;
    static int maxCoinBooster = 120*60;
    
    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        updateBooster();
	}

    void updateBooster()
    {
        //update itemAmount
        itemAmount1.text = "x" + player.Incre.progress.numBooster;
        itemAmount2.text = "x" + player.Incre.exp.numBooster;
        itemAmount3.text = "x" + player.Incre.coin.numBooster;

        //adjust fillamount
        if(player.Incre.progress.boosterRemain > 0)
        {
            itemCooldown1.fillAmount = (float)player.Incre.progress.boosterRemain / (float)maxProgressBooster;
            player.Incre.progress.boosterRate = 1.5;
            player.Incre.progress.boosterRemain--;
        }
        else
        {
            player.Incre.progress.boosterRate = 1;
            itemCooldown1.fillAmount = 0;

        }
        if(player.Incre.exp.boosterRemain > 0)
        {
            itemCooldown2.fillAmount = (float)player.Incre.exp.boosterRemain / (float)maxExpBooster;
            player.Incre.exp.boosterRate = 1.5;
            player.Incre.exp.boosterRemain--;
        }
        else
        {
            player.Incre.exp.boosterRate = 1;
            itemCooldown2.fillAmount = 0;
        }
        if(player.Incre.coin.boosterRemain > 0)
        {
            itemCooldown3.fillAmount = (float)player.Incre.coin.boosterRemain / (float)maxCoinBooster;
            player.Incre.coin.boosterRate = 1.5;
            player.Incre.coin.boosterRemain--;
        }
        else
        {
            player.Incre.coin.boosterRate = 1;
            itemCooldown3.fillAmount = 0;
        }
    }

    public void boosterClick(int type) 
    {
        switch(type)
        {
            case 1:
                if(player.Incre.progress.numBooster > 0 && player.Incre.progress.boosterRemain == 0)
                {
                    player.Incre.progress.numBooster--;
                    player.Incre.progress.boosterRemain = maxProgressBooster;
                }
                break;

            case 2:
                if (player.Incre.exp.numBooster > 0 && player.Incre.exp.boosterRemain == 0)
                {
                    player.Incre.exp.numBooster--;
                    player.Incre.exp.boosterRemain = maxExpBooster;
                }
                break;

            case 3:
                if (player.Incre.coin.numBooster > 0 && player.Incre.coin.boosterRemain == 0)
                {
                    player.Incre.coin.numBooster--;
                    player.Incre.coin.boosterRemain = maxCoinBooster;
                }
                break;
        }
    }
}
