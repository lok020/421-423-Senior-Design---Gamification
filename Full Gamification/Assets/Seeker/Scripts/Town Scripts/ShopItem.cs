using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {



    public Sprite item_pic;
    public Image item_image;
    public Text item_name;
    public Text item_description;

    public string shop_item_name;
    public string description;

    public int[] item_cost;
    public int item_id;
    public GameObject player_inventory;
    public bool can_buy;

	// Use this for initialization
	void Start () {
        can_buy = CheckInventory();
        item_image.sprite = item_pic;
        item_name.text = shop_item_name;
        item_description.text = description;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool CheckInventory()
    {
        bool has_item = false;

        for (int x = 0; x < item_cost.Length; x++)
        {
            has_item = false;

            for (int y = 0; y < 30; y++)
            {
                if (player_inventory.GetComponent<PlayerInventory>().inventory_items[y] == item_cost[x])
                {
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
        if (can_buy)
        {
            RemoveItems();
            for (int x = 0; x < 30; x++)        //Look for empty slot
            {
                if (player_inventory.GetComponent<PlayerInventory>().inventory_items[x] == 0)
                {
                    player_inventory.GetComponent<PlayerInventory>().GetItem(x, item_id);
                    can_buy = CheckInventory();
                    return;
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
