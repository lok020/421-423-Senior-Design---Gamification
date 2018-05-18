using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

    public List<Dialog> Dialog;
    
    private Image image;
    private Text text;
    private float time;

	// Use this for initialization
	void Start ()
    {
        //Find image background
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            if (i.name == "Speech Bubble Image")
            {
                image = i;
            }
        }
        //Find text
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Speech Bubble Text")
            {
                text = t;
            }
        }
        image.enabled = false;
        text.enabled = false;
        time = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (time > 0)
        {
            image.enabled = true;
            text.enabled = true;
            float step = Time.deltaTime;
            time -= step;
        }
        else
        {
            image.enabled = false;
            text.enabled = false;
        }
    }

    //Speak, matches the last Dialog in the list
    public void Speak(Inventory inventory)
    {
        float defaultTime = 5.0f;
        Dialog toSpeak = null;

        var player = GameObject.FindGameObjectWithTag("Player");
        var questManager = player.GetComponent<QuestManager>();
        var eventManager = player.GetComponent<EventManager>();

        foreach(Dialog dialog in Dialog)
        {
            if((questManager.ObjectiveCompleted(dialog.RequiredQuestID, dialog.RequiredObjectiveID) == true
                && questManager.ObjectiveCompleted(dialog.DontRunAfterQuestID, dialog.DontRunAfterObjectiveID) == false)
                || (dialog.RequiredQuestID < 0 && toSpeak == null))
            {
                toSpeak = dialog;
            }
        }
        if(toSpeak != null)
        {
            text.text = toSpeak.GetNext(eventManager, inventory);
            time = defaultTime;
        }
    }

    //Speak, used in cutscenes
    public void Speak(string dialog, float speechTime)
    {
        text.enabled = true;
        text.text = dialog;
        time = speechTime;
    }

    public string GetText()
    {
        return text.text;
    }
}
