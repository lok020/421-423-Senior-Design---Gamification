using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThirdDungeon : MonoBehaviour {

    public GameObject player;
    public GameObject choice;
    public int has_fruit;
    public int has_cows;
    public int required_cows;
    public Text number_fruits;
    public Text number_cows;
    private bool move = true;

    public UnityEvent action_on;
    public UnityEvent action_off;

    void Start()
    {
        has_fruit = 0;
        has_cows = 0;
        number_fruits.text = "Fruits in possession of: " + has_fruit.ToString();
        number_cows.text = "Cows tamed: " + has_cows.ToString() + "/" + required_cows.ToString();
        choice.SetActive(false);
    }

    void Update()
    {
    }

    public void GetFruit()
    {
        has_fruit++;
        number_fruits.text = "Fruits in possession of: " + has_fruit.ToString();
    }

    public void GetCow()
    {
        has_cows++;
        number_cows.text = "Cows tamed: " + has_cows.ToString() + "/" + required_cows.ToString();
    }

    public void AttemptLeave()
    {
        move = false;
        choice.SetActive(true);
        choice.GetComponent<ThirdDungeonPanel>().SetObject(gameObject);
        choice.GetComponent<ThirdDungeonPanel>().SetText("Leave this dungeon?");
    }

    public void Leave()
    {
        if (has_cows == required_cows)
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
