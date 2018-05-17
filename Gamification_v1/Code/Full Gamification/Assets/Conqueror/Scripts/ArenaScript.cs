using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ArenaScript : MonoBehaviour {
    float time = 0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if (SceneManager.GetActiveScene().name == "Farm" && time >= .5f)
        {
            GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("EnemyPrefab"), new Vector2(Random.Range(-4,4) , Random.Range(-4,4)), transform.rotation);
            Debug.Log("Enemy spawned.");
            time = 0f;
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
