using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {

    public GameObject resumeButton;
    public Text resumeInfo;
	// Use this for initialization
	void Start ()
    {
        if(PlayerPrefs.GetInt("sudokuContinue") == 1)
        {
            resumeButton.SetActive(true);
            //resumeInfo.text = string.Format("Time: ")
        }
        else
        {
            resumeButton.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void NextScene_Menu()
    {
        SceneManager.LoadScene("Mastermind_Menu", LoadSceneMode.Single);
    }

    public void NextScene_Play()
    {
        SceneManager.LoadScene("Mastermind_Play", LoadSceneMode.Single);
    }

    public void NextScene_PlayZone(int mode)
    {
        if(mode == 1)
        {
            PlayerPrefs.SetInt("sudokuContinue", 0);
            Difficulty.diff = difficult.easy;
        }
        if(mode == 2)
        {
            PlayerPrefs.SetInt("sudokuContinue", 0);
            Difficulty.diff = difficult.medium;
        }
        if(mode == 3)
        {
            PlayerPrefs.SetInt("sudokuContinue", 0);
            Difficulty.diff = difficult.hard;
        }
        if(mode == 5)
        {
            PlayerPrefs.SetInt("sudokuContinue", 0);
            Difficulty.diff = difficult.fix;
        }
        if(player.stamina.cur > 0)
        {
            player.stamina.cur--;
            SceneManager.LoadScene("Mastermind_PlayZone", LoadSceneMode.Single);
        }
    }

    public void NextScene_Stat()
    {
        SceneManager.LoadScene("Mastermind_Stat", LoadSceneMode.Single);
    }

    public void NextScene_Setting()
    {
        SceneManager.LoadScene("Mastermind_Setting", LoadSceneMode.Single);
    }
}
