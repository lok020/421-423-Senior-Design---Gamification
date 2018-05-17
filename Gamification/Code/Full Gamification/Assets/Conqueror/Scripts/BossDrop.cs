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
        player.Incre.coin.active += 5;
        player.Incre.coin.passive += 5;
		//add bonus to incremental

		//add drop to player
		//player.GetComponent<PlayerMovementScript> ().player1.guns [3] = new Gun (1, 2, 2);
		if (GameObject.FindGameObjectWithTag ("Boss") != null) {
			return;
		}

		//dont award guns for the farming stage
		if (SceneManager.GetActiveScene ().name != "Farm") {

			//add stronger gun based on current level
			Gun g;
			if (SceneManager.GetActiveScene ().name == "Scene1") {
				g = new Gun (rand.Next (1, 3), rand.Next (100, 221), rand.Next (0, 1));
			} else if (SceneManager.GetActiveScene ().name == "Scene2") {
				g = new Gun (rand.Next (1, 4), rand.Next (100, 241), rand.Next (0, 2));
			} else if (SceneManager.GetActiveScene ().name == "Scene3") {
				g = new Gun (rand.Next (1, 5), rand.Next (100, 261), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene4") {
				g = new Gun (rand.Next (2, 6), rand.Next (100, 281), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene5") {
				g = new Gun (rand.Next (3, 7), rand.Next (100, 301), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene6") {
				g = new Gun (rand.Next (4, 8), rand.Next (100, 321), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene7") {
				g = new Gun (rand.Next (5, 9), rand.Next (100, 341), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene8") {
				g = new Gun (rand.Next (6, 10), rand.Next (100, 361), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene9") {
				g = new Gun (rand.Next (7, 11), rand.Next (100, 381), rand.Next (0, 3));
			} else if (SceneManager.GetActiveScene ().name == "Scene10") {
				g = new Gun (rand.Next (8, 12), rand.Next (100, 401), rand.Next (0, 3));
			} else {
				g = new Gun (rand.Next (1, 3), rand.Next (100, 221), rand.Next (0, 1));
			}
			Debug.Log ("Generating gun.");
		string gun = "gun"  + rand.Next(10).ToString() + " " + g.damage.ToString () + " " + g.speed.ToString () + " " + g.type.ToString ();
		using (System.IO.StreamWriter file = new System.IO.StreamWriter (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", true)) {
			file.WriteLine (gun);
		}
			

			SceneManager.LoadScene ("Farm");
		}



	}
}
