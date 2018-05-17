using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItems : MonoBehaviour
{
    // This script will activate when an item in the INVENTORY is clicked on.
    // Right click will be to drop item. Left click will consume consumable items.

    public int slot_num;
    public int item_id;
    public bool in_shop;     //Allow selling


    public void LeftClicked()
    {
    }

    public void RightClicked()
    {

    }

}
