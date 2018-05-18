using UnityEngine;
using System.Collections;

public class PlayerSetActiveHelper : MonoBehaviour {
    public GameObject Player;
	// Use this for initialization
	void Start () {
	    
	}
	
    void Awake()
    {
        Debug.Log("Setting player active");
        Player.SetActive(true);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
