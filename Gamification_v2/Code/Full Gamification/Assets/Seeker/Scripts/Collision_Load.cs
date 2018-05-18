using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Collision_Load : MonoBehaviour {

    public UnityEvent OnCollision;
    public UnityEvent OnExit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D (Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            OnCollision.Invoke();
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            OnExit.Invoke();
        }
    }
}
