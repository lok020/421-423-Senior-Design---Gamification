using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class seeker
{
    public bool initialized;
    public int hp;
    public int stam;
    public int ins;
    public int dex;
    public int armor;
    public int durability;
    public int armor_id;

    public int reputation;

    public int quest_progress;
    public string current_quest;
    // Visited area is used in scenes such as Town_Castle1 to prevent dialogue from popping up after being there.
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
    public bool scanner; //bool in GlobalControl

}


public class GlobalControl : MonoBehaviour {
    public static GlobalControl Instance;
    public UnityEvent upon_waking;
    public TextAsset[] quest_dialogue;
    public bool in_dungeon;

    //Data will be copied to seekerData
    public bool initialized { get { return player.seekerData.initialized; } set { player.seekerData.initialized = value; } }
    public int hp { get { return player.seekerData.hp; } set { player.seekerData.hp = value; } }
    public int stam { get { return player.seekerData.stam; } set { player.seekerData.stam = value; } }
    public int ins { get { return player.seekerData.ins; } set { player.seekerData.ins = value; } }
    public int dex { get { return player.seekerData.dex; } set { player.seekerData.dex = value; } }
    public int armor { get { return player.seekerData.armor; } set { player.seekerData.armor = value; } }
    public int durability { get { return player.seekerData.durability; } set { player.seekerData.durability = value; } }
    public int armor_id { get { return player.seekerData.armor_id; } set { player.seekerData.armor_id = value; } }
    public int reputation { get { return player.seekerData.reputation; } set { player.seekerData.reputation = value; } }
    public int quest_progress { get { return player.seekerData.quest_progress; } set { player.seekerData.quest_progress = value; } }
    public string current_quest { get { return player.seekerData.current_quest; } set { player.seekerData.current_quest = value; } }
    public bool[] visited_area { get { return player.seekerData.visited_area; } set { player.seekerData.visited_area = value; } }
    public int[] entire_inventory { get { return player.seekerData.entire_inventory; } set { player.seekerData.entire_inventory = value; } }
    public int[] entire_storage_inventory { get { return player.seekerData.entire_storage_inventory; } set { player.seekerData.entire_storage_inventory = value; } }
    public bool[] discovered_items { get { return player.seekerData.discovered_items; } set { player.seekerData.discovered_items = value; } }
    public int current_population { get { return player.seekerData.current_population; } set { player.seekerData.current_population = value; } }
    public int max_population { get { return player.seekerData.max_population; } set { player.seekerData.max_population = value; } }
    public int forge_level { get { return player.seekerData.forge_level; } set { player.seekerData.forge_level = value; } }
    public int herb_level { get { return player.seekerData.herb_level; } set { player.seekerData.herb_level = value; } }
    public int bakery_level { get { return player.seekerData.bakery_level; } set { player.seekerData.bakery_level = value; } }
    public int training_level { get { return player.seekerData.training_level; } set { player.seekerData.training_level = value; } }
    public bool scanner { get { return player.seekerData.scanner; } set { player.seekerData.scanner = value; } }
    //=======================================


    public DungeonSetting dungeon;

    public List<ItemClass> full_items_list;
    //public AudioClip[] music;

    private void Start()
    {
        /*
        network = NetworkManager.Instance;
        Debug.Log("GlobalControl Start.");
        if (Instance == null)
        {
            
            while (!network._isLoggedIn) { }

            initialized = network._playersSeeker["Initialized"].AsBool;


            if (!initialized) //initialized is for seeker only. TODO: make Initialize methods and initialized flags for each game
            {
                InitializePlayer();
            }
            LoadInfo();
            LoadConq();
            LoadMm();
            //LoadIncremental(); --used for local loading?
            DontDestroyOnLoad(gameObject);
            Instance = this;
            upon_waking.Invoke();
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        InvokeRepeating("SaveInfo", 10.0f, 10.0f);
        */

        Debug.Log("Discovered_Items: " + discovered_items[1]);
    }

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

