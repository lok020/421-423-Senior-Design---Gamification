using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainQuestManager : MonoBehaviour {

    public int quest_progress_requirement;
    public GameObject standard_scene;


	// Use this for initialization
	void Start () {
        if (GlobalControl.Instance.quest_visited[quest_progress_requirement] == false && standard_scene != null)
        {
            standard_scene.SetActive(true);
            GlobalControl.Instance.quest_visited[quest_progress_requirement] = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
