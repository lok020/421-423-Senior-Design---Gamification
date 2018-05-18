using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageOverlayController : MonoBehaviour {

    private Queue<string> messageQueue = new Queue<string>();   //Message queue
    private Text[] lines = new Text[lineCount];                 //Text objects on screen
    private float[] times = new float[lineCount];               //Remaining time for message to be displayed
    private int[] states = new int[lineCount];                  //0 = free, 1 = fade in, 2 = active, 3 = fade out

    private static int lineCount = 4;
    private float fadeInTime = 0.5f, activeTime = 5.0f, fadeOutTime = 1.0f;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);

        //Debug.Log("Created overlay controller!");

        var linesText = GetComponentsInChildren<Text>();
        foreach(var line in linesText)
        {
            //Debug.Log("Found line, name: " + line.gameObject.name);
            line.enabled = false;
            line.text = "";
            SetAlpha(line, 0.0f);
            if (line.gameObject.name == "Text_Line_1")
            {
                lines[0] = line;
                times[0] = 0;
                states[0] = 0;
            }
            if (line.gameObject.name == "Text_Line_2")
            {
                lines[1] = line;
                times[1] = 0;
                states[1] = 0;
            }
            if (line.gameObject.name == "Text_Line_3")
            {
                lines[2] = line;
                times[2] = 0;
                states[2] = 0;
            }
            if (line.gameObject.name == "Text_Line_4")
            {
                lines[3] = line;
                times[3] = 0;
                states[3] = 0;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        //First decrement each time and update the states
        for(int i = 0; i < lineCount; i++)
        {
            if (times[i] <= 0) continue;
            times[i] -= Time.deltaTime;
            //Update the state if the timer is done
            if (times[i] <= 0)
            {
                states[i]++;
                states[i] %= 4;
                //Update the time if the new state is 2 or 3
                if (states[i] == 2) times[i] = activeTime;
                if (states[i] == 3) times[i] = fadeOutTime;
            }
        }
        //Now update the drawn text depending on the state
        for(int i = 0; i < lineCount; i++)
        {
            switch(states[i])
            {
                //Line has been cleared OR line is already clear
                case 0:
                    {
                        //If text has already been disabled, continue
                        if (lines[i].enabled == false) continue;
                        //Shift up text
                        int lastShifted = i;
                        for(int j = i; j < lineCount; j++)
                        {
                            if(states[j] != 0)
                            {
                                lines[j - 1].text = lines[j].text;
                                lines[j - 1].color = lines[j].color;
                                times[j - 1] = times[j];
                                states[j - 1] = states[j];
                                lastShifted = j;
                            }
                        }
                        //Clear the last shifted variable (defaults to "i" if nothing shifted)
                        lines[lastShifted].enabled = false;
                        lines[lastShifted].text = "";
                        SetAlpha(lines[lastShifted], 0.0f);
                        times[lastShifted] = 0;
                        states[lastShifted] = 0;
                        //Decrement i if anything shifted
                        if(lastShifted != i) i--;
                        break;
                    }
                //Line is fading in
                case 1:
                    {
                        SetAlpha(lines[i], (fadeInTime - times[i]) / fadeInTime);    //Alpha will go from 0.0 to 1.0
                        break;
                    }
                //Line is opaque, do nothing
                case 2: break;
                //Line is fading out
                case 3:
                    {
                        SetAlpha(lines[i], times[i] / fadeOutTime);     //Alpha will go from 1.0 to 0.0
                        break;
                    }
            }
        }
        //Finally, if there is anything in the queue, see if it can be displayed
        if(messageQueue.Count > 0)
        {
            //Find the first empty message, if there is any
            for(int i = 0; i < lineCount; i++)
            {
                if (messageQueue.Count == 0) break;
                if(states[i] == 0)
                {
                    //Display the message
                    lines[i].enabled = true;
                    lines[i].text = messageQueue.Dequeue();
                    SetAlpha(lines[i], 0.0f);
                    states[i] = 1;
                    times[i] = fadeInTime;
                }
            }
        }
	}

    //Set alpha color
    private void SetAlpha(Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;    //Alpha will go from 1.0 to 0.0
        text.color = color;
    }

    //Add message to queue
    public void EnqueueMessage(string message)
    {
        //Limit queue size to 10,000 (this should be reasonable)
        if(messageQueue.Count < 10000)
            messageQueue.Enqueue(message);
    }
}
