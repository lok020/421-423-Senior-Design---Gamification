using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {

    public static GlobalControl Instance;

    public int hp;
    public int hpxp;
    public int hpnextxp;

    public int stam;
    public int stamxp;
    public int stamnextxp;
    
    public int ins;
    public int insxp;
    public int insnextxp;

    public int dex;
    public int dexxp;
    public int dexnextxp;

    public int quest_progress;

    public int[] entire_inventory;

    public int[] entire_storage_inventory;

    public int current_population;
    public int max_population;

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
