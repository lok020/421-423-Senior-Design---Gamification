using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    public Button[] inventory_list;

    public StorageManager storage;

    public Text rep_amount;

    public GameObject description_display;
    public Text description_text;

    public delegate void DiscoveredItem();
    public static event DiscoveredItem OnDiscovery;


    void Start()
    {
        for (int x = 0; x < inventory_list.Length; x++)                     // Set slot numbers
        {
            inventory_list[x].GetComponent<InventoryItems>().slot_num = x;
            inventory_list[x].GetComponent<InventoryItems>().item_id = GlobalControl.Instance.entire_inventory[x];
            inventory_list[x].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[GlobalControl.Instance.entire_inventory[x]].image;
        }
    }

    void Update()
    {
        if (rep_amount != null)
        {
            rep_amount.text = GlobalControl.Instance.reputation.ToString() + " Rep";
        }
    }

    public int HasRoom()                                                    // Sees if there's room for items in inventory
    {
        for (int x = 0; x < inventory_list.Length; x++)
        {
            if (inventory_list[x].GetComponent<InventoryItems>().item_id == 0)  // Returns free slot number
            {
                return x;
            }
        }
        return -1;                                                         // Returns -1 if no room
    }

    public void GetItem(int slot, int id)                 // Acquire item
    {
        GlobalControl.Instance.entire_inventory[slot] = id;
        inventory_list[slot].GetComponent<InventoryItems>().item_id = id;
        inventory_list[slot].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[id].image;
        if (!GlobalControl.Instance.full_items_list[id].discovered)
        {
            GlobalControl.Instance.full_items_list[id].discovered = true;
            if (OnDiscovery != null)
            {
                OnDiscovery();
            }
        }
    }

    public void RemoveItem(int slot)
    {
        GlobalControl.Instance.entire_inventory[slot] = 0;
        inventory_list[slot].GetComponent<InventoryItems>().item_id = 0;
        inventory_list[slot].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[0].image;
    }

    public void ToStorage(int slot, int id)
    {
        if (storage == null)
        {
            return;
        }

        int x = storage.FindEmptySlot();
        if (x >= 0)
        {
            storage.GetItem(x, id);
            RemoveItem(slot);
        }
    }

    public void SaveInventory()
    {
        for (int x = 0; x < 30; x++)
        {
            GlobalControl.Instance.entire_inventory[x] = inventory_list[x].GetComponent<InventoryItems>().item_id;
        }
    }

    public void EmptyInventory()
    {
        for (int x = 0; x < 30; x++)
        {
            GlobalControl.Instance.entire_inventory[x] = 0;
        }
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
}
