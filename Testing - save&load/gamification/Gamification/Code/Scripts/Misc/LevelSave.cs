using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSave {
    public List<GameObject> objects;
    public string name;
    public bool comingFromCombat = false;
    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public LevelSave()
    {
        objects = new List<GameObject>();
    }
}
