using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame() {
		SceneManager.LoadScene ("Scene1");
	}

	public void loadInventory() {
		SceneManager.LoadScene ("Inventory");
	}
	public void EndGame() {
		Application.Quit ();
	}

	public void Return() {
		SceneManager.LoadScene ("Menu");
	}

	public void changeWeapon() {
		string name = EventSystem.current.currentSelectedGameObject.name;
		string[] num = name.Split ('n');
		name = num [2];
		string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");

		if (lines.Length <= 6) {
			name = "0";
		}

		lines [4] = "equip " + name;

		System.IO.File.WriteAllLines ("Assets\\Conqueror\\Scripts\\save.txt", lines);
	}

	public void loadFarm() {
		SceneManager.LoadScene ("Farm");
	}

	public void updateGuns() {
		Button[] buttons = GameObject.FindGameObjectWithTag("GunPanel").GetComponentsInChildren<Button> ();
		//print (buttons [0].name);
		string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");
		int i = 0;
		foreach (Button button in buttons) {
			try {
			if (lines [i + 5].Contains ("gun")) {
				button.GetComponentInChildren<Text> ().text = lines [i + 5];
			} else {
				button.GetComponentInChildren<Text> ().text = "No Gun";
			}
			}
			catch {
				button.GetComponentInChildren<Text> ().text = "No Gun";
			}
			i++;
		}
	}

	public void deleteGun() {
		//string name = EventSystem.current.currentSelectedGameObject.name;
		//string[] num = name.Split ('n');
		//name = num [2];
		//string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");
		string[] lines = new string[6];
		//int i = 0;
		//int.TryParse (name, out i);

		//lines [5] = "";
		lines[0] = "Player";
		lines[1] = "hp 30";
		lines[2] = "damage 1";
		lines[3] = "skill 2";
		lines[4] = "equip 0";
		lines [5] = "gun1 1 150 0";
		System.IO.File.WriteAllLines ("Assets\\Conqueror\\Scripts\\save.txt", lines);
	}
}
