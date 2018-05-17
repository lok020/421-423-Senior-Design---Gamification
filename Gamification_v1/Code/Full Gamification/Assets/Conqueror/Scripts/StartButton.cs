using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
		GameObject LocationPickerLabel = GameObject.Find ("Dropdown");
		GameObject itemd = GameObject.Find ("ItemDescription");

		string[] lines = System.IO.File.ReadAllLines("AssetsConqueror\\Scripts\\save.txt");
		lines [4] = "equip " + (LocationPickerLabel.GetComponent<Dropdown>().value).ToString();

		string[] guns = lines [LocationPickerLabel.GetComponent<Dropdown> ().value + 5].Split (' ');
		itemd.GetComponent<Text> ().text = "Item Details\n Weapon " + 
			(LocationPickerLabel.GetComponent<Dropdown>().value + 1).ToString() + 
		"\nType: " + guns[1] + "\nDamage: " + guns[2] + "\nBullet Speed: " + guns[3];
		System.IO.File.WriteAllLines ("AssetsConqueror\\Scripts\\save.txt", lines);
	}

	public void loadFarm() {
		SceneManager.LoadScene ("Farm");
	}
}
