using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {

    public Button[] item_list;
    public GameObject inventory;
    public GameObject loot;
    public bool show;

    public GameObject description_display;
    public Text description_text;

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

    public void SetItems(int[] items, TreasureManager chest)   // Setting up the interface with items
    {
        for (int x = 0; x < item_list.Length; x++)
        {
            item_list[x].GetComponent<Items>().item_id = items[x];
            item_list[x].GetComponent<Image>().sprite = GlobalControl.Instance.full_items_list[items[x]].image;
            item_list[x].GetComponent<Items>().tied_chest = chest;
        }
    }

    public void RemoveItem(int slot, Sprite empty)                  // Removes item from interface when clicked on
    {
        item_list[slot].GetComponent<Items>().item_id = 0;
        item_list[slot].GetComponent<Image>().sprite = empty;
    }
    

    public bool ReturnItem(int id, Sprite item, int slot)
    {
        for (int i = 0; i < 30; i++)
        {
            if (item_list[i].GetComponent<Items>().item_id == 0)
            {
                item_list[i].GetComponent<Items>().ReceivedItem(id, slot);
                item_list[i].GetComponent<Image>().sprite = item;
                return true;
            }
        }
        return false;
    }

    public void DisplayDescription(int item_id)
    {
        if (description_display != null)
        {
            description_text.text = GlobalControl.Instance.full_items_list[item_id].name + ": " + GlobalControl.Instance.full_items_list[item_id].description + "\nSells for: " + GlobalControl.Instance.full_items_list[item_id].value + " reps.";
            description_display.SetActive(true);
        }
    }

    public void CloseDescription()
    {
        if (description_display != null)
        {
            description_display.SetActive(false);
        }
    }
}
