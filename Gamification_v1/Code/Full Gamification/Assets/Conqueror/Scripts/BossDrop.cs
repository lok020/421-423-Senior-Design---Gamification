using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BossDrop : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col) {
		GameObject player = GameObject.Find ("player");
		player.GetComponent<PlayerMovementScript> ().player1.guns [3] = new Gun (1, 2, 2);
		if (SceneManager.GetActiveScene ().name == "Scene1") {
				SceneManager.LoadScene ("Scene2");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene2") {
				SceneManager.LoadScene ("Scene3");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene3") {
				SceneManager.LoadScene ("Menu");
			}
	}
}
