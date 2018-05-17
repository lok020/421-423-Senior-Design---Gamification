using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    }

    public void CheckChange()
    {
        if (can_change)
        {
            if (fades != null)
            {
                fades.GetComponent<Fading>().enabled = true;
                fades.GetComponent<Fading>().fade_direction = fade_dir;
            }

            Invoke("ChangeScene", delay);
        }

        else
        {
            dialogue.GetComponent<DialogueInstances>().SetInstance(lines);
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(next_level);
    }
}
