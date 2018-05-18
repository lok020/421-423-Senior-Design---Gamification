using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {

    public static GlobalControl Instance;
    
    public int hp;
    public int stam;
    public int ins;
    public int dex;

    public int reputation;

    public int quest_progress;
    public bool[] quest_visited;

    public int[] entire_inventory;

    public int[] entire_storage_inventory;

    public int current_population;
    public int max_population;

    public int forge_level;
    public int herb_level;
    public int bakery_level;
    public int training_level;

    public DungeonSetting dungeon;

    public List<ItemClass> full_items_list;

    void Update()
    {
    }

    public void QuestUpdate()
    {
        quest_progress++;
    }

    public void QuestSet(int qp)
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
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveInfo()
    {
        //int hp;
        //int stam;
        //int ins;
        //int dex;

        //int reputation;

        //int quest_progress;
        //bool[] quest_visited;

        //int[] entire_inventory;
        //int[] entire_storage_inventory;

        //int current_population;
        //int max_population;

        //int forge_level;
        //int herb_level;
        //int bakery_level;
        //int training_level;

        //public List<ItemClass> full_items_list;
    }
}
