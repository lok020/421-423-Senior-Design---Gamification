using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {

    public GameObject resumeButton;
    public Text resumeInfo;
    public Text fix_easy_button;
    public Text fix_medium_button;
    public Text fix_hard_button;
    // Use this for initialization
    void Start ()
    {
        /*if(player.Sudoku.sudokuContinue == 1)
        {
            resumeButton.SetActive(true);
            //resumeInfo.text = string.Format("Time: ")
        }
        else
        {
            resumeButton.SetActive(false);
        }
        fix_easy_button.text = "Easy\r\n(" + player.Sudoku.fix_easy_index + "/" + fixedSudoku.easy.Count + ")";
        fix_medium_button.text = "Medium\r\n(" + player.Sudoku.fix_medium_index + "/" + fixedSudoku.medium.Count + ")";
        fix_hard_button.text = "Hard\r\n(" + player.Sudoku.fix_hard_index + "/" + fixedSudoku.hard.Count + ")";*/

        if (resumeButton != null)
        {
            if (player.Sudoku.sudokuContinue == 1)
            {
                resumeButton.SetActive(true);
                //resumeInfo.text = string.Format("Time: ")
            }
            else
            {
                resumeButton.SetActive(false);
            }
        }
        if (fix_easy_button != null)
        {
            fix_easy_button.text = "Easy\r\n(" + player.Sudoku.fix_easy_index + "/" + fixedSudoku.easy.Count + ")";
        }
        if (fix_medium_button != null)
        {
            fix_medium_button.text = "Medium\r\n(" + player.Sudoku.fix_medium_index + "/" + fixedSudoku.medium.Count + ")";
        }
        if (fix_hard_button != null)
        {
            fix_hard_button.text = "Hard\r\n(" + player.Sudoku.fix_hard_index + "/" + fixedSudoku.hard.Count + ")";
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
            //PlayerPrefs.SetInt("sudokuContinue", 0);
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.easy;
        }
        if(mode == 2)
        {
            //PlayerPrefs.SetInt("sudokuContinue", 0);
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.medium;
        }
        if(mode == 3)
        {
            //PlayerPrefs.SetInt("sudokuContinue", 0);
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.hard;
        }
        if(mode == 5)
        {
            //PlayerPrefs.SetInt("sudokuContinue", 0);
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.fix_easy;
        }
        if(mode == 6)
        {
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.fix_medium;
        }
        if(mode == 7)
        {
            player.Sudoku.sudokuContinue = 0;
            Difficulty.diff = difficult.fix_hard;
        }
        if(player.Incre.stamina.cur > 0)
        {
            player.Incre.stamina.cur--;
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
