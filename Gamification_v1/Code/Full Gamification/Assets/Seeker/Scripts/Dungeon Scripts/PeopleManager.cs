using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour {

    public GameObject trapped_villager;
    private GameObject player;
    private GameObject flasher;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        flasher = GameObject.Find("White");

        gameObject.GetComponent<Flash>().flash_object = flasher;
        trapped_villager.GetComponent<Following>().target = player;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
