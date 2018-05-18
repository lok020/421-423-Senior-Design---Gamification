using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItems : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // This script will activate when an item in the INVENTORY is clicked on.
    // Right click will be to drop item. Left click will consume consumable items.

    private InventoryManager inventory;
    public int slot_num;
    public int item_id;
    public GameObject treasure;
    public ItemManager loot;
    public StorageManager storage;

    void Start () {
        if (storage == null)
        {
            inventory = GetComponentInParent<InventoryManager>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item_id == 0)
        {
            return;
        }

        if (storage != null)
        {
            storage.DisplayDescription(item_id);
        }
        else
        {
            inventory.DisplayDescription(item_id);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (storage != null)
        {
            storage.CloseDescription();
        }
        else
        {

            inventory.CloseDescription();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClicked();

            if (item_id == 0)
            {
                inventory.CloseDescription();
            }
        }
    }

    public void LeftClicked()
    {
        if (treasure != null && treasure.activeSelf)
        {
            loot.ReturnItem(item_id, GlobalControl.Instance.full_items_list[item_id].image, slot_num);
        }
        else
        {
            if (GlobalControl.Instance.full_items_list[item_id].usable == true)
            {
                GlobalControl.Instance.UseItem(item_id);
                inventory.RemoveItem(slot_num);
            }
        }

        if (item_id == 0)
        {
            inventory.CloseDescription();
        }
    }

    public void RightClicked()
    {
        if (treasure != null)
        {
            if (!treasure.activeSelf)
            {
                inventory.RemoveItem(slot_num);
            }
        }
    }

    public void Sell()
    {
        GlobalControl.Instance.reputation += GlobalControl.Instance.full_items_list[item_id].value;
        inventory.RemoveItem(slot_num);

        if (item_id == 0)
        {
            inventory.CloseDescription();
        }
    }

    public void FromInventory()
    {
        if (item_id > 0)
        {
            inventory.ToStorage(slot_num, item_id);
        }

        if (item_id == 0)
        {
            inventory.CloseDescription();
        }
    }

    public void FromStorage()
    {
        if (item_id > 0)
        {
            storage.ToInventory(slot_num, item_id);
        }

        if (item_id == 0)
        {
            storage.CloseDescription();
        }
    }

    /*private void RemoveItem()
    {
        this.GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[0].image;
        
        if (GlobalControl.Instance.entire_inventory[slot_num] == item_id)
        {
            GlobalControl.Instance.entire_inventory[slot_num] = 0;
        }
        item_id = 0;
        return;
    }*/
}
