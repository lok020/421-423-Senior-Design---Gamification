using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour {

    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}

    public void click()
    {
        audioSource.mute = !audioSource.mute;
    }

	// Update is called once per frame
	void Update () {
        
	}
}
