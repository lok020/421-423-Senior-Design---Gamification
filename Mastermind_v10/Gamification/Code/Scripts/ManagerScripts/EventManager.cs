using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Game actions. Used as a first parameter in a list of objects - the following parameters depend on the action type
//You can add or rename actions, but DO NOT REMOVE OR REARRANGE ANY ACTIONS. This will break stuff and I will not be happy.
public enum GameAction
{
    IN_INVENTORY, LOOTED, PICKED_UP, CRAFTED, CONSUMED, TRADE_GIVE, TRADE_RECEIVED,
    DROPPED, DISCARDED, IN_AREA, QUEST_CONSUMED, TALK_TO, QUEST_MARKER, CUTSCENE_PLAYED, UNLOCKED, NPC_GIVE, NPC_RECEIVE,
    QUEST_COMPLETED, SECRET_AREA, MONSTER_KILLED, ATTACK_MELEE, ATTACK_MAGIC, ATTACK_RANGED, ATTACK_HEAL, MOUSE_CLICK,
    SPRITE_CHANGED
};


//Event manager. If an important event occurs, this will notify the quest and achievement systems. This is done with
//a List<object>() { GameAction, ... } where the subsequent info depends on the action taken (usually ints or strings)
public class EventManager : MonoBehaviour {

    private AchievementManager _achievementManager;
    private QuestManager _questManager;

	// Use this for initialization
	void Start () {
        _achievementManager = GetComponent<AchievementManager>();
        _questManager = GetComponent<QuestManager>();
	}

    //Notify all systems of the event
    public void Event(List<object> action)
    {
        //All events go to the achievement manager (it processes them quickly)
        _achievementManager.UpdateAchievements(action);
        //Some go to the quest manager (it processes them more slowly)
        //Skip achievement-only actions (update this list as necessary)
        switch((GameAction)action[0])
        {
            case GameAction.ATTACK_MAGIC:
            case GameAction.ATTACK_MELEE:
            case GameAction.ATTACK_RANGED:
            case GameAction.SECRET_AREA:
                return;
        }
        _questManager.UpdateQuests(action);
    }

    //Start quest
    public void StartQuest(double questId)
    {
        _questManager.StartQuest(questId);
    }
}
