using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_Set : MonoBehaviour {

    public int stam_cost = 1;

    public int min_rewards = 1;
    public int max_rewards = 5;
    public int trap_difficulty = 1;                          // How hard the traps hit

    // MAY NEED MINIMUM AMOUNT OF REWARDS
    public float reward_chance = 1.0f;                             // Chance to contain reward
    public float people_chance = 1.0f;                             // chance to contain people

    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.

    public int[] regular_reward_set;
    public int[] lower_reward_set;
    public int[] higher_reward_set;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDungeon()
    {
        if (!CheckStamina())
        {
            return;
        }
        GlobalControl.Instance.DungeonSet(min_rewards, max_rewards, trap_difficulty, reward_chance, people_chance, columns, rows, numRooms, roomWidth, roomHeight, corridorLength, regular_reward_set, lower_reward_set, higher_reward_set);
        GlobalControl.Instance.in_dungeon = true;
    }

    private bool CheckStamina()
    {
        if (player.stamina.cur < stam_cost)
        {
            return false;
        }
        else
        {
            player.stamina.cur -= stam_cost;
            return true;
        }
    }
}
