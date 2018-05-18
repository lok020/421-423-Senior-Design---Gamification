using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Change_Scene : MonoBehaviour {

    public float delay;
    public string next_level;
    public bool can_change = true;
    public string[] lines;
    public GameObject dialogue;
    public GameObject fades;
    public int fade_dir;


    void Start()
    {
        if(GameObject.Find("Fade") != null)
        {
            fades = GameObject.Find("Fade");
        }
    }

    public void CheckChange()
    {
        if (can_change)
        {
            if (fades != null)
            {
                fades.SetActive(true);
                fades.GetComponent<Fading>().enabled = true;
                fades.GetComponent<Fading>().BeginFade(fade_dir);
            }

            Invoke("ChangeScene", delay);
        }

        else
        {
            dialogue.GetComponent<DialogueInstances>().SetInstance(lines);
        }
    }

    public void ChangeScene()
    {

        SceneManager.LoadScene(next_level);
    }

    public void ChangeCanChange()
    {
        if (can_change == false)
        {
            can_change = true;
        }
        else
        {
            can_change = false;
        }
    }

    public void LoadStart()
    {

        switch (GlobalControl.Instance.quest_progress)
        {
            case 0:
                next_level = "Intro1";
                break;
            case 1:
                next_level = "Town_Market1";
                break;
            case 2:
                next_level = "Town_Market1";
                break;
            case 3:
                next_level = "Town_Home1";
                break;
            case 4:
                next_level = "First_Dungeon";
                break;
            default:
                next_level = "Town_Market";
                break;
        }

        Invoke("ChangeScene", delay);
    }
}
