using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainQuestManager : MonoBehaviour {

    public GameObject scene;
    public int quest_progress_indicator;


	// Use this for initialization
	void Start () {
        if (!GlobalControl.Instance.GetVisitArea(quest_progress_indicator))
        {
            scene.SetActive(true);
            GlobalControl.Instance.VisitArea(quest_progress_indicator);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
