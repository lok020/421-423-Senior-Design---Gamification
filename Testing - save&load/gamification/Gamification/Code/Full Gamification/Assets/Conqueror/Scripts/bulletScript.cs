using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour {

	// Use this for initialization
	GameObject bullet;
	GameObject player;
	void Start () {
		player = GameObject.Find ("player");
	}
	
	// Update is called once per frame
	void Update () {
		bullet = GameObject.FindWithTag("Bullet");
		//if (Vector3.Distance (player.transform.position, bullet.transform.position) > 5) {
		/*if(bullet.GetComponent<Rigidbody2D>().velocity.x <= .5f && bullet.GetComponent<Rigidbody2D>().velocity.y <= .5f){
			Destroy (bullet);
		}*/
	}

	void OnCollisionEnter2D(Collision2D col) {
		//if (col.gameObject.tag == "Bullet")
			//Destroy (col.gameObject);
		if (col.gameObject.tag == "BossBullet")
			Destroy (col.gameObject);
        if (col.gameObject.name == "Boss")
        {
			//GameObject r = (GameObject)GameObject.Instantiate(Resources.Load ("BossTextPrefab"), GameObject.Find("Boss").transform.position, GameObject.Find("Boss").transform.rotation);
			//r.transform.SetParent (GameObject.Find ("Canvas").transform);
            GameObject.Find("Boss").GetComponent<bossScript>().boss1.hp -= player.GetComponent<PlayerMovementScript>().player1.g.damage;
        }
		if (col.gameObject.tag == "Enemy") {
			/*Vector3 mouse = GameObject.Find ("player").transform.position;
			mouse.x -= col.gameObject.transform.position.x;
			mouse.y -= col.gameObject.transform.position.y;
			mouse = mouse.normalized;
			col.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (mouse.x * 20, mouse.y * 20)); */

			//string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");
			//int enemies;
			//float damage;
			//int.TryParse(lines[8].Split(' ')[1], out enemies);
			//float.TryParse(lines[2].Split(' ')[1], out damage);
			//enemies++;
			//damage += .01f;
			//lines[8] = "enemieskilled " + enemies.ToString();
			//lines[2] = "damage " + damage.ToString();
			//System.IO.File.WriteAllLines("Assets\\Conqueror\\Scripts\\save.txt",lines);

			Destroy (col.gameObject);
		}
	}
}
