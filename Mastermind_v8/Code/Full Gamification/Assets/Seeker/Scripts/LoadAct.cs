using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadAct : MonoBehaviour {

    public GameObject panel;
    public GameObject yes;
    public Text text_box;
    public string text;
    public string scene_name;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenPanel()
    {
        panel.SetActive(true);
        text_box.text = text;
        yes.GetComponent<Change_Scene>().next_level = scene_name;
    }



}
