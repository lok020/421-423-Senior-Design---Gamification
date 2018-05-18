using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToQuestArea : MonoBehaviour {

    public Change_Scene where;

	// Use this for initialization
	void Start () {
        where.can_change = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToQuest()
    {

        switch(GlobalControl.Instance.quest_progress)
        {
            case 5:
                where.next_level = "Second_Dungeon";
                where.can_change = true;
                break;
            case 7:
                where.next_level = "Third_Dungeon";
                where.can_change = true;
                break;
            default:
                break;
        }
        where.CheckChange();
    }
}
