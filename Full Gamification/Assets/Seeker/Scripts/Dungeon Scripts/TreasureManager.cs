using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TreasureManager : MonoBehaviour {

    public int num_rewards;             // Number of rewards
    public int rank;                    // How good the rewards are
    public int[] rewards;               // Array of rewards ID


    public Sprite[] item_images;
    public Sprite opened;
    public Sprite empty_chest;


    public GameObject treasure_items;
    public GameObject inventory;

    void Start()
    {
        treasure_items = GameObject.Find("Loot");
        inventory = GameObject.Find("InventoryDungeon");
        rewards = new int[30];

        for (int x = 0; x < 30; x++)    // NEEDS FORMULA. Currently randomly generates items between 1 and 11
        {
            if (x >= num_rewards)
            {
                rewards[x] = 0;
            }
            else
            {
                if (Random.Range(0.0f, 10.0f) >= 9.5) //Chance of rare item
                {
                    rewards[x] = Random.Range(rank + 5, rank + 15);
                }
                else //Regular level items
                {
                    rewards[x] = Random.Range(rank, rank + 10);
                }
            }
        }
            
    }


    void OnTriggerEnter2D(Collider2D other)         // Opens loot interface when sees chest
    {
        if (other.gameObject.name == "Player")
        {
            this.GetComponent<SpriteRenderer>().sprite = opened;    // Changes looks of chest to signify it's been looked in

            treasure_items.GetComponent<ItemManager>().show = true;
            treasure_items.GetComponent<ItemManager>().SetItems(rewards, item_images, this);    // Sets up the loot interface to see items in this chest
        }
        
    }

    void OnTriggerExit2D(Collider2D other)              // Closes the loot interface when leaving chest
    {
        if (other.gameObject.name == "Player")
        {

            if (num_rewards <= 0)
            {
                this.GetComponent<SpriteRenderer>().sprite = empty_chest; // Changes looks of chest to be empty
            }

            treasure_items.GetComponent<ItemManager>().show = false;
        }
    }

    public void TakeItem(int item_slot)
    {
        Debug.Log(item_slot);
        int x = inventory.GetComponent<InventoryManager>().HasRoom();
        int y = rewards[item_slot];
        if (y != 0 && x >= 0)
        {
            inventory.GetComponent<InventoryManager>().GetItem(x, y, item_images[y]);
            treasure_items.GetComponent<ItemManager>().RemoveItem(item_slot, item_images[0]);
            rewards[item_slot] = 0;
            num_rewards--;
        }
    }
}
