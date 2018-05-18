using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTab : MonoBehaviour {

    public Text quest_box;

	// Use this for initialization
	void Start () {
        quest_box.text = GlobalControl.Instance.current_quest;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetQuest()
    {
        quest_box.text = GlobalControl.Instance.current_quest;
    }
}
