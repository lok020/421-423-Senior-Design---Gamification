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
		//make text file if it does not exist
		Debug.Log(System.IO.Directory.GetCurrentDirectory());
		if (!File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\save.txt")) {
			using (StreamWriter sw = File.CreateText (System.IO.Directory.GetCurrentDirectory () + "\\save.txt")) {
				sw.WriteLine ("Player");
				sw.WriteLine ("hp 60");
				sw.WriteLine ("damage .3");
				sw.WriteLine ("skill 2");
				sw.WriteLine ("equip 0");
				sw.WriteLine ("gun1 1 150 0");
			}
		}
		if (SceneManager.GetSceneByName ("Inventory") == SceneManager.GetActiveScene ()) {
			//remove later, for testing only
			player.stamina.cur += 1;
			//
			updateGuns ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame() {
		//use up stamina
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene1");
		}
	}
	public void StageSelect() {
		
		SceneManager.LoadScene ("StageSelect");
	}
	public void Stage1() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene1");
		}
	}
	public void Stage2() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene2");
		}
	}
	public void Stage3() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene3");
		}
	}
	public void Stage4() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene4");
		}
	}
	public void Stage5() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene5");
		}
	}
	public void Stage6() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene6");
		}
	}
	public void Stage7() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene7");
		}
	}
	public void Stage8() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene8");
		}
	}
	public void Stage9() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene9");
		}
	}
	public void Stage10() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Scene10");
		}
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
		Debug.Log ("HITTING CHANGE WEAPON");
		string name = EventSystem.current.currentSelectedGameObject.name;
		Debug.Log (name);
		string[] num = name.Split ('n');
		name = num [2];
		Debug.Log (name);

		GameObject g = EventSystem.current.currentSelectedGameObject;
		Debug.Log (g.GetComponentInChildren<Text> ().text);
		//Debug.Log (EventSystem.current.currentSelectedGameObject.GetComponent<Text> ().text);
		if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text> ().text.Contains ("No")) {
			GameObject obj = GameObject.Find("ActiveGunNotification");
			obj.GetComponent<Text> ().text = "You don't have a gun there.";
			return;
		}

		//if (!EventSystem.current.currentSelectedGameObject.GetComponent<Text> ().text.Contains ("No")) {
			//equip gun notification
			GameObject obj2 = GameObject.Find("ActiveGunNotification");
			obj2.GetComponent<Text> ().text = "Equipped Gun " + num[2];
		//}

		//string[] lines = System.IO.File.ReadAllLines("Assets\\Conqueror\\Scripts\\save.txt");
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		//if (lines.Length == 0) {
			//lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		//}
		if (lines.Length <= 6) {
			name = "0";
		}

		lines [4] = "equip " + name;

		System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines);
	}

	public void loadFarm() {
		if (player.stamina.cur > 0) {
			player.stamina.cur -= 1;
			SceneManager.LoadScene ("Farm");
		}
	}

	public void updateGuns() {
		Button[] buttons = GameObject.FindGameObjectWithTag("GunPanel").GetComponentsInChildren<Button> ();
		//print (buttons [0].name);
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		int i = 0;

		//ensures there is never an invalid save file
		if (lines.Length <= 5) {

			string[] lines2 = new string[6];
			lines2[0] = "Player";
			lines2[1] = "hp 60";
			lines2[2] = "damage .3";
			lines2[3] = "skill 2";
			lines2[4] = "equip 0";
			lines2 [5] = "gun1 1 150 0";
			System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines2);
			lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		}

		foreach (Button button in buttons) {
			try {
			if (lines [i + 5].Contains ("gun")) {
					string[] split = lines[i+5].Split(' ');
				//button.GetComponentInChildren<Text> ().text = lines [i + 5];
					int num = int.Parse(split[1]);
					int num2 = int.Parse(split[2]);
					int result = num * num2;
					button.GetComponentInChildren<Text> ().text = split [0] + " Damage: " + result.ToString();
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
		GameObject text = GameObject.Find ("GunText");
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		int i = 0;
		List<string> list = new List<string>(lines);
		foreach (string line in lines) {
			//if (text.GetComponentInChildren<Text> ().text.Contains ("gun")) {
			Debug.Log(text.GetComponentsInChildren<Text>()[1].text);
			if (line.Contains(text.GetComponentsInChildren<Text>()[1].text)) {
				if (i != 5) {
					list.RemoveAt (i);
				}
				}
			i++;
		}
		lines = list.ToArray ();
		/*
		string[] lines = new string[6];
		lines[0] = "Player";
		lines[1] = "hp 60";
		lines[2] = "damage .3";
		lines[3] = "skill 2";
		lines[4] = "equip 0";
		lines [5] = "gun1 1 150 0";
		*/

		System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines);
		//updateGuns ();
	}

	public void SetSkill() {
		GameObject s = GameObject.Find ("Change Skill");
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		lines [3] = "skill 0";
		System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines);
	}
	public void SetSkill2() {
		GameObject s = GameObject.Find ("Change Skill2");
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		lines [3] = "skill 1";
		System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines);
	}
	public void SetSkill3() {
		GameObject s = GameObject.Find ("Change Skill3");
		string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\save.txt");
		lines [3] = "skill 2";
		System.IO.File.WriteAllLines (System.IO.Directory.GetCurrentDirectory() + "\\save.txt", lines);
	}
}