    public void DungeonSet(int a, int b,int d, float e, float f, int g, int h, IntRange i, IntRange j, IntRange k, IntRange l, int[] m, int[] n, int[] o, int p)
    {
        dungeon = new DungeonSetting(a, b, d, e, f, g, h, i, j, k, l, m, n, o, p);
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
            
            //initialized = NetworkManager.Instance._playersSeeker["Initialized"].AsBool;
            //initialized = PlayerPrefsX.GetBool("SeekerInitialize");
            if (!initialized) //initialized is for seeker only. TODO: make Initialize methods and initialized flags for each game
            {
                Debug.Log("INITIALIZING");
                InitializePlayer();
            }
            LoadInfo();
            DontDestroyOnLoad(gameObject);
            Instance = this;
            upon_waking.Invoke();
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("InitialLoad");
        PlayerPrefs.DeleteAll();
        InitializePlayer();
        LoadInfo();
        GameObject.Find("Canvas").GetComponentInChildren<loginScript>().gameStart();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Saving from closing");
        SaveInfo();
        in_dungeon = false;
    }

    private void InitializePlayer()
    {
        Debug.Log("Initializing Stuff Here.");
        initialized = true;
        player.Incre.stamina.cur = player.Incre.stamina.max;

        hp = 10;
        stam = 10;
        ins = 1;
        dex = 1;
        armor = 0;
        durability = 0;

        reputation = 0;

        quest_progress = 0;
        current_quest = "None.";

        visited_area = new bool[10];
        entire_inventory = new int[30];
        entire_storage_inventory = new int[60];
        discovered_items = new bool[full_items_list.Count];

        for (int x = 0; x < 10; x++)
        {
            visited_area[x] = false;
        }

        for (int x = 0; x < 30; x++)
        {
            entire_inventory[x] = 0;
        }

        for (int x = 0; x < 60; x++)
        {
            entire_storage_inventory[x] = 0;
        }

        for (int x = 0; x < full_items_list.Count; x++)
        {
            discovered_items[x] = false;
        }

        current_population = 0;
        max_population = 20;

        forge_level = 1;
        herb_level = 1;
        bakery_level = 1;
        training_level = 1;
        scanner = false;

        //SaveInfo();
    }

