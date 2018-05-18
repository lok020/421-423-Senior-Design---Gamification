using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss
{
    public float hp = 10f;
	public float maxhp = 10f;
	public float maxrof = 8f;
    public float rof = 8f;
	public int index = 1;
	public float movespeed = 5f;
	public GameObject boss;
    public Boss()
    {


    }
	public void Move() {
		boss = GameObject.Find ("Boss");
		float movex = Input.GetAxis ("Horizontal");
		float movey = Input.GetAxis ("Vertical");
		boss.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (movex * movespeed, movey * movespeed));
		if (boss.GetComponent<Rigidbody2D> ().velocity.magnitude > 5) {
			boss.GetComponent<Rigidbody2D> ().velocity = boss.GetComponent<Rigidbody2D> ().velocity.normalized * 5;
		}
	}

	public void Shoot() {
		boss = GameObject.Find ("Boss");
		switch(index) {
		case 0:
			boss = GameObject.Find ("Boss");
			rof--;
			if (rof <= 0) {
				GameObject rocket = (GameObject)GameObject.Instantiate (Resources.Load ("BossBulletPrefab"), boss.transform.position, boss.transform.rotation);
				rocket.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (Random.Range (-4, 4) * 150, Random.Range (-4, 4) * 150));
				rof = maxrof;
			}
			break;
		case 1:
			rof--;
			if (rof <= 0) {
				Vector3 mouse = GameObject.Find ("player").transform.position;
				mouse.x -= boss.transform.position.x;
				mouse.y -= boss.transform.position.y;
				mouse = mouse.normalized;
				GameObject rocket2 = (GameObject)GameObject.Instantiate (Resources.Load ("BossBulletPrefab"), boss.transform.position, boss.transform.rotation);
				rocket2.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (mouse.x * 1000, mouse.y * 1000));
				rof = maxrof;
			}
			break;
		}
	}
}
public class bossScript : MonoBehaviour {

    // Use this for initialization

    GameObject rocket;
	GameObject boss;
	GameObject player;
    public Boss boss1 = new Boss();
	float counter = 0f;
	void Start () {
		if (SceneManager.GetActiveScene ().name == "Scene2") {
			boss1.hp = 20;
			boss1.maxhp = 20;
			boss1.maxrof = 4;
			boss1.rof = 4;
		} else if (SceneManager.GetActiveScene ().name == "Scene3") {
			boss1.hp = 50;
			boss1.maxhp = 50;
			boss1.maxrof = 2;
			boss1.rof = 2;
		}
		GameObject bosshp = GameObject.Find ("BossHP");
		bosshp.GetComponent<Slider> ().minValue = 0;
		bosshp.GetComponent<Slider> ().maxValue = boss1.hp;
		bosshp.GetComponent<Slider> ().value = boss1.hp;
		boss = GameObject.Find("Boss");
		player = GameObject.Find ("player");
	}


	// Update is called once per frame
	void FixedUpdate () {
		counter += Time.deltaTime;
		if (counter >= 10f) {

		}
		if (counter >= 3f) {
			if (boss1.index == 0) {
				boss1.index = 1;
			} else {
				boss1.index = 0;
			}
			counter = 0f;
		}
		boss1.Shoot();
	}

	void OnCollisionEnter2D(Collision2D col) {

		//spawn enemy



		if (SceneManager.GetActiveScene ().name == "Scene2") {
		Vector3 mouse = player.transform.position;
		mouse.x -= boss.transform.position.x;
		mouse.y -= boss.transform.position.y;
		mouse = mouse.normalized;

			GameObject rocket2 = (GameObject)GameObject.Instantiate (Resources.Load ("EnemyPrefab"), boss.transform.position, boss.transform.rotation);

		boss.GetComponent<Rigidbody2D> ().AddForce(new Vector2 (mouse.x * 250,mouse.y * 250));
		}
		if (col.gameObject.name == "Arena")
			return;
		if (col.gameObject.name == "BossBulletPrefab")
			return;
		if (col.gameObject.name == "player") {
			SceneManager.LoadScene ("Menu");
			return;
		}
		//hp--;
		GameObject bosshp = GameObject.Find ("BossHP");
		bosshp.GetComponent<Slider> ().value = boss1.hp;
		Destroy (col.gameObject);
		if (boss1.hp <= 0) {
		GameObject drop = (GameObject)GameObject.Instantiate (Resources.Load ("BossDropPrefab"), boss.transform.position, boss.transform.rotation);
			/*
            string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");
            int enemies;
            int.TryParse(lines[9].Split(' ')[1], out enemies);
            enemies++;
            lines[9] = "enemieskilled " + enemies.ToString();
            System.IO.File.WriteAllLines("Assets\\Conqueror\\Scripts\\save.txt", lines);
			*/
            Destroy(GameObject.Find("Boss"));



			/*if (SceneManager.GetActiveScene ().name == "Scene1") {
				SceneManager.LoadScene ("Scene2");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene2") {
				SceneManager.LoadScene ("Scene3");
			}
			else if (SceneManager.GetActiveScene ().name == "Scene3") {
				SceneManager.LoadScene ("Menu");
			}*/
		print ("COLLISION");
		}
	}
}
