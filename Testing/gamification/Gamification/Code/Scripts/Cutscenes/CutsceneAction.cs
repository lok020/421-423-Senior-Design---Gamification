/*  Each CutsceneAction requires different vars, which is why the class has them as "objects".
    Vars for each CAction: (items marked with an X haven't been implemented yet)
    CAMERA_MOVE:    float x_dir, float y_dir, float moveSpeed, float time
    COMBAT_START:   GameObject combatDetails
    GAMEOBJECT_ENABLE:  GameObject object
    GAMEOBJECT_DISABLE: GameObject object
    NPC_ANIMATE:    GameObject target, string animName
    NPC_FACE:       GameObject npc, int direction
    NPC_MOVE:       GameObject npc, float x_dir, float y_dir, float moveSpeed, float time
    NPC_SPEAK:      GameObject npc, string dialog, float time
    PLAYER_ANIMATE: string animName
    PLAYER_FACE:    int direction
    PLAYER_MOVE:    float x_dir, float y_dir, float moveSpeed, float time
    PLAYER_SPEAK:   string dialog, float time
    QUEST_COMPLETEOBJECTIVE:    double objectiveID
    QUEST_STARTQUEST:           double questID
    INVENTORY_ADDITEM:  GameObject item, int quantity
    INVENTORY_REMOVEITEM:   GameObject item, int quantity
    LOADLEVEL:      string level
    SPRITE_CHANGE:  GameObject spriteToChange
    NPC_TELEPORT:   GameObject npc, float x_coord, float y_coord
*/

//To access the arguments, use the Arg1...Arg5 properties. Use the custom editor to set these up.

using UnityEngine;

public class CutsceneAction : MonoBehaviour
{
    //You can rename these or add new ones to the end, but DO NOT REMOVE OR REARRANGE THEM
    //If you do, I will cut you.

    public enum CAction {
        CAMERA_MOVE,
        COMBAT_START,
        NPC_ANIMATE, NPC_FACE, NPC_MOVE, NPC_SPEAK,
        PLAYER_ANIMATE, PLAYER_FACE, PLAYER_MOVE, PLAYER_SPEAK,
        QUEST_COMPLETEOBJECTIVE, QUEST_STARTQUEST,
        INVENTORY_ADDITEM, INVENTORY_REMOVEITEM,
        GAMEOBJECT_ACTIVATE, GAMEOBJECT_DEACTIVATE,
        LOADLEVEL,
        SPRITE_CHANGE, NPC_TELEPORT
    };

    public double StartTime;
    public CAction Action;
    public object Arg1
    { 
        get
        {
            switch (Action)
            {
                case CAction.CAMERA_MOVE:
                case CAction.PLAYER_MOVE:
                    return f1;
                case CAction.PLAYER_ANIMATE:
                case CAction.PLAYER_SPEAK:
                    return s1;
                case CAction.PLAYER_FACE:
                    return i1;
                case CAction.QUEST_STARTQUEST:
                    return d1;
                default:
                    return g1;
            }
        }
    }
    public object Arg2
    {
        get
        {
            switch (Action)
            {
                case CAction.CAMERA_MOVE:
                case CAction.PLAYER_MOVE:
                    return f2;
                case CAction.NPC_ANIMATE:
                case CAction.NPC_SPEAK:
                case CAction.LOADLEVEL:
                    return s1;
                case CAction.NPC_FACE:
                case CAction.INVENTORY_ADDITEM:
                case CAction.INVENTORY_REMOVEITEM:
                    return i1;
                case CAction.NPC_MOVE:
                case CAction.NPC_TELEPORT:
                case CAction.PLAYER_SPEAK:
                    return f1;
                default:
                    return null;
            }
        }
    }
    public object Arg3
    {
        get
        {
            switch (Action)
            {
                case CAction.CAMERA_MOVE:
                case CAction.PLAYER_MOVE:
                    return f3;
                case CAction.NPC_MOVE:
                case CAction.NPC_TELEPORT:
                    return f2;
                case CAction.NPC_SPEAK:
                    return f1;
                case CAction.LOADLEVEL:
                    return i1;
                default:
                    return null;
            }
        }
    }
    public object Arg4
    {
        get
        {
            switch (Action)
            {
                case CAction.CAMERA_MOVE:
                case CAction.PLAYER_MOVE:
                    return f4;
                case CAction.NPC_MOVE:
                    return f3;
                default:
                    return null;
            }
        }
    }
    public object Arg5
    {
        get
        {
            switch (Action)
            {
                case CAction.NPC_MOVE:
                    return f4;
                default:
                    return null;
            }
        }
    }
    [SerializeField] private float f1;
    [SerializeField] private float f2;
    [SerializeField] private float f3;
    [SerializeField] private float f4;
    [SerializeField] private string s1;
    [SerializeField] private int i1;
    [SerializeField] private GameObject g1;
    [SerializeField] private double d1;
}
