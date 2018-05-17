using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {
	Turret turret;
	public class Turret {
		public GameObject turret;
		public int index = 0;
		public int rof = 5;
		public int maxrof = 5;
		public Turret() {
			turret = GameObject.FindGameObjectWithTag("Turret");
		}

		public void Shoot() {
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
						p.x -= turret.transform.position.x;
						p.y -= turret.transform.position.y;
						p = p.normalized;
						GameObject projectile = (GameObject)GameObject.Instantiate (Resources.Load ("BossBulletPrefab"), turret.transform.position, turret.transform.rotation);
						projectile.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (p.x * 1000, p.y * 1000));
						rof = maxrof;
					}
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		turret = new Turret ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		//turret.Shoot ();
	}
}
