using System;

// Serializable so it will show up in the inspector.
[Serializable]
public class DungeonSetting
{
    //For dungeons
    public int level;
    public int min_rewards;
    public int max_rewards;
    public int trap_difficulty;                          // How hard the traps hit

    // MAY NEED MINIMUM AMOUNT OF REWARDS
    public float reward_chance;                             // Chance to contain reward
    public float people_chance;                             // chance to contain people

    public int columns;                                 // The number of columns on the board (how wide it will be).
    public int rows;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms;         // The range of the number of rooms there can be.
    public IntRange roomWidth;         // The range of widths rooms can have.
    public IntRange roomHeight;        // The range of heights rooms can have.
    public IntRange corridorLength;    // The range of lengths corridors between rooms can have.

    public int[] regular_rewards;
    public int[] lower_rewards;
    public int[] higher_rewards;

    //Constructor to set the values.
    public DungeonSetting(int a, int b, int d, float e, float f, int g, int h, IntRange i, IntRange j, IntRange k, IntRange l, int[] m, int[] n, int[] o, int p)
    {
        min_rewards = a;
        max_rewards = b;
        trap_difficulty = d;

        reward_chance = e;
        people_chance = f;

        columns = g;
        rows = h;
        numRooms = i;
        roomWidth = j;
        roomHeight = k;
        corridorLength = l;

        regular_rewards = m;
        lower_rewards = n;
        higher_rewards = o;

        level = p;
    }

}
