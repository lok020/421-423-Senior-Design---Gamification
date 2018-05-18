using UnityEngine;
using System.Collections.Generic;

public class Recipe : MonoBehaviour {

    //Item that can be crafted
    public GameObject CraftedObject;

    //Ingredients
    public List<GameObject> Ingredients;
    public List<int> IngredientCounts;

    //Recipe info
    public int ID;
    public int State;
    public int Price;
    public string UnlockInfo;
    
    //Paths for each recipe object
    public static Dictionary<int, string> Paths = new Dictionary<int, string>()
    {
        //------    Tier One
        { 0, "Recipes/Health Potion" },
        { 1, "Recipes/Bronze Boots" },
        { 2, "Recipes/Bronze Cuirass" },
        { 3, "Recipes/Bronze Gauntlets" },
        { 4, "Recipes/Bronze Greaves" },
        { 5, "Recipes/Bronze Helm" },
        { 6, "Recipes/Cotton Boots" },
        { 7, "Recipes/Cotton Robe" },
        { 8, "Recipes/Cotton Gloves" },
        { 9, "Recipes/Cotton Pants" },
        { 10, "Recipes/Cotton Hat" },
        { 11, "Recipes/Leather Boots" },
        { 12, "Recipes/Leather Jacket" },
        { 13, "Recipes/Leather Gloves" },
        { 14, "Recipes/Leather Pants" },
        { 15, "Recipes/Leather Helm" },
        { 16, "Recipes/Thread" },
        { 17, "Recipes/Bronze Shield" },
        { 18, "Recipes/Bronze Sword" },
        { 19, "Recipes/Bronze Mace" },
        { 20, "Recipes/Oak Bow" },
        { 21, "Recipes/Oak Staff" },
        { 22, "Recipes/Oak Wand" },
        //--    Special Items

    };
}
