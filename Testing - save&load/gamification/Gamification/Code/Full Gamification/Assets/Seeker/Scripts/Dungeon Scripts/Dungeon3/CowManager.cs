using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowManager : MonoBehaviour {

    public GameObject cow;
    public GameObject player;
    public ThirdDungeon dungeon_stuff;
    public TextInfo notification;

    // Use this for initialization
    void Start()
    {

        //gameObject.GetComponent<Flash>().flash_object = flasher;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckFruit()
    {
        if (dungeon_stuff.has_fruit > 0)
        {
            
            cow.GetComponent<Following>().enabled = true;
            cow.GetComponent<Following>().target = player.GetComponent<Followers>().LastFollower();
            cow.GetComponent<Following>().SetTarget();
            player.GetComponent<Followers>().AddFollower(cow);
            dungeon_stuff.GetCow();
            dungeon_stuff.has_fruit--;
            notification.AddText("You hold out a fresh vine fruit and the beast happily consumes it.");
            notification.AddText("It begins following you around.");
            gameObject.SetActive(false);
        }
    }
}
