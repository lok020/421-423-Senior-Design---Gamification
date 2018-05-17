using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {

    public Button[] item_list;
    public GameObject inventory;
    public GameObject loot;
    public bool show;

    void Start()
    {

    }

    void Update()                   // Opens loot interface when show is true
    {
        if (show)
        {
            loot.SetActive(true);
        }
        else
        {
            loot.SetActive(false);
        }
    }

    public void SetItems(int[] items, Sprite[] images, TreasureManager chest)   // Setting up the interface with items
    {
        for (int x = 0; x < item_list.Length; x++)
        {
            item_list[x].GetComponent<Items>().item_id = items[x];
            item_list[x].GetComponent<Image>().sprite = images[items[x]];
            item_list[x].GetComponent<Items>().tied_chest = chest;
        }
    }

    public void RemoveItem(int slot, Sprite empty)                  // Removes item from interface when clicked on
    {
        item_list[slot].GetComponent<Items>().item_id = 0;
        item_list[slot].GetComponent<Image>().sprite = empty;
    }
    
}
