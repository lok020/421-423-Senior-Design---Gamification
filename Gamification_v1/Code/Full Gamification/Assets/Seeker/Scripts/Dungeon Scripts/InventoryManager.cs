using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    public Button[] inventory_list;

    void Start()
    {
        for (int x = 0; x < inventory_list.Length; x++)                     // Set slot numbers
        {
            inventory_list[x].GetComponent<InventoryItems>().slot_num = x;
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

    public void GetItem(int slot, int id, Sprite new_image)                 // Acquire item from chest
    {
        inventory_list[slot].GetComponent<InventoryItems>().item_id = id;
        inventory_list[slot].GetComponent<Image>().sprite = new_image;
    }


    public void SaveInventory()
    {
        for (int x = 0; x < 30; x++)
        {
            GlobalControl.Instance.entire_inventory[x] = inventory_list[x].GetComponent<InventoryItems>().item_id;
        }
    }
}
