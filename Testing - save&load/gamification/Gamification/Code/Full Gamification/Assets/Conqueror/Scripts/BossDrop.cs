using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BossDrop : MonoBehaviour {
	System.Random rand;
	// Use this for initialization
	void Start () {
		rand = new System.Random ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col) {
		//GameObject play = GameObject.Find ("player");
		player.coin.active += 5;
		player.coin.passive += 5;
		//add bonus to incremental

		//add drop to player
		//player.GetComponent<PlayerMovementScript> ().player1.guns [3] = new Gun (1, 2, 2);
		if (GameObject.FindGameObjectWithTag ("Boss") != null) {
			return;
		}

		Gun g = new Gun (rand.Next (1, 6), rand.Next (100, 400), rand.Next (0, 3));

		string gun = "gun"  + rand.Next(10).ToString() + " " + g.damage.ToString () + " " + g.speed.ToString () + " " + g.type.ToString ();
		using (System.IO.StreamWriter file = new System.IO.StreamWriter ("Assets\\Conqueror\\Scripts\\save.txt", true)) {
			file.WriteLine (gun);
		}

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
