using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombatDetails : MonoBehaviour {
    public Sprite background;
    public List<GameObject> enemies;
    public List<GameObject> rewards;
    //adds up to 1.0
    public List<float> rewardRates;
    public int maxGold, minGold, maxXp, minXp;
    public int maxRewards;
    public int minRewards;
    public List<bool> guaranteedDrop;
    public bool isEvent = false;
	// Use this for initialization
	void Start () {
	    //need to add levelmanager  to get current level background
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
