using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load {

    public static void load_all()
    {
        int x = 0;

        GameInfo.player_name = PlayerPrefs.GetString("Player Name");
        GameInfo.health = PlayerPrefs.GetInt("Health");
        GameInfo.stamina = PlayerPrefs.GetInt("Stamina");
        GameInfo.dexterity = PlayerPrefs.GetInt("Dexterity");
        GameInfo.insight = PlayerPrefs.GetInt("Insight");

        GameInfo.current_area = PlayerPrefs.GetString("Current Town");
        GameInfo.main_quest_progress = PlayerPrefs.GetInt("Main Quest");

        if (GameInfo.inventory != null || GameInfo.inventory.Length <= 0)
        {
            for (x = 0; x < GameInfo.inventory.Length; x++)
            {
                GameInfo.inventory[x] = PlayerPrefs.GetInt("Inventory Items" + x);
            }
        }


        if (GameInfo.town_pop != null || GameInfo.inventory.Length <= 0)
        {
            for (x = 0; x < GameInfo.town_pop.Count; x++)
            {
                GameInfo.town_pop[x].town_id = PlayerPrefs.GetInt("Town ID" + x);
                GameInfo.town_pop[x].population = PlayerPrefs.GetInt("Town Population" + x);
            }
        }
            

        if (GameInfo.active_quests != null || GameInfo.inventory.Length <= 0)
        {
            for (x = 0; x < GameInfo.active_quests.Count; x++)
            {
                GameInfo.active_quests[x].quest_id = PlayerPrefs.GetInt("Active Quests" + x);
                GameInfo.active_quests[x].quest_time = PlayerPrefs.GetInt("Quest Time" + x);
            }
        }
    }
}
