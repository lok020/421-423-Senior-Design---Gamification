using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_Set : MonoBehaviour {

    //Values varied in Unity.
    public int dungeon_level;
    public int stam_cost;

    public int min_rewards;
    public int max_rewards;
    public int trap_difficulty;                          // How hard the traps hit

    // MAY NEED MINIMUM AMOUNT OF REWARDS
    public float reward_chance;                             // Chance to contain reward
    public float people_chance;                             // chance to contain people

    public int columns;                                 // The number of columns on the board (how wide it will be).
    public int rows;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.

    // Set of rewards by their item ID.
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

        GlobalControl.Instance.SaveInfo(); // Save player data before entering dungeon.
        GlobalControl.Instance.DungeonSet(min_rewards, max_rewards, trap_difficulty, reward_chance, people_chance, columns, rows, numRooms, roomWidth, roomHeight, corridorLength, regular_reward_set, lower_reward_set, higher_reward_set, dungeon_level);
        GlobalControl.Instance.in_dungeon = true;

        this.GetComponent<Change_Scene>().CheckChange();
    }

    private bool CheckStamina()
    {
        if (player.Incre.stamina.cur < stam_cost)
        {
            return false;
        }
        else
        {
            player.Incre.stamina.cur -= stam_cost;
            return true;
        }
    }
}
