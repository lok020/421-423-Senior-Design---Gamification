using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
public class Gun
{
    public float damage = 1f;
    public float speed = 50;
    //type 0 = single spray
    //type 1 = double spray
    //type 2 = triple spray
    //type 3 = laser
    public int type = 2;
	public float rof = 20f;
	public float maxrof = 20f;

	public Gun() {

	}
	public Gun(float d, float s, int t) {
		damage = d;
		speed = s;
		type = t;
	}
    public void shoot ()
    {
		GameObject player = GameObject.Find ("player");
		Debug.Log(speed.ToString ());
        switch (type)
        {
		case 0:
			Vector3 mouse = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1));
			GameObject rocket = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			mouse.x -= player.transform.position.x;
			mouse.y -= player.transform.position.y;
			mouse = mouse.normalized;

			rocket.GetComponent<Rigidbody2D> ().AddForce(new Vector2 (mouse.x * speed,mouse.y * speed));
                break;
		case 1:
			Vector3 mouse2 = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1));
			GameObject rocket2 = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			GameObject rocket3 = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			mouse2.x -= player.transform.position.x;
			mouse2.y -= player.transform.position.y;
			mouse2 = mouse2.normalized;

			Vector2 mouse3 = mouse2;
                         
                mouse2.x = mouse2.x * Mathf.Cos(45);
                mouse2.y = mouse2.y * Mathf.Sin(45);
                mouse3.x = mouse3.x * Mathf.Sin(45);
                mouse3.y = mouse3.y * Mathf.Cos(45); 
                
			rocket2.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (mouse2.x * speed,mouse2.y * speed));
			rocket3.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (mouse3.x * speed,mouse3.y * speed));
                break;



            case 2:
			Vector3 mouse4 = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1));
			GameObject rocket4 = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			GameObject rocket5 = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			GameObject rocket6 = (GameObject)GameObject.Instantiate (Resources.Load ("BulletPrefab"), player.transform.position, player.transform.rotation);
			mouse4.x -= player.transform.position.x;
			mouse4.y -= player.transform.position.y;
			//mouse4 = mouse4.normalized;
			rocket6.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (mouse4.x * speed,mouse4.y * speed));
			Vector2 mouse5 = mouse4;

			mouse4.x = mouse4.x * Mathf.Cos(45);
			mouse4.y = mouse4.y * Mathf.Sin(45);
			mouse5.x = mouse5.x * Mathf.Sin(45);
			mouse5.y = mouse5.y * Mathf.Cos(45);

			rocket4.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (mouse4.x * speed,mouse4.y * speed));
			rocket5.GetComponent<Rigidbody2D> ().AddForce( new Vector2 (mouse5.x * speed,mouse5.y * speed));


                break;
            case 3:
                break;
        }
    }

}

public class GunScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
