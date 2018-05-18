﻿using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void NextScene_Menu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void NextScene_Play()
    {
        SceneManager.LoadScene("Play", LoadSceneMode.Single);
    }

    public void NextScene_PlayZone()
    {
        SceneManager.LoadScene("PlayZone", LoadSceneMode.Single);
    }

    public void NextScene_Stat()
    {
        SceneManager.LoadScene("Stat", LoadSceneMode.Single);
    }

    public void NextScene_Setting()
    {
        SceneManager.LoadScene("Setting", LoadSceneMode.Single);
    }
}