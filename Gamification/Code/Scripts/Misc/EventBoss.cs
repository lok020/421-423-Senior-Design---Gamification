using UnityEngine;
using System.Collections;

public class EventBoss : MonoBehaviour {
    public int health, maxHealth;
    public float progress, maxProgress;
    public string Name;
    public GameObject fightDetails;
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateProgress()
    {
        //get info from database here
    }

    public void StartCombat(PlayerController Player)
    {
        //Player.CombatPosition = transform.position;
        Player.nextLevel.name = Application.loadedLevelName;
        Player.combatDetails = fightDetails;
        Player.DoNotDestroy();
        Application.LoadLevel("Combat");
    }
}
