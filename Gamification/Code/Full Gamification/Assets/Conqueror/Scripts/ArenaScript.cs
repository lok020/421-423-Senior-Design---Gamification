using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ArenaScript : MonoBehaviour {
	float time = 0f;
	float spawn = 0f;
	float stagetime = 300f;
	int rof = 20;
	int maxrof = 20;
	GameObject t;
	// Use this for initialization
	void Start () {
		t = GameObject.Find ("Time");
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		spawn += Time.deltaTime;
		stagetime -= Time.deltaTime;
		if (SceneManager.GetActiveScene ().name == "Farm") {
			t.GetComponent<Text> ().text = "Time Remaining: " + stagetime.ToString () + "s";
		}
		if (SceneManager.GetActiveScene ().name == "Farm" && stagetime <= 0f) {
			SceneManager.LoadScene ("Menu");
		}
		if (SceneManager.GetActiveScene().name == "Farm" && spawn >= 3f)
		{
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			Debug.Log("Enemy spawned.");
			spawn = 0f;

			//Turret spawning
			GameObject turret = (GameObject)GameObject.Instantiate(Resources.Load("TurretPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			Debug.Log("Turret spawned.");
		}
		if (time >= 5f) {
			GameObject drop = (GameObject)GameObject.Instantiate (Resources.Load ("BossDropPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			time = 0f;
		}
		if (SceneManager.GetActiveScene ().name == "Scene4" && spawn >= 5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene5" && spawn >= 2.5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene6" && spawn >= 2f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene7" && spawn >= 1.5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene8" && spawn >= 1f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			GameObject enemy2 = (GameObject)GameObject.Instantiate(Resources.Load("TurretPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene9" && spawn >= 1f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			GameObject enemy2 = (GameObject)GameObject.Instantiate(Resources.Load("TurretPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		else if (SceneManager.GetActiveScene ().name == "Scene10" && spawn >= .5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			GameObject enemy2 = (GameObject)GameObject.Instantiate(Resources.Load("TurretPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}



		//make turrets shoot
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Turret");
		Vector3 mouse = GameObject.Find ("player").transform.position;
		for (int i = 0; i < enemies.Length; i++) {
			if (enemies[i]) {
				mouse.x -= enemies[i].transform.position.x;
				mouse.y -= enemies[i].transform.position.y;
				mouse = mouse.normalized;
				enemies [i].GetComponent<Rigidbody2D> ().AddForce (new Vector2 (mouse.x * 5, mouse.y * 5));
				rof--;
				if (rof <= 0) {
					Vector3 p = GameObject.Find ("player").transform.position;
					p.x -= enemies[i].transform.position.x;
					p.y -= enemies[i].transform.position.y;
					p = p.normalized;
					GameObject projectile = (GameObject)GameObject.Instantiate (Resources.Load ("BossBulletPrefab"), enemies[i].transform.position, enemies[i].transform.rotation);
					projectile.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (p.x * 1000, p.y * 1000));
					rof = maxrof;
				}
			}
		}
	}
	void OnCollisionEnter2D(Collision2D col) {
		if (SceneManager.GetActiveScene().name == "Scene3" && col.gameObject.tag == "BossBullet") {
			GameObject rocket = (GameObject)GameObject.Instantiate (Resources.Load ("BossBulletPrefab"), col.gameObject.transform.position, col.gameObject.transform.rotation);
			rocket.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (Random.Range (-4, 4) * 150, Random.Range (-4, 4) * 150));
		}
		if (col.gameObject.tag == "Bullet" || col.gameObject.tag == "BossBullet")
			Destroy (col.gameObject);
		//if (col.gameObject.tag == "Enemy") {
		//	Destroy(col.gameObject);
		//}
	}
}
