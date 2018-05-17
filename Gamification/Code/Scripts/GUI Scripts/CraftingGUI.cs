using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class CraftingGUI : MonoBehaviour {
    
    public List<GameObject> craftingSlots = new List<GameObject>();
    public GameObject slots;
    public GameObject tooltip;
    public Text itemName;
    public Text itemStats;
    public Inventory inventory;
    public Text CraftsRemaining;

    private bool _update;
    private float x = 0;
    private float y = 0;
    private int scaledWidth;
    private int scaledHeight;
    private GameObject[] _slots = new GameObject[20];   //List of all slots
    private NetworkManager _network;
    private RecipeManager _recipeManager;
    private Scrollbar _scrollbar;

	// Use this for initialization
	void Start () {
        var player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        _recipeManager = player.GetComponent<RecipeManager>();
        _scrollbar = GetComponentInChildren<Scrollbar>();
        _update = true;

        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();

        var rect = GetComponent<RectTransform>().rect;
        x = 8 - rect.width / 2;
        y = 0 + rect.height / 2;
        scaledWidth = (int)(rect.width - 30) / 6;
        scaledHeight = (int)(rect.height - 30) / 6;

        //Instantiate all 24 slots
        for(int i = 0; i < _slots.Length; i++)
        {
            //Create slot
            GameObject newSlot = Instantiate(slots);
            var craftingSlot = newSlot.GetComponent<CraftingSlot>();
            var rectTransform = newSlot.GetComponent<RectTransform>();
            //Set up slot
            craftingSlot.GUI = this;
            craftingSlot.Inventory = inventory;
            craftingSlot.Network = _network;
            craftingSlot.RecipeManager = _recipeManager;
            newSlot.transform.SetParent(transform, false);
            newSlot.name = "slot" + i;
            rectTransform.sizeDelta = new Vector3(scaledWidth, scaledHeight);
            rectTransform.localPosition = new Vector3(x + 5 + rectTransform.rect.width / 2,
                y - 5 - rectTransform.rect.height / 2, 0);
            x += (5 + scaledWidth);
            if (i % 5 == 4)
            {
                x = 8 - GetComponent<RectTransform>().rect.width / 2;
                y -= (20 + scaledHeight);
            }
            //Hide slot
            newSlot.SetActive(false);
            //Add to array
            _slots[i] = newSlot;
        }

        //Set up scrollbar
        _scrollbar.numberOfSteps = Mathf.CeilToInt((_recipeManager.Recipes.Count - 15) / 5f);

        //Update the slots
        UpdateSlots();

        //Update crafts available
        CraftsRemaining.text = "Crafts remaining  " + _network.CraftsAvailable;
    }

    //Update the slots if the recipe list changes
    public void Update()
    {
        //Reload all slots if recipe list has changed
        if (_recipeManager.RecipeListChanged || _update)
        {
            UpdateSlots();
            //Update scrollbar
            _scrollbar.numberOfSteps = Mathf.CeilToInt((_recipeManager.Recipes.Count - 15) / 5f);
            //Update flags
            _recipeManager.RecipeListChanged = false;
            _update = false;
        }
        //Refresh each slot if inventory has changed
        if (inventory.InventoryChanged)
        {
            foreach (var slot in _slots)
            {
                if (slot.activeSelf == false) continue;
                var craftingSlot = slot.GetComponent<CraftingSlot>();
                craftingSlot.SetCraftable(inventory.RecipeIsCraftable(craftingSlot.Recipe));
            }
            inventory.InventoryChanged = false;
        }
        //Update crafts available
        CraftsRemaining.text = "Crafts remaining  " + _network.CraftsAvailable;
    }

    //Updates all of the slots
    private void UpdateSlots()
    {
        //This will store the number of slots used
        int slotsActive = 0;
        int step = (int)(_scrollbar.value * (_scrollbar.numberOfSteps - 1));
        if (step < 0) step = 0;
        //Assign recipes to the slots, until all recipes are used or slots are all full
        for(int i = 5 * step; slotsActive < _slots.Length && i < _recipeManager.Recipes.Count; i++)
        {
            _slots[slotsActive].SetActive(true);
            _slots[slotsActive].GetComponent<CraftingSlot>().Recipe = _recipeManager.Recipes[i];
            slotsActive++;
        }
        //Hide all remaining unused slots
        for (int i = slotsActive; i < _slots.Length; i++)
        {
            _slots[i].SetActive(false);
        }
    }

    public void Scroll()
    {
        _update = true;
    }

    public void ShowToolTip(Vector3 position, Recipe recipe)
    {
        //Get reference to crafted item
        var craftedItem = recipe.CraftedObject.GetComponent<Item>();
        //CraftingDetails cdetails = item.Item.GetComponent<CraftingDetails>();
        itemName.text = craftedItem.Name;

        itemStats.text = craftedItem.GetTooltip();
        if (itemStats.text.Length > 0) itemStats.text += "\n";

        //If item is not unlocked but can be bought
        if(recipe.State == 1 && recipe.Price > 0)
        {
            itemStats.text += String.Format("{0,-10} {1,12}", "Price", recipe.Price + " gold");
        }
        //If item is not unlocked and cannot be bought
        else if(recipe.State == 1)
        {
            itemStats.text += recipe.UnlockInfo;
        }
        //Item is unlocked
        else
        {
            itemStats.text += "Crafting Requirements";
            for(int i = 0; i < recipe.Ingredients.Count; i++)
            {
                itemStats.text += String.Format("\n{0,-18} {1,4}", recipe.Ingredients[i].GetComponent<Item>().Name, "x" + recipe.IngredientCounts[i]);
            }

        }
        var rect = GetComponent<RectTransform>();
        var tRect = tooltip.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(rect.localPosition.x + position.x + (scaledWidth / 2) + (tRect.rect.width / 2),
                                          rect.localPosition.y + position.y - (tRect.rect.height / 2), 0);
        tRect.SetAsLastSibling();
        tooltip.SetActive(true);

    }

    public void ExitToolTip()
    {
        tooltip.SetActive(false);
        itemName.text = "";
        itemStats.text = "";
    }
}
