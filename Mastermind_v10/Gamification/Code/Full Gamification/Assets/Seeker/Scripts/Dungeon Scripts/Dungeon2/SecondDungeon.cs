using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SecondDungeon : MonoBehaviour {

    public GameObject player;
    public GameObject choice;
    private bool has_seeds;
    private bool move = true;

    public UnityEvent action_on;
    public UnityEvent action_off;

    void Start()
    {
        has_seeds = false;
        choice.SetActive(false);
    }

    void Update()
    {
    }

    public void GetSeeds()
    {
        has_seeds = true;
    }

    public void AttemptLeave()
    {
        move = false;
        choice.SetActive(true);
        choice.GetComponent<SecondDungeonPanel>().SetObject(gameObject);
        choice.GetComponent<SecondDungeonPanel>().SetText("Leave this dungeon?");
    }

    public void Leave()
    {
        if (has_seeds)
        {
            GlobalControl.Instance.QuestUpdate();
        }
        action_on.Invoke();
    }

    public void Stay()
    {
        action_off.Invoke();
        move = true;
    }
}
