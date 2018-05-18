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

    //conqueror
    public static int conq_health { get; set; }
    public static double damage { get; set; }
    public static int skill { get; set; }
    public static int equip { get; set; }
    public static string[] con_inven = new string[10];

    //mastermind
    public static int beginner_complete { get; set; }
    public static int medium_complete { get; set; }
    public static int hard_complete { get; set; }
    public static int beginner_move { get; set; }
    public static int medium_move { get; set; }
    public static int hard_move { get; set; }
    public static int total_complete { get; set; }
    public static int total_time { get; set; }

    //incremental
    public static int passive_coins { get; set; }
    public static int active_coins { get; set; }
    public static int player_level { get; set; }
    public static int active_fill_rate { get; set; }
    public static int passive_fill_rate { get; set; }
    public static double progress { get; set; }
    public static double xp { get; set; }
    public static double inc_stamina { get; set; }
    public static string[] upgrades = new string[20];
}
