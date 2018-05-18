using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public int SlotNumber;

    private Image _itemImage;
    private Inventory _inventory;
    private InventoryGUI _gui;
    private NetworkManager _network;
    private Text Sstacks;

	// Use this for initialization
	void Start () {
        _gui = GetComponentInParent<InventoryGUI>();
        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        _itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        Sstacks = gameObject.transform.GetChild(1).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_inventory.inventory[SlotNumber] != null) //not empty
        {
            _itemImage.enabled = true;
            _itemImage.sprite = _inventory.inventory[SlotNumber].Sprite;
            if (_inventory.inventory[SlotNumber].Count > 1)
            {
                Sstacks.enabled = true;
                Sstacks.text = _inventory.inventory[SlotNumber].Count.ToString();
            }
            else
            {
                Sstacks.enabled = false;
            }
        }
        else
        {
            Sstacks.enabled = false;
            _itemImage.enabled = false;
        }
	}

    public void OnPointerEnter(PointerEventData data)
    {
        _gui.ExitItemOptions();
        if(_inventory.inventory[SlotNumber] != null && !_gui.isBeingDragged)
        {
            _gui.ShowToolTip(GetComponent<RectTransform>(), _inventory.inventory[SlotNumber]);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (_inventory.inventory[SlotNumber] != null)
        {
            _gui.ExitToolTip();
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        _gui.ExitToolTip();
        //Pick up item
        if(data.pointerId == -1 && _inventory.inventory[SlotNumber] != null && !_gui.isBeingDragged)
        {
            _gui.ShowDraggedItem(_inventory.inventory[SlotNumber], SlotNumber);
            _inventory.inventory[SlotNumber] = null;
        }
        //Place dragged item in empty slot
        else if (data.pointerId == -1 && _inventory.inventory[SlotNumber] == null && _gui.isBeingDragged)
        {
            _inventory.inventory[SlotNumber] = _gui.DraggedItem;
            //Update server
            _network.RemoveInventoryItem(_gui.DraggedItem.Slot, _gui.DraggedItem.Count);
            _gui.DraggedItem.Slot = SlotNumber;
            _network.AddInventoryItem(_gui.DraggedItem);
            //Clear dragged item
            _gui.DeleteDraggedItem();
        }
        //Swap dragged item with full slot
        else if (data.pointerId == -1 && _inventory.inventory[SlotNumber] != null && _gui.isBeingDragged)
        {
            Item itemBeingDragged = _gui.DraggedItem;
            Item itemInSlot = _inventory.inventory[SlotNumber];
            _inventory.inventory[_gui.DraggedSlotNumber] = itemInSlot;
            _inventory.inventory[SlotNumber] = itemBeingDragged;
            //Update server
            //Remove items
            _network.RemoveInventoryItem(itemBeingDragged.Slot, itemBeingDragged.Count);
            _network.RemoveInventoryItem(itemInSlot.Slot, itemInSlot.Count);
            //Swap slot numbers
            itemBeingDragged.Slot = SlotNumber;
            itemInSlot.Slot = _gui.DraggedSlotNumber;
            //Add items
            _network.AddInventoryItem(itemBeingDragged);
            _network.AddInventoryItem(itemInSlot);
            //Clear dragged item
            _gui.DeleteDraggedItem();
        }
        //Right click on non-empty slot
        if (data.pointerId == -2 && _inventory.inventory[SlotNumber] != null)
        {
            _gui.ShowItemOptions(_gui.itemSlots[SlotNumber].GetComponent<RectTransform>(), _inventory.inventory[SlotNumber]);
        }
    }
}
