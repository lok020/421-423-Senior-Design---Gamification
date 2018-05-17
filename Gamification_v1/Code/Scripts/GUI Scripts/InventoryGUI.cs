using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour {

    public List<GameObject> itemSlots = new List<GameObject>();
    public GameObject slots;
    public GameObject tooltip;
    public GameObject itemOptions;
    public Text itemName;
    public Text itemStats;
    public Text option;
    public Text upgrade;
    public Text discard;
    private float x = 0;
    private float y = 0;
    public GameObject ItemDraggedIcon;
    public bool isBeingDragged = false;
    public int DraggedSlotNumber;
    private int scaledWidth;
    private int scaledHeight;
    public Inventory inventory;
    public Item DraggedItem;

    private Item _tempItem;

    //References
    private GameObject _player;


	void Start () 
    {
        int slotNumber = 0;
        _player = GameObject.FindGameObjectWithTag("Player");
        inventory = _player.GetComponent<Inventory>();
        scaledWidth = (int)(GetComponent<RectTransform>().rect.width - 30) / 5;
        scaledHeight = (int)(GetComponent<RectTransform>().rect.height - 30) / 5;
        x -= GetComponent<RectTransform>().rect.width / 2;
        y += GetComponent<RectTransform>().rect.height / 2;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                GameObject slot = Instantiate(slots);
                slot.GetComponent<Slot>().SlotNumber = slotNumber;
                itemSlots.Add(slot);
                slot.transform.SetParent(this.gameObject.transform, false);
                slot.name = "Slot " + (j + (5 * i));
                slot.GetComponent<RectTransform>().sizeDelta = new Vector3(scaledWidth, scaledHeight, 0);
                slot.GetComponent<RectTransform>().localPosition = new Vector3(x + 5 + slot.GetComponent<RectTransform>().rect.width / 2, y - 5 - slot.GetComponent<RectTransform>().rect.height / 2, 0);
                x += (5 + scaledWidth);
                slotNumber++;
                if (j == 5)
                {
                    x = 0 - GetComponent<RectTransform>().rect.width / 2;
                    y -= (5 + scaledHeight);
                }
            }
        }
	}

    void Update()
    {
        if (isBeingDragged)
        {
            //Vector3 position = (Input.mousePosition - GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<RectTransform>().localPosition);

            Canvas canvas = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<Canvas>();
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            ItemDraggedIcon.transform.position = canvas.transform.TransformPoint(pos);
        }
    }

    public void ShowToolTip(RectTransform slotTransform, Item item)
    {
        itemName.text = item.Name;

        itemStats.text = item.GetTooltip();

        if(itemStats.text == "")
        {
            itemStats.enabled = false;
        }
        else
        {
            itemStats.enabled = true;
        }
        
        var local = GetComponent<RectTransform>().localPosition;
        var tRect = tooltip.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        tooltip.SetActive(true);
    }

    public void ExitToolTip()
    {
        tooltip.SetActive(false);
        itemName.text = "";
        itemStats.text = "";
    }

    public void ShowItemOptions(RectTransform slotTransform, Item item)
    {
        _tempItem = item;
        if(item.ItemType == Item.Type.Gear) //equip gear
        {
            option.text = "Equip";
        }
        else if(item.ItemType == Item.Type.Consumable) //consume item
        {
            option.text = "Consume";
        }
        //If item can be upgraded
        if(item.ItemType == Item.Type.Gear && item.Tier > 0 && item.Quality < Item.UpgradeCapByTier[item.Tier])
        {
            upgrade.enabled = true;
        }
        else
        {
            upgrade.enabled = false;
        }
        //Hide option text entirely if there is none (fixes spacing issue)
        if(option.text == "")
        {
            option.enabled = false;
        }
        else
        {
            option.enabled = true;
        }
        //Remove the "discard" option for quest items
        if(item.NotDisposable)
        {
            discard.enabled = false;
        }
        else
        {
            discard.enabled = true;
        }


        var local = GetComponent<RectTransform>().localPosition;
        var tRect = itemOptions.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        itemOptions.SetActive(true);
    }

    public void ExitItemOptions()
    {
        itemOptions.SetActive(false);
        option.text = "";
    }

    public void EquipOrConsumeItem()
    {
        if (option.text == "Equip")
        {
            _player.GetComponent<PlayerController>().Equip(_tempItem);
        }
        else if (option.text == "Consume")
        {
            _player.GetComponent<PlayerController>().Consume(_tempItem);
            _player.GetComponent<Inventory>().RemoveItemFromInventory(_tempItem.ID, 1);
            //Notify the event manager
            _player.GetComponent<EventManager>().Event(new List<object>() { GameAction.CONSUMED, _tempItem.ID, _tempItem.Count });
        }
        ExitItemOptions();
    }

    //Opens item menu, doesn't actually upgrade item
    public void UpgradeItem()
    {
        var upgradeUI = transform.parent.parent.GetComponent<GUIManager>().UpgradeUI;
        upgradeUI.SetActive(true);
        upgradeUI.GetComponentInChildren<UpgradeGUI>().UpdateSlot(UpgradeGUI.Slot.Upgrade, _tempItem);
        ExitItemOptions();
    }

    public void DiscardItem()
    {
        _player.GetComponent<Inventory>().RemoveItemFromInventory(_tempItem);
        //Notify the event manager
        _player.GetComponent<EventManager>().Event(new List<object>() { GameAction.DISCARDED, _tempItem.ID, _tempItem.Count });
        ExitItemOptions();
    }

    public void ShowDraggedItem(Item item, int number)
    {
        ExitToolTip();
        DraggedSlotNumber = number;
        DraggedItem = item;
        ItemDraggedIcon.GetComponent<Image>().sprite = item.Sprite;
        ItemDraggedIcon.GetComponent<RectTransform>().sizeDelta = new Vector3((2 * scaledWidth) / 3, (2 * scaledHeight) / 3);
        ItemDraggedIcon.GetComponent<RectTransform>().SetAsLastSibling();
        ItemDraggedIcon.SetActive(true);
        isBeingDragged = true;
    }

    public void DeleteDraggedItem()
    {
        isBeingDragged = false;
        ItemDraggedIcon.SetActive(false);
        DraggedItem = null;
    }

    public void ReturnItemToInventory()
    {
        inventory.inventory[DraggedSlotNumber] = DraggedItem;
        DeleteDraggedItem();
    }
}
