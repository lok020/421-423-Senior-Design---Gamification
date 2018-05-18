using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    
    public Button[] inventory_list;
    public int[] inventory_items;

    public Text rep_amount;

	// Use this for initialization
	void Start () {
        int x = 0;

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
            inventory_list[x].GetComponent<InventoryItems>().slot_num = x;
            inventory_list[x].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[inventory_items[x]].image;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (rep_amount != null)
        {
            rep_amount.text = GlobalControl.Instance.reputation.ToString() + " Rep";
        }
	}

    public void RemoveItem(int slot_num)
    {
        inventory_items[slot_num] = 0;
        inventory_list[slot_num].GetComponent<InventoryItems>().item_id = 0;
        inventory_list[slot_num].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[0].image;
    }

    public void GetItem(int slot_num, int id)
    {
        inventory_items[slot_num] = id;
        inventory_list[slot_num].GetComponent<InventoryItems>().item_id = id;
        inventory_list[slot_num].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[id].image;
    }

    public void ExitScene()
    {
        GlobalControl.Instance.entire_inventory = inventory_items;
    }
}