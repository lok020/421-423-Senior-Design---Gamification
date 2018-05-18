using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class bonusCode
{ 
    public static void analyzeCode(string code)
    {
        if(code == "money")
        {
            player.coin.active += 1000;
            player.coin.passive += 1000;
        }
        if(code == "time")
        {
            player.timeleft.cur+=60*10;
        }
        if(code == "stamina")
        {
            player.stamina.cur++;
        }
    }
	
}
