using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthUIManager : MonoBehaviour {

    public PlayerStats player_info;

    public Slider health_bar;
    public Text hp_text;

    public Slider stamina_bar;
    public Text stamina_text;

    public Text dexterity_text;
    public Text insight_text;
    public SpriteRenderer armor_image;
    public Text armor_text;

    public UnityEvent death;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        health_bar.maxValue = player_info.max_health;
        health_bar.value = player_info.current_health;
        hp_text.text = (player_info.current_health + "/" + player_info.max_health);

        stamina_bar.maxValue = player_info.max_stamina;
        stamina_bar.value = player_info.current_stamina;
        stamina_text.text = (player_info.current_stamina + "/" + player_info.max_stamina);
        
        if (GlobalControl.Instance.in_dungeon)
        {
            dexterity_text.text = "Dex:\n" + player_info.GetTotalDexterity().ToString() + " + " + player_info.add_dexterity;
            insight_text.text = "Ins:\n " + player_info.GetTotalInsight().ToString() + " + " + player_info.add_insight;

            if (player_info.armor == 0)
            {
                armor_image.sprite = GlobalControl.Instance.full_items_list[0].image;
            }
            else
            {
                armor_image.sprite = GlobalControl.Instance.full_items_list[player_info.armor_id].image;
            }
            armor_text.text = "Armor: " + player_info.armor.ToString() + " / Durability: " + player_info.armor_durability.ToString(); ;
        }


        if (player_info.current_health <= 0)
        {
            if (GlobalControl.Instance.in_dungeon)
            {
                for (int x = 0; x < 30; x++)
                {
                    GlobalControl.Instance.entire_inventory[x] = 0;         //Lose all inventory items.
                    GlobalControl.Instance.in_dungeon = false;
                }
            }
            death.Invoke();
        }
	}
}
