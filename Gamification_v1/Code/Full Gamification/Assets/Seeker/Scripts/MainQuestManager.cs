using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainQuestManager : MonoBehaviour {

    public GameObject[] scenes;
    public int[] quest_progress_indicator;
    private bool no_scene = true;
    public GameObject standard_scene;


	// Use this for initialization
	void Start () {
        for (int x = 0; x < quest_progress_indicator.Length; x++)
        {
            if (quest_progress_indicator[x] == GlobalControl.Instance.quest_progress)
            {
                scenes[x].SetActive(true);
                no_scene = false;
            }
            else
            {
                scenes[x].SetActive(false);
            }
        }
        if (no_scene == true && standard_scene != null)
        {
            standard_scene.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void IncreaseQuestProgress()
    {
        GlobalControl.Instance.quest_progress++;
    }
}
