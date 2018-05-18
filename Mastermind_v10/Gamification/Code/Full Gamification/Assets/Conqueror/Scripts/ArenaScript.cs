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
        if (SceneManager.GetActiveScene().name == "Farm" && spawn >= 5f)
        {
            GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
            Debug.Log("Enemy spawned.");
            spawn = 0f;
        }
		if (time >= 5f) {
			GameObject drop = (GameObject)GameObject.Instantiate (Resources.Load ("BossDropPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			time = 0f;
		}
		if (SceneManager.GetActiveScene ().name == "Scene4" && spawn >= 5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
		}
		if (SceneManager.GetActiveScene ().name == "Scene5" && spawn >= 2.5f) {
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-8,8) , Random.Range(-8,8)), transform.rotation);
			spawn = 0f;
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
