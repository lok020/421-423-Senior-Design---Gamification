using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {

    public static GlobalControl Instance;

    public int hp;

    public int stam;
    
    public int ins;

    public int dex;

    public int quest_progress;
    public int total_quest_progress;

    public int[] entire_inventory;

    public int[] entire_storage_inventory;

    public int current_population;
    public int max_population;

    void Update()
    {
        if (total_quest_progress < quest_progress)
        {
            total_quest_progress = quest_progress;
        }
    }

    public void QuestUpdate()
    {
        quest_progress++;
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
}
