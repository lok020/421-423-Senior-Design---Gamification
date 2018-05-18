using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UpgradeGUI : MonoBehaviour {

    //Slots
    public GameObject UpgradeSlot;
    public GameObject ConsumeSlot;
    public GameObject ResultSlot;

    //Upgrade button
    public GameObject UpgradeButton;

    //Info
    public GameObject Info;

    //Tooltip stuff
    public GameObject Tooltip;
    public GameObject ContextMenu;

    //Slot contents
    public Item[] Items = new Item[3];

    //Enum for identifying slots
    public enum Slot { Upgrade, Consume, Result };

    //Private variables
    private bool _canUpgrade = false;       //Indicates if base item can be upgraded
    private bool _firstOpen = false;

    //References
    private InventoryGUI _inventoryGUI;
    private NetworkManager _network;
    private Text _infoText;
    private Text _contextMenuText;
    private Text _tooltipName, _tooltipText;

	// Use this for initialization
	void Start ()
    {
        _inventoryGUI = transform.parent.parent.gameObject.GetComponentsInChildren<InventoryGUI>(true)[0];
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();

        _infoText = Info.GetComponentInChildren<Text>();
        _contextMenuText = ContextMenu.GetComponentsInChildren<Text>(true)[0];
        foreach(Text t in Tooltip.GetComponentsInChildren<Text>(true))
        {
            if (t.name == "Name")
                _tooltipName = t;
            else if (t.name == "Stats")
                _tooltipText = t;
        }

        var upgradeSlot = UpgradeSlot.GetComponent<UpgradeSlot>();
        var consumeSlot = ConsumeSlot.GetComponent<UpgradeSlot>();
        var resultSlot = ResultSlot.GetComponent<UpgradeSlot>();

        upgradeSlot.Inventory = _inventoryGUI;
        consumeSlot.Inventory = _inventoryGUI;
        resultSlot.Inventory = _inventoryGUI;
        upgradeSlot.Network = _network;
        consumeSlot.Network = _network;
        resultSlot.Network = _network;
        upgradeSlot.GUI = this;
        consumeSlot.GUI = this;
        resultSlot.GUI = this;

        _firstOpen = true;
        UpdateInfo();
    }

    void Update()
    {
        UpdateInfo();
    }

    //Show craftable item
    public void UpdateCraftableItem()
    {
        //Can only show if first slot is occupied
        if (Items[(int)Slot.Upgrade] == null) return;
        //Clone item
        Item resultItem = Instantiate(Items[(int)Slot.Upgrade].gameObject).GetComponent<Item>();
        //Upgrade cloned item
        _canUpgrade = resultItem.UpgradeQuality();
        //Delete existing crafted item (if there is one)
        if(Items[(int)Slot.Result] != null)
        {
            Destroy(Items[(int)Slot.Result]);
        }
        //Set item to null if craft failed or to new item if it succeeded
        Items[(int)Slot.Result] = resultItem;
        //Update slot
        ResultSlot.GetComponent<UpgradeSlot>().UpdateImage();
        //Update info
        UpdateInfo();
    }

    //Show tooltip
    public void ShowTooltip(RectTransform slotTransform, Slot slot)
    {
        Item item = Items[(int)slot];

        _tooltipName.text = item.Name;
        if (item.Quality > 0) _tooltipName.text += " +" + item.Quality;

        //Only hide last enchantment if it adds an enchantment
        if(slot == Slot.Result && item.Quality % 2 == 1)
        {
            _tooltipText.text = item.GetTooltip(true);
        }
        else
        {
            _tooltipText.text = item.GetTooltip(false);
        }

        var local = GetComponent<RectTransform>().localPosition;
        var tRect = Tooltip.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        Tooltip.SetActive(true);
    }

    //Show context menu
    public void ShowContextMenu(RectTransform slotTransform, Slot slot)
    {
        //Remove first and second slot items
        if(slot != Slot.Result)
        {
            _contextMenuText.text = "Remove";
        }
        //Craft third item
        else
        {
            _contextMenuText.text = "Craft";
        }

        var local = GetComponent<RectTransform>().localPosition;
        var tRect = ContextMenu.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        ContextMenu.SetActive(true);
    }

    public void ReturnItemsToInventory()
    {
        //Return items to inventory
        foreach(Item i in Items)
        {
            if (i == null) continue;
            _inventoryGUI.inventory.AddItemToInventory(i);
        }
        //Reset flag
        _firstOpen = false;
    }

    //Hide tooltip
    public void ExitTooltip()
    {
        Tooltip.SetActive(false);
        _tooltipName.text = "";
        _tooltipText.text = "";
    }

    //Hide context menu
    public void ExitContextMenu()
    {
        ContextMenu.SetActive(false);
        _contextMenuText.text = "";
    }

    //Updates slot, used for upgrade and consume slots. Result slot is updated by UpdateCraftableItem()
    public void UpdateSlot(Slot slot, Item item)
    {
        Items[(int)slot] = item;

        //Update crafted item slot if first slot was changed
        if (slot == Slot.Upgrade)
        {
            UpdateCraftableItem();
        }
        //Update slot icon
        switch(slot)
        {
            case Slot.Upgrade:
                UpgradeSlot.GetComponent<UpgradeSlot>().UpdateImage();
                break;
            case Slot.Consume:
                ConsumeSlot.GetComponent<UpgradeSlot>().UpdateImage();
                break;
            case Slot.Result:
                ResultSlot.GetComponent<UpgradeSlot>().UpdateImage();
                break;
        }
        //Update info text
        UpdateInfo();
    }

    //Update info
    public void UpdateInfo()
    {
        //Get references if they are null
        if (_network == null) Start();
        //Sets text, in priority. Errors are most important
        Item upgradeItem = Items[(int)Slot.Upgrade];
        Item consumeItem = Items[(int)Slot.Consume];

        //Not enough upgrade points
        if (_network.CraftsAvailable <= 0)
        {
            _infoText.text = "No crafting points available";
            _infoText.color = Color.red;
        }
        //If item can't be upgraded
        else if (!_canUpgrade && upgradeItem != null)
        {
            //Only gear can be upgraded
            if (upgradeItem.ItemType != Item.Type.Gear)
            {
                _infoText.text = "Only equipment can be upgraded";
                _infoText.color = Color.red;
            }
            //Quality cap hit
            else if (upgradeItem.Quality >= Item.UpgradeCapByTier[upgradeItem.Tier])
            {
                _infoText.text = "Already hit maximum upgrade cap";
                _infoText.color = Color.red;
            }
        }
        //Not enough upgrade material
        //else if(upgradeItem != null && _inventoryGUI.inventory.ItemCount(Item.UpgradeCapByTier[upgradeItem.Tier]) > 1)
        //{
        //    _infoText.text = "Not enough upgrade material";
        //    _infoText.color = Color.red;
        //}
        //Second item is not rare enough
        else if(upgradeItem != null && consumeItem != null && upgradeItem.ItemRarity > consumeItem.ItemRarity)
        {
            _infoText.text = "Can only upgrade with items of same or greater rarity";
            _infoText.color = Color.red;
        }
        //Need to add second item
        else if(upgradeItem != null && consumeItem == null)
        {
            _infoText.text = "Add second item to upgrade first item with";
            _infoText.color = Color.black;
        }
        //Show crafting points available
        else
        {
            _infoText.text = "Crafting Points: " + _network.CraftsAvailable;
            _infoText.color = Color.black;
        }
    }
}
