using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour {

    public class quest_info { public int quest_id { get; set; } public int quest_time { get; set; } }
    public class town_population { public int town_id { get; set; } public int population { get; set; } }
    

    //Player info
    public static string player_name { get; set; }
    public static int health { get; set; }
    public static int stamina { get; set; }
    public static int dexterity { get; set; }
    public static int insight { get; set; }
    public static int[] inventory = new int[30];

    //Area info
    public static string current_area { get; set; }
    public static int main_quest_progress { get; set; }
    public static List<town_population> town_pop = new List<town_population>();

    //Active quests
    public static List<quest_info> active_quests = new List<quest_info>();

}
