using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInfo : MonoBehaviour {

    public Text[] notifications;
    private int line = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void AddText (string new_line)
    {
        if (line >= 5)
        {
            int x = 0;
            for (x = 0; x < 4; x ++)
            {
                notifications[x].text = notifications[x + 1].text;
            }
            notifications[4].text = new_line;
        }
        else
        {
            notifications[line].text = new_line;
            line++;
        }
    }
}
