using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour {

    public Text timer;
    private float seconds = 0;
    private int minutes = 0;
    private int hours = 0;

    // Use this for initialization
    void Start () {

	}
	
    public void UpdateTimer()
    {
        seconds += Time.deltaTime;
        timer.text = hours + "h: " + minutes + "m: " + (int)seconds + "s";

        if (seconds >= 60)
        {
            minutes++;
            seconds = 0;
        }
        else if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }
    }


	// Update is called once per frame
	void Update () {
        UpdateTimer();
    }
}
