using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void NextScene_Play()
    {
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }
}
