using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followers : MonoBehaviour {

    public List<GameObject> followers = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddFollower(GameObject new_follower)
    {
        followers.Add(new_follower);
    }

    public GameObject LastFollower()
    {
        if (followers.Count == 0)
        {
            return this.gameObject;
        }

        return followers[followers.Count - 1];
    }
}
