using UnityEngine;
using System.Collections;

public class InstanceSwitchingTest : MonoBehaviour {
    string currentLevel;
    AsyncOperation async;

	// Use this for initialization
    //on load of the test it sets this script to not destroy over switching instances
    //then it calls an asyncronous routine for each level tested to switch there and logs whether the scene has been switched to the desired instance
	void Start () {
        DontDestroyOnLoad(this);
        StartCoroutine(load("Lobby"));
        StartCoroutine(load("Forest"));
        StartCoroutine(load("Lobby"));
        StartCoroutine(load("Lobby"));
        StartCoroutine(load("Forest"));
        StartCoroutine(load("Menu"));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator load(string level)
    {
        async = Application.LoadLevelAsync(level);
        yield return async;
        if(level == Application.loadedLevelName)
        {
            Debug.Log("Instance switch to " + level + " passed");
        }
        else
        {
            Debug.Log("Instance switch to " + level + " failed");
        }

    }
}
