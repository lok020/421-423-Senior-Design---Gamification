using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSceneChange : MonoBehaviour {

    public GameObject[] scene_change_objects;
    public GameObject[] enable_change_objects;

	// Use this for initialization
	void Start () {
        int x = 0;
		for (x = 0; x < scene_change_objects.Length; x++)
        {
            scene_change_objects[x].GetComponent<Change_Scene>().can_change = false;
        }

        for (x = 0; x < enable_change_objects.Length; x++)
        {
            enable_change_objects[x].GetComponent<Change_Scene>().can_change = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
