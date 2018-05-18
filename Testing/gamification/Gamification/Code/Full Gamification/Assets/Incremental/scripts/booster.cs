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

    static int maxProgressBooster = 10*60;
    static int maxExpBooster = 60*60;
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
        itemAmount1.text = "x" + player.progress.numBooster;
        itemAmount2.text = "x" + player.exp.numBooster;
        itemAmount3.text = "x" + player.coin.numBooster;

        //adjust fillamount
        if(player.progress.boosterRemain > 0)
        {
            itemCooldown1.fillAmount = (float)player.progress.boosterRemain / (float)maxProgressBooster;
            player.progress.boosterRate = 1.5;
            player.progress.boosterRemain--;
        }
        else
        {
            player.progress.boosterRate = 1;
            itemCooldown1.fillAmount = 0;

        }
        if(player.exp.boosterRemain > 0)
        {
            itemCooldown2.fillAmount = (float)player.exp.boosterRemain / (float)maxExpBooster;
            player.exp.boosterRate = 1.5;
            player.exp.boosterRemain--;
        }
        else
        {
            player.exp.boosterRate = 1;
            itemCooldown2.fillAmount = 0;
        }
        if(player.coin.boosterRemain > 0)
        {
            itemCooldown3.fillAmount = (float)player.coin.boosterRemain / (float)maxCoinBooster;
            player.coin.boosterRate = 1.5;
            player.coin.boosterRemain--;
        }
        else
        {
            player.coin.boosterRate = 1;
            itemCooldown3.fillAmount = 0;
        }
    }

    public void boosterClick(int type) 
    {
        switch(type)
        {
            case 1:
                if(player.progress.numBooster > 0 && player.progress.boosterRemain == 0)
                {
                    player.progress.numBooster--;
                    player.progress.boosterRemain = maxProgressBooster;
                }
                break;

            case 2:
                if (player.exp.numBooster > 0 && player.exp.boosterRemain == 0)
                {
                    player.exp.numBooster--;
                    player.exp.boosterRemain = maxExpBooster;
                }
                break;

            case 3:
                if (player.coin.numBooster > 0 && player.coin.boosterRemain == 0)
                {
                    player.coin.numBooster--;
                    player.coin.boosterRemain = maxCoinBooster;
                }
                break;
        }
    }
}
