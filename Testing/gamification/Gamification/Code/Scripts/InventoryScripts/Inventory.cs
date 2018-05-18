using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 25;
    public int Gold;

    //Flag to indicate inventory has changed
    public bool InventoryChanged;

    //Private stuff
    private int queuedId = 0;
    private int queuedCount = 0;
    private NetworkManager _network;
    private PlayerController _player;
    private GUIManager _guiManager;

    // Use this for initialization
    void Start () 
    {
        _guiManager = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<GUIManager>();
        for(int i = 0; i < inventorySize; i++)
        {
            inventory.Add(null);
        }
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        _network.RetrievePlayerInventory(GetComponent<Inventory>());
        InventoryChanged = true;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //If an item has been queued for addition, add it
        if(queuedCount > 0)
        {
            AddItemToInventory(queuedId, queuedCount);
        }
    }
    
    //Adds item via below function and notifies database
    public bool AddItemToInventory(Item item)
    {
        bool success = AddItemToInventoryBase(item);
        if(success) _network.AddInventoryItem(item);
        return success;
    }

    //Same as above, except for multiple items
    public bool AddItemToInventory(Item item, int count)
    {
        bool success = true;
        for (int i = 0; i < count; i++)
        {
            success = AddItemToInventory(item);
            if (success == false) { return false; }
        }
        return success;
    }

    //Same as above, except with item ID passed as parameter (used by non-Monobehavior scripts)
    public bool AddItemToInventory(int itemID, int count)
    {
        Item item = Instantiate(Resources.Load(Item.Path[itemID]) as GameObject).GetComponent<Item>();
        return AddItemToInventory(item, count);
    }

    //checks to see if there is an open slot in inventory or an matching item that can be stacked upon, returns true if item is successfully added, false if not
    private bool AddItemToInventoryBase(Item item)
    {
        int index;
        DontDestroyOnLoad(item);
        index = GetInventoryIndex(item.ID);
        item.Slot = index;
        //No space in inventory
        if (index == -1)
        {
            return false;
        }
        item.transform.SetParent(transform);
        //Empty slot
        if (inventory[index] == null)
        {
            inventory[index] = item;
            item.Count = 1;
            InventoryChanged = true;
            return true;
        }
        //Partially filled stack
        else
        {
            item.Count = inventory[index].Count + 1;
            inventory[index].Count++;
            InventoryChanged = true;
            return true;
        }
    }

    //Used by bonus code system
    public void QueueItemForAddition(int itemID, int count)
    {
        queuedId = itemID;
        queuedCount = count;
    }

    //Adds from database. This is the only time when the Count and Slot fields should be used as is
    public void AddItemToInventoryFromDB(Item item)
    {
        int slotNumber = item.Slot;
        //Slot numbers -1 and -2 are used by the upgrade GUI
        if((slotNumber - 1) / 2 == -1)
        {
            var upgradeGUI = _guiManager.UpgradeUI.GetComponentInChildren<UpgradeGUI>();
            if(slotNumber == -1)
            {
                //upgradeGUI.UpdateSlot(UpgradeGUI.Slot.Upgrade, item);
            }
            else
            {
                //upgradeGUI.UpdateSlot(UpgradeGUI.Slot.Consume, item);
            }
            return;
        }
        //Slot number -3 is reserved for enchanting
        //TO DO - enchanting stuffs
        //If no slot number is defined, find the first empty slot
        if(slotNumber < 0)
        {
            slotNumber = inventory.IndexOf(null);
            item.Slot = slotNumber;
        }
        inventory[slotNumber] = item;
    }

    //Loops through inventory and removes "amount" items with id "id". Prioritizes removing from smaller stacks first
    public bool RemoveItemFromInventory(int id, int amount) {
        //Debug.Log("Removing " + amount + " of items with id = " + id);
        InventoryChanged = true;
        do
        {
            //First find the smallest stack
            int smallestStackIndex = -1;
            for (int i = 0; i < inventory.Count; i++)
            {
                //Skip null items
                if (inventory[i] == null) continue;
                if (inventory[i].ID == id)
                {
                    if (smallestStackIndex < 0)
                    {
                        smallestStackIndex = i;
                    }
                    else if (inventory[smallestStackIndex].Count > inventory[i].Count)
                    {
                        smallestStackIndex = i;
                    }
                }
            }
            //If smallestStackIndex is < 0, item wasn't found, so return false
            if(smallestStackIndex < 0)
            {
                return false;
            }
            //If amount is less than stack's count, remove this stack and update the amount remaining
            if (inventory[smallestStackIndex].Count < amount)
            {
                amount -= inventory[smallestStackIndex].Count;
                inventory[smallestStackIndex] = null;
                //Notify the database that an item has been removed
                _network.RemoveInventoryItem(smallestStackIndex, amount);
            }
            //Else remove the amount of items and return true
            else
            {
                //If count == amount, clear the stack
                if (inventory[smallestStackIndex].Count == amount)
                {
                    Destroy(inventory[smallestStackIndex]);
                    inventory[smallestStackIndex] = null;
                }
                //Else shrink the stack
                else
                {
                    inventory[smallestStackIndex].Count -= amount;
                }
                //Notify the database that an item has been removed
                _network.RemoveInventoryItem(smallestStackIndex, amount);
                return true;
            }
        } while (amount > 0);
        return false;
    }

    //Removes single item from inventory. Do NOT call this for items that stack - this is for equipment only
    public bool RemoveItemFromInventory(Item item)
    {
        int index = inventory.IndexOf(item);
        //If not found, return false
        if (index < 0) return false;
        //This clears the slot. 
        inventory[index] = null;
        return true;
    }

    //Returns number of items with ID "id" in the inventory, including in multiple stacks
    public int ItemCount(int id)
    {
        int count = 0;
        for(int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null) continue;
            if(inventory[i].ID == id)
            {
                count += inventory[i].Count;
            }
        }
        return count;
    }

    //Returns whether or not an item with ID "id" is in the inventory
    public bool ItemInInventory(int id)
    {
        return ItemCount(id) > 0;
    }

    //Returns whether or not a recipe is craftable
    public bool RecipeIsCraftable(Recipe recipe)
    {
        //Checks each item requirements. Returns false if there's not enough of an ingredient
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            int itemId = recipe.Ingredients[i].GetComponent<Item>().ID;
            int amountInInventory = ItemCount(itemId);
            //Return false if there aren't enough of this item
            if (amountInInventory < recipe.IngredientCounts[i])
            {
                return false;
            }
        }
        //All items are available, so return true
        return true;
    }
    
    //Returns true if item was crafted, false if it failed
    public bool CraftItem(Recipe recipe)
    {
        //Exit if the ingredients aren't available
        if (!RecipeIsCraftable(recipe)) return false;
      
        //Notify the database that an item is being crafted (this message needs to be sent before the items are added/removed)
        _network.DBCraftItem(1);
        //Remove ingredients from inventory
        for(int i = 0; i < recipe.Ingredients.Count; i++)
        {
            RemoveItemFromInventory(recipe.Ingredients[i].GetComponent<Item>().ID, recipe.IngredientCounts[i]);
        }
        //Add crafted item to the inventory
        var craftedItem = Instantiate(recipe.CraftedObject).GetComponent<Item>();
        craftedItem.RollRarityCrafted();
        craftedItem.RollEnchantments();
        craftedItem.Creator = _player.Name;
        craftedItem.Slot = GetInventoryIndex(craftedItem.ID);
        craftedItem.transform.parent = transform;
        craftedItem.gameObject.SetActive(false);
        AddItemToInventory(craftedItem);
        //Notify the event manager that an item has been crafted
        GetComponentInParent<EventManager>().Event(new List<object>() { GameAction.CRAFTED, craftedItem.ID, 1 });
        return true;
    }

    public void AddGold(int goldAdded)
    {
        _network.DBGetGold(goldAdded);
        Gold += goldAdded;
        _network.UpdatePlayerStat("Gold", Gold.ToString());
        //Debug.Log("Adding " + goldAdded + " gold! Now have " + gold + " gold");
    }

    //Removes gold, will not go below 0 gold.
    //You must manually check that there is enough gold before calling this function
    //This is just a fail safe to prevent negative gold
    public void RemoveGold(int goldRemoved)
    {
        Gold -= goldRemoved;
        if (Gold < 0) Gold = 0;
        _network.DBRemoveGold(goldRemoved);
        _network.UpdatePlayerStat("Gold", Gold.ToString());
    }

    //Get index for next available slot. Will try to pick a non-full stack, otherwise picks an empty slot
    public int GetInventoryIndex(int id)
    {
        int index = -1;
        for (int i = 0; i < inventorySize; i++)
        {
            //First empty slot
            if (inventory[i] == null && index == -1)
            {
                index = i;
            }
            //Skip following empty slots
            else if(inventory[i] == null)
            {
                continue;
            }
            //Non-full stack
            else if (inventory[i].ID == id && inventory[i].MaxStack > inventory[i].Count)
            {
                index = i;
            }
        }
        return index;
    }
}
