using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {

    public Sprite[] item_images;
    public Button[] inventory_list;
    public int[] inventory_items;
    private GameObject is_in_shop;

	// Use this for initialization
	void Start () {
        int x = 0;

        is_in_shop = GameObject.Find("Shop");

        inventory_items = GlobalControl.Instance.entire_inventory;


        if (inventory_items.Length > 0)                                     // Set items
        {
            for (x = 0; x < inventory_items.Length; x++)
            {
                inventory_list[x].GetComponent<InventoryItems>().item_id = inventory_items[x];
            }
        }

        for (x = 0; x < inventory_list.Length; x++)                 // Sets images, slot numbers, and enable or disables selling.
        {
            if (is_in_shop != null)
            {
                inventory_list[x].GetComponent<InventoryItems>().in_shop = true;
            }
            else
            {
                inventory_list[x].GetComponent<InventoryItems>().in_shop = false;
            }
            inventory_list[x].GetComponent<InventoryItems>().slot_num = x;
            inventory_list[x].GetComponent<Image>().sprite = item_images[inventory_list[x].GetComponent<InventoryItems>().item_id];
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveItem(int slot_num)
    {
        inventory_items[slot_num] = 0;
        inventory_list[slot_num].GetComponent<InventoryItems>().item_id = 0;
        inventory_list[slot_num].GetComponent<Image>().sprite = item_images[0];
    }

    public void GetItem(int slot_num, int id)
    {
        inventory_items[slot_num] = id;
        inventory_list[slot_num].GetComponent<InventoryItems>().item_id = id;
        inventory_list[slot_num].GetComponent<Image>().sprite = item_images[id];
    }

    public void ExitScene()
    {
        GlobalControl.Instance.entire_inventory = inventory_items;
    }
}