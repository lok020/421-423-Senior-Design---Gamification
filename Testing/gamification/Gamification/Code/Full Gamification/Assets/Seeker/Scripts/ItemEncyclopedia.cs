using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEncyclopedia : MonoBehaviour {

    public int item_id;
    public Text item_name;
    public Text item_description;
    public Image item_image;
    public Text item_value;
    public Text item_usability;
    public string effects;


    void OnEnable()
    {
        ItemClass item = GlobalControl.Instance.full_items_list[item_id];
        if (item.discovered)
        {
            item_name.text = item.name;
            item_description.text = "Description: " + item.description;
            item_image.sprite = item.image;
            item_value.text = "Value: " + item.value.ToString() + " rep";
            if (!item.usable)
            {
                item_usability.text = "Non-usable.";
            }
            else
            {
                item_usability.text = effects;
            }
        }
    }

    // Use this for initialization
    void Start () {
        ItemClass item = GlobalControl.Instance.full_items_list[item_id];
        if (item.discovered)
        {
            item_name.text = item.name;
            item_description.text = "Description: " + item.description;
            item_image.sprite = item.image;
            item_value.text = "Value: " + item.value.ToString() + " rep";
            if (!item.usable)
            {
                item_usability.text = "Non-usable.";
            }
            else
            {
                item_usability.text = effects;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
