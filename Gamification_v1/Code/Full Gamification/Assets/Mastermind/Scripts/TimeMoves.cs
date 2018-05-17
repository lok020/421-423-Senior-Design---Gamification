using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeMoves : MonoBehaviour
{

    public Text txtTime;
    public Text txtMoves;

    TimeSpan timeSpan;
    int moves;
    int frameCounter;

    // Use this for initialization
    void Start()
    {
        timeSpan = TimeSpan.FromSeconds(0);
        moves = 0;
        frameCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        txtTime.text = timeSpan.ToString();
        txtMoves.text = moves.ToString();
        if (frameCounter >= 60)
        {
            timeSpan = timeSpan + TimeSpan.FromSeconds(1);
            frameCounter = 0;
            Debug.Log(timeSpan.ToString());
        }
        else
        {
            frameCounter++;
        }
    }
    public void incrementMove()
    {
        moves++;
    }

}