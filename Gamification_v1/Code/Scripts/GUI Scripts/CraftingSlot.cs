using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    //Publicly accessible variables, for ease of assignment
    public CraftingGUI GUI;
    public Inventory Inventory;
    public Recipe Recipe
    {
        get
        {
            return _recipe;
        }
        set
        {
            _recipe = value;
            _update = true;
        }
    }
    public NetworkManager Network;
    public RecipeManager RecipeManager;

    //Flags
    private bool _craftable;
    private bool _update;

    //References
    private Button _button;
    private Image _image, _buttonImage;
    private Recipe _recipe;
    private Text _buttonText;


	// Use this for initialization
	void Start () {
        _button = GetComponentInChildren<Button>();
        _image = transform.GetChild(0).GetComponent<Image>();
        _buttonImage = _button.transform.GetChild(1).GetComponent<Image>();
        _buttonText = _button.GetComponentInChildren<Text>();
        _update = false;
	}
	
    //If recipe has changed, trigger an update
	void Update () {

        if(_update)
        {
            //Set image
            _image.sprite = _recipe.CraftedObject.GetComponent<Item>().Sprite;
            //If not unlocked
            if (_recipe.State == 1)
            {
                //Set color overlay
                Color c = _image.color;
                c.a = 0.5f;
                _image.color = c;
                //If buyable, update button
                if(_recipe.Price > 0)
                {
                    _button.gameObject.SetActive(true);
                    _buttonImage.enabled = true;
                    _buttonText.text = _recipe.Price.ToString();
                    //If player does not have enough gold, grey out button
                    //If item cannot be crafted, grey out button
                    if (Inventory.Gold < _recipe.Price)
                    {
                        _buttonText.color = Color.grey;
                    }
                    else
                    {
                        _buttonText.color = Color.black;
                    }
                }
                //Else hide the button
                else
                {
                    _button.gameObject.SetActive(false);
                }
            }
            //If unlocked
            else
            {
                _button.gameObject.SetActive(true);
                _buttonImage.enabled = false;
                _buttonText.text = "Craft";
                //If item cannot be crafted, grey out button
                if(!_craftable || Network.CraftsAvailable <= 0)
                {
                    _buttonText.color = Color.grey;
                }
                else
                {
                    _buttonText.color = Color.black;
                }
            }
            //Reset flag
            _update = false;
        }
	}

    public void OnPointerEnter(PointerEventData data)
    {
        GUI.ShowToolTip(GetComponent<RectTransform>().localPosition, Recipe);
    }

    public void OnPointerExit(PointerEventData data)
    {
        GUI.ExitToolTip();
    }

    public void ButtonClick()
    {
        //If not unlocked but purchasable, try buying the recipe
        if (Recipe.State == 1 && Recipe.Price > 0)
        {
            //Buy if player has enough gold
            if (Inventory.Gold >= Recipe.Price)
            {
                Inventory.RemoveGold(Recipe.Price);
                RecipeManager.Unlock(Recipe);
            }
        }
        //If unlocked, try crafting
        else if(Recipe.State == 2)
        {
            if (Network.CraftsAvailable > 0)
            {
                Inventory.CraftItem(Recipe);
            }
        }
    }

    public void SetCraftable(bool value)
    {
        _craftable = value;
        _update = true;
    }
}
