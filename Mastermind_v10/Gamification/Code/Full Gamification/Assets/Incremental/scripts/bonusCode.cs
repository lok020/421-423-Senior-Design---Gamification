using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class bonusCode
{
    //generate code from https://www.randomcodegenerator.com/en/generate-codes
    static string randomCode = "jrskdskszj rqqbhbtbtg ytvjbdbxqv ctmymmkpzz ghwpszgqww sdmmfmmbhn zcsfshgrpy mvrkwzkqps xgsvnbqhzf dvvgwmqvhw zgbqcckhqy xyjrmhntyq krvntjfmrt zhdvtcjgkm wryknsvmsh rftgnhwdyq fqzvbbnzxp gknbsjjkzp gpkpqmhysd bmydqphgvm wcvffgjnmv shcjpdcyxg krqncyqyhz wkyyzsfvhw stsszndnwr ggxqqhxrrs jmhpkjbxdz dhnxmwttcf tsqqdddtgs dvrhhkjvwv ppxzpnmhtm szhpxsjszf wqxyzpsjxq jwrswxqhkf ypqttmpyjm crnbvxcrpt khbhjzvksp fmbpdhnwvh ffxzygnxfq bnbcydhthf gvbrhggdrg pzmwpfmsnp dwzhyprdqg wvrcppcpjz gccsctbfnn tjpsnhbgnc gtdmxkhksh mfhkrtzzfj jvwbgffdyb nptjxpqcws ktyktdmcmh qjybkwmsvs smbnzstxkz zxgysjtdfy qghspqsnst pvvbvnnzwh bpwgzkfpjy xydpsqjhgx jgzghhxknr dhtdkxqnyt tvdzxxgwxz jjkjbtcgty cftfkrtdvc chsxnnchzw qwftmnhmrq rhrsgwtsrf krwzpqkhrh nfytcftnkk vkgjkfrzwz dffckkrdqq btsqdrwdpv htrtvxkbcf mpzqhrxqgk bbbpzyxqpc vytzncnfwn hgvhgffgdn zbzfvhnmkk gbzchfwpwd zvbryfzhtp qzhkckbkhj yrzckqhtbc qhtywpcywq xqjqhrrrwz kxbmhgpmzy xzwfvcbxkj zvxcyzgysp vhczrdbqyh rdkprfhkww qxtkrfyykw nxgbmypfyt swtjcyssng pnszwyhvdx gbwghrjtbv rqdgkgnngz qhtfdqsgnb ngczsfkrys dzmmprsjvb jpcpqzqftb rgmhmrsxkt jcgmxpndnt";
    static Dictionary<string, string> dataset = new Dictionary<string, string>();
    public static void parseCode()
    {
        string[] codeList = randomCode.Split(' ');
        int count = 0;
        foreach (string code in codeList)
        {
            switch(count)
            {
                case 0:
                    dataset.Add(code, "money");
                    break;
                case 1:
                    dataset.Add(code, "time");
                    break;
                case 2:
                    dataset.Add(code, "stamina");
                    break;
                case 3:
                    dataset.Add(code, "item");
                    break;
            }

            if(count >= 3)
            {
                count = 0;
            }
            else
            {
                count++;
            }
        }
    }
    
    //get list of code with category. It will be used for instructors. 
    static void getList()
    {
        string res = "";
        foreach(var code in dataset)
        {
            res += code.Key + "\t\t\t" + code.Value + "\r\n";
        }
        Debug.Log(res);
    }

    public static void analyzeCode(string code, strDele fun)
    {
        if(dataset.Count == 0)
        {
            parseCode();    //parsing code
            //getList();
        }
        if(code == null || code == "" || !dataset.ContainsKey(code))
        {
            fun.Invoke("Invalid Code.", "Error!");
            return;
        }

        if(player.Incre.usedCode.Contains(code))
        {
            fun.Invoke("You have already used this code.", "Error!");
            return;
        }

        //=========developer -------------//
        if(code == "money")
        {
            fun.Invoke("Great Job on Class work!", "1000 Money Added!");
            player.Incre.coin.active += 1000;
            player.Incre.coin.passive += 1000;
        }
        if(code == "time")
        {
            fun.Invoke("Great Job on Class work!", "time Added!");
            player.Incre.timeleft.cur+=60*10;
        }
        if(code == "stamina")
        {
            fun.Invoke("Great Job on Class work!", "stamina Added!");
            player.Incre.stamina.cur++;
        }


        string reward = "";
        dataset.TryGetValue(code, out reward);
        //============= player ==================//
        if (reward == "money")
        {
            fun.Invoke((bal.getPassiveCoinBonus() * 10).ToString() + "You gained Passive Coin and " + (bal.getActiveCoinBonus() * 10).ToString() + " Active Coin. ", "Great Job on Class work!");
            player.Incre.coin.passive += (bal.getPassiveCoinBonus() * 10);
            player.Incre.coin.active += (bal.getActiveCoinBonus()*10);
        }
        if (reward == "time")
        {
            //add 10 minutes
            player.Incre.timeleft.cur += 60 * 10;
            fun.Invoke("You gained extra 10 minutes play time!", "Great Job on Class work!");
        }
        if (reward == "item")
        {
            player.Incre.coin.numBooster += 2;
            player.Incre.exp.numBooster += 2;
            player.Incre.progress.numBooster += 2;
            fun.Invoke("You gained 2 of each booster items!", "Great Job on Class work!");
        }
        if (reward == "stamina")
        {
            //add 5 more stamina
            player.Incre.stamina.cur += 5;
            fun.Invoke("You gained 5 extra stamina!", "Great Job on Class work!");
        }
        player.Incre.usedCode.Add(code);
    }
	
}
