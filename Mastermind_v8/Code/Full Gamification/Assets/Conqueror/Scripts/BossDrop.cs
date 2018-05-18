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

		//dont award guns for the farming stage
		if (SceneManager.GetActiveScene ().name != "Farm") {
		Gun g = new Gun (rand.Next (1, 4), rand.Next (100, 301), rand.Next (0, 3));

		string gun = "gun"  + rand.Next(10).ToString() + " " + g.damage.ToString () + " " + g.speed.ToString () + " " + g.type.ToString ();
		using (System.IO.StreamWriter file = new System.IO.StreamWriter (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", true)) {
			file.WriteLine (gun);
		}
			

			SceneManager.LoadScene ("Farm");
		}
		/*
		if (SceneManager.GetActiveScene ().name == "Scene1") {
				SceneManager.LoadScene ("Scene2");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene2") {
				SceneManager.LoadScene ("Scene3");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene3") {
				SceneManager.LoadScene ("Scene4");
			}
		else if (SceneManager.GetActiveScene ().name == "Scene4") {
			SceneManager.LoadScene ("Scene5");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene5") {
			SceneManager.LoadScene ("Scene6");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene6") {
			SceneManager.LoadScene ("Scene7");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene7") {
			SceneManager.LoadScene ("Scene8");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene8") {
			SceneManager.LoadScene ("Scene9");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene9") {
			SceneManager.LoadScene ("Scene10");
		}
		else if (SceneManager.GetActiveScene ().name == "Scene10") {
			SceneManager.LoadScene ("Menu");
		}

	*/}
}
