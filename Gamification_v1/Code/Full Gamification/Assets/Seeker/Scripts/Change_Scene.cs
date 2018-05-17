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
    public int progress_update = 0;
    public GameObject fades;
    public int fade_dir;


    void Start()
    {
        fades = GameObject.Find("Fade");
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

    public void ProgressAdd()
    {
        GlobalControl.Instance.quest_progress += progress_update;
    }

    public void ProgressSet()
    {
        if (GlobalControl.Instance.quest_progress < progress_update)
            GlobalControl.Instance.quest_progress = progress_update;
    }
}