    public void LoadInfo()
    {
        Debug.Log("Seeker Loading.");
        
        //loading: hp = NetworkManger._seekerStats["health"].asint/asdouble/asfloat/value(for strings)/asbool/
        //loading arrays: entire_inventory[x] = NetworkManager._seekerInventory[x].asint

        /*
        hp = NetworkManager.Instance._seekerStats["Health"].AsInt;
        stam = NetworkManager.Instance._seekerStats["Stamina"].AsInt;
        ins = NetworkManager.Instance._seekerStats["Insight"].AsInt;
        dex = NetworkManager.Instance._seekerStats["Dexterity"].AsInt;
        armor = NetworkManager.Instance._seekerStats["Armor"].AsInt;
        durability = NetworkManager.Instance._seekerStats["Durability"].AsInt;
        armor_id = NetworkManager.Instance._seekerStats["ArmorID"].AsInt;

        reputation = NetworkManager.Instance._seekerStats["Reputation"].AsInt;

        quest_progress = NetworkManager.Instance._seekerQuestLog["StoryProgress"].AsInt;
        current_quest = NetworkManager.Instance._seekerQuestLog["CurrentQuest"].Value;

        in_dungeon = false;

        for (int x = 0; x < 30; x++)
        {
            entire_inventory[x] = NetworkManager.Instance._seekerInventory[x].AsInt;
        }
        for (int x = 0; x < 60; x++)
        {
            entire_storage_inventory[x] = NetworkManager.Instance._seekerStorage[x].AsInt;
        }
        for (int x = 0; x < full_items_list.Count; x++)
        {
            discovered_items[x] = NetworkManager.Instance._seekerItems[x].AsBool;
            full_items_list[x].discovered = discovered_items[x];
        }

        current_population = NetworkManager.Instance._playersSeeker["CurrentPopulation"].AsInt;
        max_population = NetworkManager.Instance._playersSeeker["MaxPopulation"].AsInt;

        forge_level = NetworkManager.Instance._seekerStats["Forge"].AsInt;
        herb_level = NetworkManager.Instance._seekerStats["Herb"].AsInt;
        bakery_level = NetworkManager.Instance._seekerStats["Bakery"].AsInt;
        training_level = NetworkManager.Instance._seekerStats["Training"].AsInt;
        scanner = NetworkManager.Instance._seekerStats["Scanner"].AsBool;
        //*/

        /*
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

        in_dungeon = false;

        entire_inventory = PlayerPrefsX.GetIntArray("SeekerEntireInventory");
        entire_storage_inventory = PlayerPrefsX.GetIntArray("SeekerEntireStorageInventory");
        discovered_items = PlayerPrefsX.GetBoolArray("SeekerDiscoveredItems");

        for(int x = 0; x < full_items_list.Count; x++)
        {
            full_items_list[x].discovered = discovered_items[x];
        }

        current_population = PlayerPrefs.GetInt("SeekerCurrentPopulation");
        max_population = PlayerPrefs.GetInt("SeekerMaxPopulation");

        forge_level = PlayerPrefs.GetInt("SeekerForgeLevel");
        herb_level = PlayerPrefs.GetInt("SeekerHerbLevel");
        bakery_level = PlayerPrefs.GetInt("SeekerBakeryLevel");
        training_level = PlayerPrefs.GetInt("SeekerTrainingLevel");
        scanner = PlayerPrefsX.GetBool("SeekerScannerItem");
        */
        Debug.Log("Seeker Loaded");
    }

    public void SaveInfo()
    {
        Debug.Log("Seeker Saving");

        //saving: NetworkManager.QueMessage("SEEKER_HP", "value")
        /*
        NetworkManager.Instance.QueueMessage(new List<string>() { "INIIALIZED", initialized.ToString() });
        NetworkManager.Instance.QueueMessage(new List<string>() { "SEEKER_STATS_UPDATE", hp.ToString(), stam.ToString(), dex.ToString(),
            ins.ToString(), armor.ToString(), durability.ToString(), armor_id.ToString(), reputation.ToString(), forge_level.ToString(), herb_level.ToString(),
            bakery_level.ToString(), training_level.ToString(), scanner.ToString() });

        List<string> inv = new List<string>();
        inv.Add("SEEKER_INVENTORY");
        for (int x = 0; x < 30; x++)
        {
            inv.Add(entire_inventory[x].ToString());
        }
        NetworkManager.Instance.QueueMessage(inv);

        inv.Clear();
        inv.Add("SEEKER_STORAGE");
        for (int x = 0; x < 60; x++)
        {
            inv.Add(entire_storage_inventory[x].ToString());
        }
        NetworkManager.Instance.QueueMessage(inv);

        inv.Clear();
        inv.Add("SEEKER_ITEMS");
        for (int x = 0; x < full_items_list.Count; x++)
        {
            inv.Add(discovered_items[x].ToString());
        }
        NetworkManager.Instance.QueueMessage(inv);

        NetworkManager.Instance.QueueMessage(new List<string> { "SEEKER_CURRENTPOPULATION", current_population.ToString() });
        NetworkManager.Instance.QueueMessage(new List<string> { "SEEKER_MAXPOPULATION", max_population.ToString() });
        */
        /*
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
        for (int x = 0; x < full_items_list.Count; x++)
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
        PlayerPrefsX.SetBool("SeekerScannerItem", scanner);
        */

        Debug.Log("Seeker Saved");
    }
    
}