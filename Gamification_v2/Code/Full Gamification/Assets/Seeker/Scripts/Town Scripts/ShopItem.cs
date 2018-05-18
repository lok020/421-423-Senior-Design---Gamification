using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    
    public Image item_image;
    public Text item_name;
    public Text item_description;
    public Text display_price;
    public string description;
    public Text items_required;
    public string items_used;

    private List<ItemClass> items_list;
    public int[] item_cost;
    public int item_id;
    public int rep_cost;
    public InventoryManager player_inventory;
    public bool can_buy;

	// Use this for initialization
	void Start () {
        items_list = GlobalControl.Instance.full_items_list;
        //can_buy = CheckInventory();
        item_image.sprite = items_list[item_id].image;
        item_name.text = items_list[item_id].name;
        item_description.text = description;
        display_price.text = rep_cost.ToString() + " Rep";

        if (items_required != null)
        {
            items_required.text = items_used;
        }

        //Upgrades
        if (item_id >= 56 && item_id<= 59)
        {
            rep_cost = GlobalControl.Instance.full_items_list[item_id].value * 10;
            display_price.text = rep_cost.ToString() + " Rep";
        }
        else if (item_id >= 68 && item_id <= 72)
        {
            switch (item_id)
            {
                case 68:
                    rep_cost = GlobalControl.Instance.full_items_list[68].value * 300;
                    display_price.text = rep_cost.ToString() + " Rep";
                    break;
                case 69:
                    rep_cost = GlobalControl.Instance.full_items_list[69].value * 200;
                    display_price.text = rep_cost.ToString() + " Rep";
                    break;
                case 70:
                    switch (GlobalControl.Instance.forge_level)
                    {
                        case 1:
                            rep_cost = 200;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 2:
                            rep_cost = 500;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 3:
                            rep_cost = 1000;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 4:
                            rep_cost = 0;
                            display_price.text = "MAX";
                            break;
                        default:
                            break;
                    }
                    break;
                case 71:
                    switch (GlobalControl.Instance.herb_level)
                    {
                        case 1:
                            rep_cost = 200;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 2:
                            rep_cost = 500;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 3:
                            rep_cost = 1000;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 4:
                            rep_cost = 0;
                            display_price.text = "MAX";
                            break;
                        default:
                            break;
                    }
                    break;
                case 72:
                    switch (GlobalControl.Instance.bakery_level)
                    {
                        case 1:
                            rep_cost = 200;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 2:
                            rep_cost = 500;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 3:
                            rep_cost = 1000;
                            display_price.text = rep_cost.ToString() + " Rep";
                            break;
                        case 4:
                            rep_cost = 0;
                            display_price.text = "MAX";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool CheckRep()
    {
        if (GlobalControl.Instance.reputation >= rep_cost)
        {
            return true;
        }
        return false;
    }

    private bool CheckInventory()
    {
        if (item_cost.Length == 0)
        {
            return true;
        }

        bool has_item = false;

        int[] temp_inventory = new int[30];
        
        for (int i = 0; i < 30; i++)
        {
            temp_inventory[i] = GlobalControl.Instance.entire_inventory[i];
        }

        for (int x = 0; x < item_cost.Length; x++)
        {
            has_item = false;

            for (int y = 0; y < 30; y++)
            {
                if (temp_inventory[y] == item_cost[x])
                {
                    temp_inventory[y] = 0;
                    has_item = true;
                    y = 30;         //Instantly end for loop.
                }
            }

            if (!has_item)
            {
                return false;
            }
        }

        return true;
    }

    public void BuyItem()
    {
        if (CheckInventory() && CheckRep())
        {
            can_buy = true;
        }
        else
        {
            can_buy = false;
        }
        
        if (can_buy)
        {
            for (int i = 0; i < item_cost.Length; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (GlobalControl.Instance.entire_inventory[j] == item_cost[i])
                    {
                        player_inventory.RemoveItem(j);
                        j = 30;
                    }
                }
            }

            int x = player_inventory.HasRoom();
            //RemoveItems();
            if (x >= 0)
            {
                player_inventory.GetItem(x, item_id);
                GlobalControl.Instance.reputation -= rep_cost;
                can_buy = CheckRep();
            }
        }

    }

    public void BuyUpgrade()
    {
        int upgrade_multiplier = GlobalControl.Instance.training_level;
        can_buy = CheckRep();
        if (can_buy)
        {
            if (item_id >= 56 && item_id <= 59)
            {
                switch (item_id)
                {
                    case 56://Health
                        GlobalControl.Instance.hp += 10 * upgrade_multiplier;
                        break;
                    case 57://Stamina
                        GlobalControl.Instance.stam += 10 * upgrade_multiplier;
                        break;
                    case 58://Dexterity
                        GlobalControl.Instance.dex += 1 * upgrade_multiplier;
                        break;
                    case 59://Insight
                        GlobalControl.Instance.ins += 1 * upgrade_multiplier;
                        break;
                    default:
                        break;
                }
                GlobalControl.Instance.reputation -= rep_cost;
                GlobalControl.Instance.full_items_list[item_id].value++;
                rep_cost = GlobalControl.Instance.full_items_list[item_id].value * 10;
                display_price.text = rep_cost.ToString() + " Rep";
            }
            else if (item_id >= 68 && item_id <= 72)
            {
                switch (item_id)
                {
                    case 68://Maximum Population
                        GlobalControl.Instance.max_population += 20 * GlobalControl.Instance.full_items_list[68].value;
                        GlobalControl.Instance.full_items_list[68].value++;
                        GlobalControl.Instance.reputation -= rep_cost;
                        rep_cost = GlobalControl.Instance.full_items_list[68].value * 300;
                        display_price.text = rep_cost.ToString() + " Rep";

                        break;
                    case 69://Training Room
                        GlobalControl.Instance.training_level++;
                        GlobalControl.Instance.full_items_list[69].value++;
                        GlobalControl.Instance.reputation -= rep_cost;
                        rep_cost = GlobalControl.Instance.full_items_list[69].value * 200;
                        display_price.text = rep_cost.ToString() + " Rep";
                        break;
                    case 70://Forge Shop
                        if (GlobalControl.Instance.forge_level < 4)
                        {
                            GlobalControl.Instance.forge_level++;
                            GlobalControl.Instance.full_items_list[70].value++;
                            GlobalControl.Instance.reputation -= rep_cost;
                        }

                        switch (GlobalControl.Instance.forge_level)
                        {
                            case 1:
                                rep_cost = 200;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 2:
                                rep_cost = 500;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 3:
                                rep_cost = 1000;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 4:
                                rep_cost = 0;
                                display_price.text = "MAX";
                                break;
                            default:
                                break;
                        }
                        break;
                    case 71://Herb Shop
                        if (GlobalControl.Instance.herb_level < 4)
                        {
                            GlobalControl.Instance.herb_level++;
                            GlobalControl.Instance.full_items_list[71].value++;
                            GlobalControl.Instance.reputation -= rep_cost;
                        }

                        switch (GlobalControl.Instance.herb_level)
                        {
                            case 1:
                                rep_cost = 200;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 2:
                                rep_cost = 500;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 3:
                                rep_cost = 1000;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 4:
                                rep_cost = 0;
                                display_price.text = "MAX";
                                break;
                            default:
                                break;
                        }
                        break;
                    case 72://Bakery Shop
                        if (GlobalControl.Instance.bakery_level < 4)
                        {
                            GlobalControl.Instance.bakery_level++;
                            GlobalControl.Instance.full_items_list[72].value++;
                            GlobalControl.Instance.reputation -= rep_cost;
                        }

                        switch (GlobalControl.Instance.bakery_level)
                        {
                            case 1:
                                rep_cost = 200;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 2:
                                rep_cost = 500;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 3:
                                rep_cost = 1000;
                                display_price.text = rep_cost.ToString() + " Rep";
                                break;
                            case 4:
                                rep_cost = 0;
                                display_price.text = "MAX";
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }

    private void RemoveItems()
    {
        for (int x = 0; x < item_cost.Length; x++)
        {
            for (int y = 0; y < 30; y++)
            {
                if (player_inventory.GetComponent<PlayerInventory>().inventory_items[y] == item_cost[x])
                {
                    player_inventory.GetComponent<PlayerInventory>().RemoveItem(y);
                    y = 30;     //Instantly end for loop
                }
            }
        }
    }

}
