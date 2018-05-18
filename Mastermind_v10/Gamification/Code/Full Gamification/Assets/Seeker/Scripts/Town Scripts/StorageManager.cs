using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour {

    public Button[] storage_list;

    public InventoryManager inventory;

    public GameObject description_display;
    public Text description_text;

    // Use this for initialization
    void Start () {
        for (int x = 0; x < storage_list.Length; x++)
        {
            storage_list[x].GetComponent<InventoryItems>().slot_num = x;
            storage_list[x].GetComponent<InventoryItems>().item_id = GlobalControl.Instance.entire_storage_inventory[x];
            storage_list[x].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[GlobalControl.Instance.entire_storage_inventory[x]].image;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayDescription(int item_id)
    {
        if (description_display != null)
        {
            description_text.text = GlobalControl.Instance.full_items_list[item_id].name + ": " + GlobalControl.Instance.full_items_list[item_id].description + "\nSells for: " + GlobalControl.Instance.full_items_list[item_id].value + " reps.";
            description_display.SetActive(true);
        }
    }

    public void CloseDescription()
    {
        if (description_display != null)
        {
            description_display.SetActive(false);
        }
    }

    public int FindEmptySlot()
    {
        for (int x = 0; x < storage_list.Length; x++)
        {
            if (GlobalControl.Instance.entire_storage_inventory[x] == 0)
            {
                return x;
            }
        }
        return -1;
    }

    public void RemoveItem(int slot_num)
    {
        GlobalControl.Instance.entire_storage_inventory[slot_num] = 0;
        storage_list[slot_num].GetComponent<InventoryItems>().item_id = 0;
        storage_list[slot_num].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[0].image;
    }

    public void GetItem(int slot_num, int id)
    {
        GlobalControl.Instance.entire_storage_inventory[slot_num] = id;
        storage_list[slot_num].GetComponent<InventoryItems>().item_id = id;
        storage_list[slot_num].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[id].image;
    }

    public void ToInventory(int slot_num, int id)
    {
        int x = inventory.HasRoom();
        if (x >= 0)
        {
            inventory.GetItem(x, id);
            RemoveItem(slot_num);
        }
    }
}
