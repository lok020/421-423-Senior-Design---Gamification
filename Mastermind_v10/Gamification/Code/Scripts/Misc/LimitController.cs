using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LimitController {
    //player can't level further than this
    public static int levelCap = 5;

    //LevelRequirements<level,xp required>
    public static Dictionary<int, int> LevelReqs = new Dictionary<int, int>(){
            { 1, 0 },
            { 2, 50 },
            { 3, 125 },
            { 4, 250 },
            { 5, 400 },
            { 6, 600 },
            { 7, 850 },
            { 8, 1050 },
            { 9, 1300 },
            { 10, 1500 }
    };
}
