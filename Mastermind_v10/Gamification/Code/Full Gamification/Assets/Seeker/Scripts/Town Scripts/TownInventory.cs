using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownInventory : MonoBehaviour {

    public Sprite[] item_images;

    public GameObject inventory_ui;
    public int[] inventory;

	// Use this for initialization
	void Start () {
        inventory_ui = GameObject.Find("InventoryPanel");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadInventory (int[] load)
    {
        for (int x = 0; x < 30; x++)
        {
            inventory[x] = load[x];
        }
    }

    public void UpdateInventory()
    {

    }
}
