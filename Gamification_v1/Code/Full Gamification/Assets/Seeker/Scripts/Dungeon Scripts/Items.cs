using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour {

    public int slot_num;                // The slot where the item is in
    public int item_id;                 // Item ID
    public TreasureManager tied_chest;  // The chest that loot inventory is showing


    public void clicked()               // Activates when item slot is clicked
    {
        if (item_id == 0)               //If there's no item in this slot, do nothing
        {
            return;
        }
        tied_chest.TakeItem(slot_num);
    }

}
