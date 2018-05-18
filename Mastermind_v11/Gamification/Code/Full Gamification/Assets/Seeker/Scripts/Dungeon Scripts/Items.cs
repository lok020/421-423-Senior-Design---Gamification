using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Items : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public int slot_num;                // The slot where the item is in
    public int item_id;                 // Item ID
    public TreasureManager tied_chest;  // The chest that loot inventory is showing
    public ItemManager lootbox;

    void Start()
    {
        lootbox = GetComponentInParent<ItemManager>();
    }

    public void clicked()               // Activates when item slot is clicked
    {
        if (item_id == 0)               //If there's no item in this slot, do nothing
        {
            return;
        }
        tied_chest.TakeItem(slot_num);

        if (item_id == 0)
        {
            lootbox.CloseDescription();
        }
    }

    public void ReceivedItem(int id, int from_slot)
    {
        item_id = id;
        tied_chest.PlaceItem(id, slot_num, from_slot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item_id == 0)
        {
            return;
        }

        lootbox.DisplayDescription(item_id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lootbox.CloseDescription();
    }
}
