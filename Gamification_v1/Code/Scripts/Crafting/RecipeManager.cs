using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour {
    
    //All available (not hidden) recipes
    public SortedList<int, Recipe> Recipes;

    //Flags
    public bool RecipeStateChanged;     //Triggers Update() in this object
    public bool RecipeListChanged;      //Triggers Update() in CraftingGUI

    //Recipe states
    private List<int> _recipeStates;

    //References
    private NetworkManager _network;

	// Use this for initialization
	void Start () {
        //Initialize objects and variables
        Recipes = new SortedList<int, Recipe>();
        _recipeStates = new List<int>(new int[Recipe.Paths.Count]);
        RecipeStateChanged = true;
        //Get network manager
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
	    //Load default states to start
        SetDefaultStates();
        //Then load from the database
        for(int i = 0; i < _recipeStates.Count; i++)
        {
            _recipeStates[i] = _network.GetRecipeStatus(i, _recipeStates[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {

	    //Update list of Recipes if RecipeStateChanged == true
        if(RecipeStateChanged)
        {
            //Loop through each state
            for(int i = 0; i < _recipeStates.Count; i++)
            {
                //If state is 0 (hidden), skip this
                if (_recipeStates[i] == 0) continue;
                
                //If this key is not in use in Recipes, instantiate
                //the relevant object and add it to the list
                if(!Recipes.ContainsKey(i))
                {
                    //Load object
                    GameObject recipeObject = Resources.Load(Recipe.Paths[i]) as GameObject;
                    //Add it to list
                    Recipes.Add(i, recipeObject.GetComponent<Recipe>());
                }

                //Sync states
                Recipes[i].State = _recipeStates[i];

                //List has changed
                RecipeListChanged = true;
            }
            //Finally, reset flag
            RecipeStateChanged = false;
        }
	}

    //Makes recipe unlockable
    public void MakeUnlockable(Recipe recipe)
    {
        //If state is not 0 (hidden), do nothing
        if (recipe.State != 0) return;

        //Update state
        _recipeStates[recipe.ID] = 1;
        recipe.State = 1;

        //Save to server
        _network.UpdateRecipe(recipe.ID, 1);

        //Update flag
        RecipeStateChanged = true;
    }

    public void Unlock(Recipe recipe)
    {
        //If state is 2 (unlocked), do nothing
        if (recipe.State == 2) return;

        //Update state
        recipe.State = 2;
        _recipeStates[recipe.ID] = 2;

        //Save to server
        _network.UpdateRecipe(recipe.ID, 2);

        //Update flag
        RecipeStateChanged = true;
    }


    //Sets default values for all recipes
    //0 = hidden, 1 = unlockable, 2 = unlocked
    private void SetDefaultStates()
    {
        _recipeStates[0] = 1;
        _recipeStates[1] = 1;
        _recipeStates[2] = 2;
        _recipeStates[3] = 1;
        _recipeStates[4] = 2;
        _recipeStates[5] = 1;
        _recipeStates[6] = 1;
        _recipeStates[7] = 2;
        _recipeStates[8] = 1;
        _recipeStates[9] = 2;
        _recipeStates[10] = 1;
        _recipeStates[11] = 1;
        _recipeStates[12] = 2;
        _recipeStates[13] = 1;
        _recipeStates[14] = 2;
        _recipeStates[15] = 1;
        _recipeStates[16] = 1;
        _recipeStates[17] = 1;
        _recipeStates[18] = 2;
        _recipeStates[19] = 1;
        _recipeStates[20] = 2;
        _recipeStates[21] = 1;
        _recipeStates[22] = 2;
    }
}
