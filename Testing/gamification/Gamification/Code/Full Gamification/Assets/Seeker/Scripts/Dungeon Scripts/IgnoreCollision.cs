using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Villager" || other.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(other.collider, GetComponent<Collider2D>(), true);
        }
    }
}
