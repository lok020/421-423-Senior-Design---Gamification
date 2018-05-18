using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Log : MonoBehaviour {
	//Vector2 scroll;
	// Use this for initialization
	float dps = 0f;
	float time = 0f;
	void Start () {

	}

	void OnGUI() {
		Rect window = new Rect (10, 500, Screen.width, Screen.height);
		GUILayout.BeginArea (window);
        //scroll = GUILayout.BeginScrollView (scroll);
        if (SceneManager.GetActiveScene().name != "Farm")
        {
            if (GameObject.Find("Boss"))
            {
                dps = (GameObject.Find("Boss").GetComponent<bossScript>().boss1.maxhp - GameObject.Find("Boss").GetComponent<bossScript>().boss1.hp) / time;
            }
        }
		GUIStyle style = new GUIStyle ();
		style.fontSize = 40;
		//style.fontStyle = Color.red;
		GUI.color = Color.red;
		style.normal.textColor = Color.red;
		GUILayout.Label ("DPS: " + dps.ToString(),style);

		//GUILayout.EndScrollView ();
		GUILayout.EndArea ();

	}
	// Update is called once per frame
	void FixedUpdate () {
		time += Time.deltaTime;
	}
}
