using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class titleManager : MonoBehaviour {

    public Text myTitles;
    bool active;
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        checkTitle();
	}

    public void addTitle(string name, string description, string color)
    {
        myTitles.text += string.Format("<color={0}><size=35>{1}</size></color>\n"
            +"{2}\n\n", color, name, description);
    }

    public void checkTitle()
    {
        //1. Show me the money1
        int index = 1;
        if(player.coin.active > 3000 && player.titleCollection[index] == false)
        {
            player.titleCollection[index] = true;
            addTitle("Show me the money1", "have more than 3000 coins", "cyan");
        }
        //2. Show me the money2
        index = 2;
        if (player.coin.active > 4000 && player.titleCollection[index] == false)
        {
            player.titleCollection[index] = true;
            addTitle("Show me the money2", "have more than 4000 coins", "blue");
        }
        
    }
}
