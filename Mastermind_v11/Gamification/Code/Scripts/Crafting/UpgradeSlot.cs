using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public UpgradeGUI.Slot Slot;

    public InventoryGUI Inventory;
    public NetworkManager Network;
    public UpgradeGUI GUI;

    //Update flag
    private bool _updateImage = true;

    //References
    private Image _itemImage;

    // Use this for initialization
    void Start ()
    {
        _itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(_updateImage)
        {
            var item = GUI.Items[(int)Slot];
            if (item != null)
            {
                _itemImage.enabled = true;
                _itemImage.sprite = GUI.Items[(int)Slot].Sprite;
            }
            else
            {
                _itemImage.enabled = false;
            }
            _updateImage = false;
        }
	}

    public void UpdateImage()
    {
        _updateImage = true;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        GUI.ExitContextMenu();
        if(GUI.Items[(int)Slot] != null && !Inventory.isBeingDragged)
        {
            GUI.ShowTooltip(GetComponent<RectTransform>(), Slot);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (GUI.Items[(int)Slot] != null)
        {
            GUI.ExitTooltip();
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        GUI.ExitTooltip();
        Item item = GUI.Items[(int)Slot];
        //Pick up item (only works for ingredient slots)
        if (data.pointerId == -1 && Slot != UpgradeGUI.Slot.Result && item != null && !Inventory.isBeingDragged)
        {
            int emptySlot = Inventory.inventory.GetInventoryIndex(item.ID);
            Inventory.ShowDraggedItem(item, emptySlot);
            GUI.UpdateSlot(Slot, null);
        }
        //Place dragged item in empty slot
        else if (data.pointerId == -1 && Slot != UpgradeGUI.Slot.Result && item == null && Inventory.isBeingDragged)
        {
            GUI.UpdateSlot(Slot, Inventory.DraggedItem);
            //Update slot number server side
            Network.RemoveInventoryItem(Inventory.DraggedItem.Slot, Inventory.DraggedItem.Count);
            Inventory.DraggedItem.Slot = GetSlotIndex();
            Network.AddInventoryItem(Inventory.DraggedItem);
            //Clear dragged item
            Inventory.DeleteDraggedItem();
        }
        //Swap dragged item with full slot
        else if (data.pointerId == -1 && Slot != UpgradeGUI.Slot.Result && item != null && Inventory.isBeingDragged)
        {
            Item itemBeingDragged = Inventory.DraggedItem;
            Inventory.inventory.inventory[Inventory.DraggedSlotNumber] = item;      //.inventory.inventory.inventory.Inventory
            GUI.Items[(int)Slot] = itemBeingDragged;
            //Update server
            //Remove items
            Network.RemoveInventoryItem(itemBeingDragged.Slot, itemBeingDragged.Count);
            Network.RemoveInventoryItem(item.Slot, item.Count);
            //Swap slot numbers
            itemBeingDragged.Slot = GetSlotIndex();
            item.Slot = Inventory.DraggedSlotNumber;
            //Add items
            Network.AddInventoryItem(itemBeingDragged);
            Network.AddInventoryItem(item);
            //Clear dragged item
            Inventory.DeleteDraggedItem();
        }
        //Right click on non-empty slot
        if (data.pointerId == -2 && item != null)
        {
            GUI.ShowContextMenu(GetComponent<RectTransform>(), Slot);
        }
    }

    //Gets index number for slot. Used for inventory stuff
    private int GetSlotIndex()
    {
        //Upgrade = -1, consume = -2, result is ignored
        return -(int)Slot;
    }
}
