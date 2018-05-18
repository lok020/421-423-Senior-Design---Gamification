﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalControl : MonoBehaviour {

    public static GlobalControl Instance;

    public UnityEvent upon_waking;

    public bool initialized = false;

    public int hp;
    public int stam;
    public int ins;
    public int dex;
    public int armor;
    public int durability;
    public int armor_id;

    public int reputation;

    public int quest_progress;
    public TextAsset[] quest_dialogue;
    public string current_quest;
    public bool[] visited_area;

    public int[] entire_inventory;
    public int[] entire_storage_inventory;
    public bool[] discovered_items;

    public int current_population;
    public int max_population;

    public int forge_level;
    public int herb_level;
    public int bakery_level;
    public int training_level;

    public bool in_dungeon;

    public DungeonSetting dungeon;

    public List<ItemClass> full_items_list;
    //public AudioClip[] music;
    

    //conqueror fields
    //public bool c_initialized = false;
    public int conq_health;
    public int conq_equip;
    public int conq_skill;
    public double conq_damage;
    public string[] conq_inven;

    //mastermind fields
    public int beginner_level_complete;
    public int medium_level_complete;
    public int hard_level_complete;
    public int beginner_level_move;
    public int medium_level_move;
    public int hard_level_move;
    public int total_complete;
    public int total_time;

    //incremental fields
    public int passive_coins;
    public int active_coins;
    public int player_level;
    public int active_fill_rate;
    public int passive_fill_rate;
    public double progress;
    public double xp;
    public double inc_stamina;
    public string[] upgrades;

    void Update()
    {
    }

    public void VisitArea(int area)
    {
        visited_area[area] = true;
    }

    public bool GetVisitArea(int area)
    {
        return visited_area[area];
    }

    public void QuestSet(string new_quest)
    {
        current_quest = new_quest;
    }

    public void QuestUpdate()
    {
        quest_progress++;
    }

    public void QuestPointSet(int qp)
    {
        quest_progress = qp;
    }

    public void DungeonSet(int a, int b,int d, float e, float f, int g, int h, IntRange i, IntRange j, IntRange k, IntRange l, int[] m, int[] n, int[] o)
    {
        dungeon = new DungeonSetting(a, b, d, e, f, g, h, i, j, k, l, m, n, o);
    }
    

    public void UseItem(int id)
    {
        GameObject player = GameObject.Find("Player");

        player.GetComponent<PlayerStats>().ItemUsed(id);
    }

    void Awake()
    {
        Debug.Log("In awake.");
        if (Instance == null)
        {
            initialized = PlayerPrefsX.GetBool("SeekerInitialize");
            
            if (!initialized) //initialized is for seeker only. TODO: make Initialize methods and initialized flags for each game
            {
                InitializePlayer();
            }
            LoadInfo();
            LoadConq();
            LoadMm();
            //LoadIncremental();
            DontDestroyOnLoad(gameObject);
            Instance = this;
            upon_waking.Invoke();
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Saving from closing");
        SaveInfo();
        in_dungeon = false;
        SaveConq();
        SaveMm();
        //SaveIncremental();
        //Clear all playerprefs. Used for build testing.
        //PlayerPrefs.DeleteAll();
    }

    private void InitializePlayer()
    {
        Debug.Log("Initializing Stuff Here.");
        initialized = true;

        hp = 10;
        stam = 10;
        ins = 1;
        dex = 1;
        armor = 0;
        durability = 0;

        reputation = 0;

        quest_progress = 0;
        current_quest = "None.";

        entire_inventory = new int[30];
        entire_storage_inventory = new int[60];
        discovered_items = new bool[78];

        for (int x = 0; x < 30; x++)
        {
            entire_inventory[x] = 0;
        }

        for (int x = 0; x < 60; x++)
        {
            entire_storage_inventory[x] = 0;
        }

        for (int x = 0; x < 78; x++)
        {
            discovered_items[x] = false;
        }

        current_population = 0;
        max_population = 20;

        forge_level = 1;
        herb_level = 1;
        bakery_level = 1;
        training_level = 1;

        SaveInfo();
    }

    public void LoadInfo()
    {
        Debug.Log("Seeker Loading.");
        hp = PlayerPrefs.GetInt("SeekerHealth");
        stam = PlayerPrefs.GetInt("SeekerStamina");
        ins = PlayerPrefs.GetInt("SeekerInsight");
        dex = PlayerPrefs.GetInt("SeekerDexterity");
        armor = PlayerPrefs.GetInt("SeekerArmor");
        durability = PlayerPrefs.GetInt("SeekerDurability");
        armor_id = PlayerPrefs.GetInt("SeekerArmorID");

        reputation = PlayerPrefs.GetInt("SeekerReputation");

        quest_progress = PlayerPrefs.GetInt("SeekerQuestProgress");
        Debug.Log("Quest Progress: " + quest_progress);
        current_quest = PlayerPrefs.GetString("SeekerCurrentQuest");

        entire_inventory = PlayerPrefsX.GetIntArray("SeekerEntireInventory");
        entire_storage_inventory = PlayerPrefsX.GetIntArray("SeekerEntireStorageInventory");
        discovered_items = PlayerPrefsX.GetBoolArray("SeekerDiscoveredItems");

        for(int x = 0; x < 78; x++)
        {
            full_items_list[x].discovered = discovered_items[x];
        }

        current_population = PlayerPrefs.GetInt("SeekerCurrentPopulation");
        max_population = PlayerPrefs.GetInt("SeekerMaxPopulation");

        forge_level = PlayerPrefs.GetInt("SeekerForgeLevel");
        herb_level = PlayerPrefs.GetInt("SeekerHerbLevel");
        bakery_level = PlayerPrefs.GetInt("SeekerBakeryLevel");
        training_level = PlayerPrefs.GetInt("SeekerTrainingLevel");
        Debug.Log("Seeker Loaded");
    }

    public void LoadConq()
    {
       // initialized = PlayerPrefsX.GetBool("SeekerInitialize");
        conq_health = PlayerPrefs.GetInt("ConqHealth");
        conq_equip = PlayerPrefs.GetInt("ConqEquip");
        conq_skill = PlayerPrefs.GetInt("ConqSkill");
        conq_damage = PlayerPrefs.GetFloat("ConqDamage");//PlayerPrefs.GetInt("SeekerDexterity");

        conq_inven = PlayerPrefsX.GetStringArray("ConqInventoryhow");
    }

    public void LoadMm()
    {
         beginner_level_complete = PlayerPrefs.GetInt("BeginnerLevelComplete");
         medium_level_complete = PlayerPrefs.GetInt("MediumeLevelComplete");
         hard_level_complete = PlayerPrefs.GetInt("HardLevelComplete");
         beginner_level_move = PlayerPrefs.GetInt("BeginnerLevelMove");
         medium_level_move = PlayerPrefs.GetInt("MediumLevelMore");
         hard_level_move = PlayerPrefs.GetInt("HardLevelMove");
         total_complete = PlayerPrefs.GetInt("TotalComplete");
         total_time = PlayerPrefs.GetInt("TotalTime");
    }
    
    public void SaveInfo()
    {
        Debug.Log("Seeker Saving");
        PlayerPrefsX.SetBool("SeekerInitialize", initialized);
        PlayerPrefs.SetInt("SeekerHealth", hp);
        PlayerPrefs.SetInt("SeekerStamina", stam);
        PlayerPrefs.SetInt("SeekerInsight", ins);
        PlayerPrefs.SetInt("SeekerDexterity", dex);
        PlayerPrefs.SetInt("SeekerArmor", armor);
        PlayerPrefs.SetInt("SeekerDurability", durability);
        PlayerPrefs.SetInt("SeekerArmorID", armor_id);

        PlayerPrefs.SetInt("SeekerReputation", reputation);

        PlayerPrefs.SetInt("SeekerQuestProgress", quest_progress);
        Debug.Log("Saving quest progress: " + quest_progress);
        PlayerPrefs.SetString("SeekerCurrentQuest", current_quest);

        Debug.Log("In dungeon: " + in_dungeon);
        if (!in_dungeon)
        {
            PlayerPrefsX.SetIntArray("SeekerEntireInventory", entire_inventory);
        }
        in_dungeon = false;

        PlayerPrefsX.SetIntArray("SeekerEntireStorageInventory", entire_storage_inventory);
        for (int x = 0; x < 78; x++)
        {
            discovered_items[x] = full_items_list[x].discovered;
        }
        PlayerPrefsX.SetBoolArray("SeekerDiscoveredItems", discovered_items);

        PlayerPrefs.SetInt("SeekerCurrentPopulation", current_population);
        PlayerPrefs.SetInt("SeekerMaxPopulation", max_population);

        PlayerPrefs.SetInt("SeekerForgeLevel", forge_level);
        PlayerPrefs.SetInt("SeekerHerbLevel", herb_level);
        PlayerPrefs.SetInt("SeekerBakeryLevel", bakery_level);
        PlayerPrefs.SetInt("SeekerTrainingLevel", training_level);
        Debug.Log("Seeker Saved");
    }

    public void SaveConq()
    {
        PlayerPrefs.SetInt("ConqHealth", conq_health);
        PlayerPrefs.SetInt("ConqEquip", conq_equip);
        PlayerPrefs.SetInt("ConqSkill", conq_skill);
        PlayerPrefs.SetFloat("ConqDamage", (float)conq_damage);
        PlayerPrefsX.SetStringArray("ConqInven", conq_inven);
    }

    public void SaveMm()
    {
        PlayerPrefs.SetInt("BeginnerLevelComplete", beginner_level_complete);
        PlayerPrefs.SetInt("MediumeLevelComplete", medium_level_complete);
        PlayerPrefs.SetInt("HardLevelComplete", hard_level_complete);
        PlayerPrefs.SetInt("BeginnerLevelMove", beginner_level_move);
        PlayerPrefs.SetInt("MediumLevelMore", medium_level_move);
        PlayerPrefs.SetInt("HardLevelMove", hard_level_move);
        PlayerPrefs.SetInt("TotalComplete",total_complete);
        PlayerPrefs.SetInt("TotalTime", total_time);
    }

    /*
    public void SaveIncremental()
    {
         PlayerPrefs.SetInt("PassiveCoins", passive_coins);
         PlayerPrefs.SetInt("ActiveCoins", active_coins);
         PlayerPrefs.SetInt("PlayerLevel", player_level);
         PlayerPrefs.SetInt("ActiveFillRate", active_fill_rate);
         PlayerPrefs.SetInt("PassiveFillRate", passive_fill_rate);
         PlayerPrefs.SetFloat("Progress", (float) progress);
         PlayerPrefs.SetFloat("XP", (float) xp);
         PlayerPrefs.SetFloat("IncrementalStamina",(float) inc_stamina);
         PlayerPrefsX.SetStringArray("Upgrades", upgrades);
    }

    public void LoadIncremental()
    {
        passive_coins = PlayerPrefs.GetInt("PassiveCoins");
        active_coins = PlayerPrefs.GetInt("ActiveCoins");
        player_level = PlayerPrefs.GetInt("PlayerLevel");
        active_fill_rate = PlayerPrefs.GetInt("ActiveFillRate");
        passive_fill_rate = PlayerPrefs.GetInt("PassiveFillRate");
        progress = PlayerPrefs.GetFloat("Progress");
        xp = PlayerPrefs.GetFloat("XP");
        inc_stamina = PlayerPrefs.GetFloat("IncrementalStamina");
        upgrades = PlayerPrefsX.GetStringArray("Upgrades");
    }*/
}
