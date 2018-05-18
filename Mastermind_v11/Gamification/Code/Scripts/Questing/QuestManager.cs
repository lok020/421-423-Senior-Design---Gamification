using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour {

    public List<Quest> Quests { get; set; }

    private Inventory inventory;
    private PlayerController player;
    private NetworkManager db;
    private MessageOverlayController mc;
    private EventManager em;
    
    /* This would be called when the user has logged in. First, it loads all the quests from a local database into the QuestManager.
       Then, it loads the current quest status from the server, and adjusts the quest state for each of the quests. All this relies
       on features that the game doesn't currently have, so for now it just creates a sample quest and activates it
    */
	void Start () {
        Quests = new List<Quest>();
        //Get link to player controller and inventory
        player = GetComponentInParent<PlayerController>();
        inventory = GetComponentInParent<Inventory>();
        em = GetComponent<EventManager>();
        //Get link to database - guaranteed to exist before this is created
        db = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        mc = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
        InitializeAllQuests();

        db.RetrievePlayerQuestProgress(this);

        //GetQuest(10).RestartQuest();

        //Unlock any quests if content lock has updated
        UnlockContentLockedQuests();
    }

    //Gets quests listed by quest state, will be used by UI
    public List<Quest> GetQuests(Quest.QuestState state)
    {
        List<Quest> questInfo = new List<Quest>();
        foreach(Quest q in Quests)
        {
            if(q.State == state)
            {
                questInfo.Add(q);
            }
        }
        return questInfo;
    }

    public void StartQuest(double questId)
    {
        Quest q = GetQuest(questId);
        if (q == null) return;
        if(q.State != Quest.QuestState.ACTIVE)
            q.StartQuest();
    }

    //Event must be in the form: ACTION [VAR...], where ACTION is a quest action and the vars are whatever type the action requires
    public void UpdateQuests(List<object> worldEvent)
    {
        foreach (Quest q in Quests)
        {
            if (q.State == Quest.QuestState.ACTIVE)
            {
                q.UpdateQuest(worldEvent);
            }
        }
    }

    public bool ObjectiveCompleted(double questId, double objectiveId)
    {
        Quest q = GetQuest(questId);
        if (q == null) return false;
        return q.ObjectiveWasCompleted(objectiveId);
    }

    public void LinkMessageController(MessageOverlayController controller)
    {
        foreach(Quest q in Quests)
        {
            q.LinkMessageController(controller);
        }
    }

    public void UpdateQuestFromDB(double questId, int state, int numCompletions, List<double> active, List<double> completed)
    {
        Quest q = GetQuest(questId);
        if (q == null) return;
        q.UpdateFromDB(state, numCompletions, active, completed);
        //Debug.Log("Updated ID " + questId + ", state " + state + ", numCompletions " + numCompletions + ", active[0] " + active[0] + ", completed[0] " + completed[0]);
    }

    private Quest GetQuest(double questId)
    {
        foreach(Quest q in Quests)
        {
            if (q.ID == questId)
                return q;
        }
        return null;
    }

    //Starts new quests if content lock has changed. None of these quests are repeatable
    private void UnlockContentLockedQuests()
    {
        //First populate a list of quests to verify that they have started
        //All of these are cumulative, so if Lock == 3, include all quests for 1 through 3
        List<double> QuestsToCheck = new List<double>();
        //New giant forest zone
        if(db.ContentLock >= 1)
        {
            //Find the forest!
            QuestsToCheck.Add(10);
        }
        //Mountain zone
        if(db.ContentLock >= 2)
        {
            //Nothing yet
        }

        //If any of these quests have never been activated, start them
        foreach(double id in QuestsToCheck)
        {
            Quest q = GetQuest(id);
            if (q == null) continue;
            if(q.State == Quest.QuestState.INACTIVE)
            {
                StartQuest(id);
            }
        }
    }

    /*''''''''''''''''''''''''''''''''''''''''''''''*\
    | Create all quests for the game in this section |
    | Make sure to add them to the below function!   |
    \*..............................................*/


    //Creates all quests in the game. This will eventually be replaced by a database or another more elegant solution
    private void InitializeAllQuests()
    {
        Quests.Add(TutorialQuest());
        Quests.Add(ForestQuest1());
        Quests.Add(ForestQuest2());
        Quests.Add(ForestQuest3());
        Quests.Add(ForestQuestRepeatable1());

        Quests.Add(Forest2Quest1());
    }

    //Creates a sample quest. This demonstrates the basic features of quest creation. It's kinda clunky, but it's a good first prototype
    private Quest CreateSampleQuest()
    {
        //First create the objectives
        //Stage 1 - go to the forest
        QuestObjective stage1 = new QuestObjective(1, false, "Go to the forest", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Forest" }, true);
        //Stage 2 - pick up one plant
        QuestObjective stage2 = new QuestObjective(2, false, "Pick up a leather scrap", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 19, 1 }, true);
        //Stage 3 - pick up one "pickup"
        QuestObjective stage3 = new QuestObjective(3, false, "Pick up an oak log", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 16, 1 }, true);
        //Stage 4 - speak to old man
        QuestObjective stage4 = new QuestObjective(4, false, "Return to the old man", "SUCCESS", new List<double>() { 2, 3 }, "AND", new List<object>() { GameAction.TALK_TO, "Old Man" }, true);

        //Add them to a list of objectives
        List<QuestObjective> objectives = new List<QuestObjective>() { stage1, stage2, stage3, stage4 };

        //Create a fallback objective
        FallbackObjective fb1 = new FallbackObjective(2, new List<double>() { 4 }, new List<object>() { GameAction.IN_INVENTORY, 19, 1 }, false);
        FallbackObjective fb2 = new FallbackObjective(3, new List<double>() { 4 }, new List<object>() { GameAction.IN_INVENTORY, 16, 1 }, false);

        //Add them to a list of fallback objectives
        List<FallbackObjective> fallbackObjectives = new List<FallbackObjective>() { fb1, fb2 };

        //Create a reward
        GameReward reward = new GameReward(0.0, 5, 10, 0, 0);

        //Add the rewards to a list of rewards
        List<GameReward> rewards = new List<GameReward>() { reward };

        //Create the quest
        Quest sampleQuest = new Quest(1, "Intro Quest", "Pick up items! Yay!", objectives, fallbackObjectives, rewards, player, inventory, db, mc, em);
        //Debug.Log("Created quest \"" + sampleQuest.Name + "\", description: " + sampleQuest.Description);
        return sampleQuest;
    }

    //Tutorial quest
    private Quest TutorialQuest()
    {
        //Objectives!
        //Finish first cutscene
        QuestObjective finishCutscene1 = new QuestObjective(0.1, true, "", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "TutorialCutscene01" }, true);
        //Walk on first waypoint
        QuestObjective waypoint1 = new QuestObjective(1.0, false, "Walk to next waypoint", "NO", new List<double>() { 0.1 }, "OR", new List<object>() { GameAction.QUEST_MARKER, "TutorialMarker1" }, true);
        //Walk on second waypoint
        QuestObjective waypoint2 = new QuestObjective(1.1, false, "Walk to next waypoint", "NO", new List<double>() { 1.0 }, "OR", new List<object>() { GameAction.QUEST_MARKER, "TutorialMarker2" }, true);
        //Walk on third waypoint
        QuestObjective waypoint3 = new QuestObjective(1.2, false, "Walk to next waypoint", "NO", new List<double>() { 1.1 }, "OR", new List<object>() { GameAction.QUEST_MARKER, "TutorialMarker3" }, true);
        //Walk on fourth waypoint
        QuestObjective waypoint4 = new QuestObjective(1.3, false, "Walk to next waypoint", "NO", new List<double>() { 1.2 }, "OR", new List<object>() { GameAction.QUEST_MARKER, "TutorialMarker4" }, true);
        //Escape jail
        QuestObjective finishCutscene2 = new QuestObjective(2, true, "", "NO", new List<double>() { 1.3 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "TutorialCutscene03" }, true);
        //Finish quest
        QuestObjective finishCutscene3 = new QuestObjective(3, true, "", "SUCCESS", new List<double>() { 2 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "TutorialCutscene04" }, true);

        //Add them to a list of objectives
        List<QuestObjective> questObjectives = new List<QuestObjective>() { finishCutscene1, waypoint1, waypoint2, waypoint3, waypoint4, finishCutscene2, finishCutscene3 };

        //Return the quest
        return new Quest(1.0, "Tutorial Quest", "Tutorial quest", questObjectives, null, null, player, inventory, db, mc, em);
    }

    //First real quest. Get some grapes in the forest for the old man's Potion of Recall
    private Quest ForestQuest1()
    {
        //Objectives!
        QuestObjective goToForest = new QuestObjective(1, false, "Go to the forest", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Forest" }, true);
        QuestObjective pickUpGrapes = new QuestObjective(2, false, "Collect some grapes", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 35, 1 }, true);
        QuestObjective talkToOldMan = new QuestObjective(3, false, "Return to the old man", "SUCCESS", new List<double>() { 2 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "TownCutscene02" }, true);

        //Create a fallback objective
        FallbackObjective lostGrapes = new FallbackObjective(2, new List<double>() { 3 }, new List<object>() { GameAction.IN_INVENTORY, 35, 1 }, false);

        //Create a reward
        GameReward reward1 = new GameReward(0, 25, 0, 17, 3); //3 bronze ore
        GameReward reward2 = new GameReward(0, 0, 0, 19, 3);  //3 leather scraps
        GameReward reward3 = new GameReward(0, 0, 0, 21, 1);  //1 thread

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToForest, pickUpGrapes, talkToOldMan };
        List<FallbackObjective> fallbacks = new List<FallbackObjective>() { lostGrapes };
        List<GameReward> rewards = new List<GameReward>() { reward1, reward2, reward3 };

        //Create the quest
        return new Quest(2, "Grape Collector", "Collect some grapes for a Potion of Recall", objectives, fallbacks, rewards, player, inventory, db, mc, em);
    }

    //Return to the forest and acquire the contents of the chest behind the southern gate
    private Quest ForestQuest2()
    {
        //Objectives
        QuestObjective goToForest = new QuestObjective(1, false, "Return to the forest", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Forest" }, true);
        QuestObjective unlockGate = new QuestObjective(2, false, "Unlock the south gate", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.UNLOCKED, "South Forest Gate" }, true);
        QuestObjective openChest = new QuestObjective(3, false, "Open the chest", "NO", new List<double>() { 2 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "ForestCutscene02" }, true);
        QuestObjective defeatChest = new QuestObjective(3.1, true, "", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 38, 1 }, true);
        QuestObjective returnToOldMan = new QuestObjective(4, false, "Return to the old man", "SUCCESS", new List<double>() { 3, 3.1 }, "AND", new List<object>() { GameAction.CUTSCENE_PLAYED, "TownCutscene03" }, true);
        
        //No fallback objectives

        //Rewards
        GameReward reward1 = new GameReward(0, 40, 0, 17, 2); //40 xp

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToForest, unlockGate, openChest, defeatChest, returnToOldMan };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create the quest
        return new Quest(3, "Chest Opener", "Collect the items in a chest in the south forest", objectives, null, rewards, player, inventory, db, mc, em);
    }

    //Go to the forest once more and fight a zombified bear, your toughest opponent yet!
    private Quest ForestQuest3()
    {
        //Objectives
        QuestObjective goToForest = new QuestObjective(1, false, "Return to the forest", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Forest" }, true);
        QuestObjective unlockGate = new QuestObjective(2, false, "Unlock the north gate", "NO", new List<double>() { 1 }, "OR", new List<object>() { GameAction.UNLOCKED, "North Forest Gate" }, true);
        QuestObjective exploreArea = new QuestObjective(3, false, "Explore the area", "NO", new List<double>() { 2 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "ForestCutscene03" }, true);
        QuestObjective defeatBear = new QuestObjective(4, false, "Defeat the bear", "NO", new List<double>() { 3 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 39, 1 }, true);
        QuestObjective returnToOldMan = new QuestObjective(5, false, "Return to the old man", "SUCCESS", new List<double>() { 4 }, "OR", new List<object>() { GameAction.CUTSCENE_PLAYED, "TownCutscene04" }, true);

        //No fallback objectives

        //Reward the player with tons of XP
        GameReward reward1 = new GameReward(0, 200, 0, 0, 0);

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToForest, unlockGate, exploreArea, defeatBear, returnToOldMan };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create the quest
        return new Quest(4, "Forest Explorer", "Find the secrets of the north forest", objectives, null, rewards, player, inventory, db, mc, em);
    }

    //Collect up to five potions for gold
    private Quest ForestQuestRepeatable1()
    {
        //Objectives
        QuestObjective collectPotion = new QuestObjective(1, false, "Find potions for the old man", "SUCCESS", new List<double>() { 0 }, "OR", new List<object>() { GameAction.NPC_RECEIVE, 7, 1 }, true);
        QuestObjective got1 = new QuestObjective(1.1, false, "Collect a health potion (0/5)", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, true);
        QuestObjective ret1 = new QuestObjective(1.2, false, "Return to the old man (0/5)", "NO", new List<double>() { 1.1 }, "OR", new List<object>() { GameAction.NPC_GIVE, 2, 1 }, true);
        QuestObjective got2 = new QuestObjective(1.3, false, "Collect a health potion (1/5)", "NO", new List<double>() { 1.2 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, true);
        QuestObjective ret2 = new QuestObjective(1.4, false, "Return to the old man (1/5)", "NO", new List<double>() { 1.3 }, "OR", new List<object>() { GameAction.NPC_GIVE, 2, 1 }, true);
        QuestObjective got3 = new QuestObjective(1.5, false, "Collect a health potion (2/5)", "NO", new List<double>() { 1.4 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, true);
        QuestObjective ret3 = new QuestObjective(1.6, false, "Return to the old man (2/5)", "NO", new List<double>() { 1.5 }, "OR", new List<object>() { GameAction.NPC_GIVE, 2, 1 }, true);
        QuestObjective got4 = new QuestObjective(1.7, false, "Collect a health potion (3/5)", "NO", new List<double>() { 1.6 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, true);
        QuestObjective ret4 = new QuestObjective(1.8, false, "Return to the old man (3/5)", "NO", new List<double>() { 1.7 }, "OR", new List<object>() { GameAction.NPC_GIVE, 2, 1 }, true);
        QuestObjective got5 = new QuestObjective(1.9, false, "Collect a health potion (4/5)", "NO", new List<double>() { 1.8 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, true);
        QuestObjective ret5 = new QuestObjective(1.99, false, "Return to the old man (4/5)", "NO", new List<double>() { 1.9 }, "OR", new List<object>() { GameAction.NPC_GIVE, 2, 1 }, true);

        //Fallback objectives
        FallbackObjective fo1 = new FallbackObjective(1.1, new List<double>() { 1.2 }, new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, false);
        FallbackObjective fo2 = new FallbackObjective(1.3, new List<double>() { 1.4 }, new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, false);
        FallbackObjective fo3 = new FallbackObjective(1.5, new List<double>() { 1.6 }, new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, false);
        FallbackObjective fo4 = new FallbackObjective(1.7, new List<double>() { 1.8 }, new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, false);
        FallbackObjective fo5 = new FallbackObjective(1.9, new List<double>() { 1.99 }, new List<object>() { GameAction.IN_INVENTORY, 2, 1 }, false);

        //The player will be rewarded throughout dialog, but throw in some XP
        GameReward reward1 = new GameReward(0, 50, 0, 0, 0);

        //Add the above to lists
        List<QuestObjective> objectives = new List<QuestObjective>() { collectPotion, got1, ret1, got2, ret2, got3, ret3, got4, ret4, got5, ret5 };
        List<FallbackObjective> fallbackObjectives = new List<FallbackObjective>() { fo1, fo2, fo3, fo4, fo5 };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create quest
        return new Quest(5, "Potion Crafter", "Craft some potions for the old man", objectives, fallbackObjectives, rewards, player, inventory, db, mc, em);
    }

    //-------------------------------------
    // STAGE 2: New forest zones
    //-------------------------------------

    //Go to graveyard
    private Quest Forest2Quest1()
    {
        //Objectives
        QuestObjective talkToOldFart = new QuestObjective(1, false, "Talk to old man", "NO", new List<double> { 0 }, "OR",
            new List<object>() { GameAction.CUTSCENE_PLAYED, "TownCutscene05" }, true);
        QuestObjective goToForest2 = new QuestObjective(2, false, "Go to the new forest area", "NO", new List<double>() { 1 }, "OR",
            new List<object>() { GameAction.IN_AREA, "Forest 2" }, true);
        QuestObjective exploreForest = new QuestObjective(2.1, false, "Find the graveyard", "NO", new List<double>() { 2 }, "OR",
            new List<object>() { GameAction.CUTSCENE_PLAYED, "Forest2Cutscene01" }, true);
        QuestObjective findWayAround = new QuestObjective(3, false, "Find another way across the river", "NO", new List<double>() { 2.1 }, "OR",
            new List<object>() { GameAction.CUTSCENE_PLAYED, "Forest2Cutscene02" }, true);
        QuestObjective getLogs1 = new QuestObjective(4.1, false, "Collect maple logs (1/4)", "NO", new List<double>() { 3 }, "OR",
            new List<object>() { GameAction.IN_INVENTORY, 62, 1 }, true);
        QuestObjective getLogs2 = new QuestObjective(4.2, false, "Collect maple logs (2/4)", "NO", new List<double>() { 4.1 }, "OR",
            new List<object>() { GameAction.IN_INVENTORY, 62, 2 }, true);
        QuestObjective getLogs3 = new QuestObjective(4.3, false, "Collect maple logs (3/4)", "NO", new List<double>() { 4.2 }, "OR",
            new List<object>() { GameAction.IN_INVENTORY, 62, 3 }, true);
        QuestObjective getLogs4 = new QuestObjective(4.4, false, "Collect maple logs (4/4)", "NO", new List<double>() { 4.3 }, "OR",
            new List<object>() { GameAction.IN_INVENTORY, 62, 4 }, true);
        QuestObjective fixBridgeStart = new QuestObjective(5, false, "Fix the bridge", "NO", new List<double>() { 4.4 }, "OR",
            new List<object>() { GameAction.SPRITE_CHANGED, "Forest2WoodenBridge" }, true);
        QuestObjective fixBridge = new QuestObjective(5.1, true, "", "NO", new List<double>() { 5 }, "OR",
            new List<object>() { GameAction.CUTSCENE_PLAYED, "Forest2Cutscene03" }, true);
        QuestObjective goToGraveyard = new QuestObjective(6, false, "Go to the graveyard", "NO", new List<double>() { 5.1 }, "OR",
            new List<object>() { GameAction.CUTSCENE_PLAYED, "Forest2Cutscene04" }, true);

        //Fallback objectives for the maple logs
        FallbackObjective fo1 = new FallbackObjective(4.1, new List<double>() { 4.2 }, new List<object>() { GameAction.IN_INVENTORY, 62, 1 }, false);
        FallbackObjective fo2 = new FallbackObjective(4.2, new List<double>() { 4.3 }, new List<object>() { GameAction.IN_INVENTORY, 62, 2 }, false);
        FallbackObjective fo3 = new FallbackObjective(4.3, new List<double>() { 4.4 }, new List<object>() { GameAction.IN_INVENTORY, 62, 3 }, false);
        FallbackObjective fo4 = new FallbackObjective(4.4, new List<double>() { 5 }, new List<object>() { GameAction.IN_INVENTORY, 62, 4 }, false);

        //Basic XP reward
        GameReward reward1 = new GameReward(0, 50, 0);

        //Lists for the above
        List<QuestObjective> objectives = new List<QuestObjective>() { talkToOldFart, goToForest2, exploreForest, findWayAround,
            getLogs1, getLogs2, getLogs3, getLogs4, fixBridgeStart, fixBridge, goToGraveyard };
        List<FallbackObjective> fallbacks = new List<FallbackObjective>() { fo1, fo2, fo3, fo4 };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create the quest
        return new Quest(10, "Deep Forest", "Explore the forest", objectives, fallbacks, rewards, player, inventory, db, mc, em);
    }

    //Quick quest to find the mountain
    private Quest MountainQuest1()
    {
        //Objectives!
        QuestObjective goToMountain = new QuestObjective(1, false, "Find the Mountain", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Moutain" }, true);

        //Create a reward
        GameReward reward1 = new GameReward(0, 25, 0, 2, 5); //5 hp pots

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToMountain };
        List<FallbackObjective> fallbacks = new List<FallbackObjective>() { };
        List<GameReward> rewards = new List<GameReward>() { reward1};

        //Create the quest
        return new Quest(2, "Mountain Explorer", "Find the mountain", objectives, fallbacks, rewards, player, inventory, db, mc, em);
    }

    //Quest to gather firewood for the forges
    private Quest MountainQuest2()
    {
        //Objectives!
        QuestObjective goToMountain = new QuestObjective(1, false, "Go to the Mountain", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Moutain" }, true);
        QuestObjective pickUpMaple = new QuestObjective(2, false, "Collect some maple logs for the furnaces", "YES", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 62, 3 }, true);
        QuestObjective ret1 = new QuestObjective(3, false, "Give logs to the old man", "SUCESS", new List<double>() { 3 }, "OR", new List<object>() { GameAction.NPC_GIVE, 62, 3 }, true);

        //Create a fallback objective
        FallbackObjective lostMaple = new FallbackObjective(2, new List<double>() { 3 }, new List<object>() { GameAction.IN_INVENTORY, 62, 3 }, false);

        //Create a reward
        GameReward reward1 = new GameReward(0, 50, 25, 20 , 3); //3 runestone shards

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToMountain, pickUpMaple, ret1 };
        List<FallbackObjective> fallbacks = new List<FallbackObjective>() { lostMaple };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create the quest
        return new Quest(2, "Maple for Furnace", "Collect some maple logs for the Old Man", objectives, fallbacks, rewards, player, inventory, db, mc, em);
    }

    private Quest MountainQuest3()
    {
        //Objectives!
        QuestObjective goToMountain = new QuestObjective(1, false, "Go to the Mountain", "NO", new List<double>() { 0 }, "OR", new List<object>() { GameAction.IN_AREA, "Moutain" }, true);
        QuestObjective pickUpCopper = new QuestObjective(2, false, "Collect some copper ore to make the key", "YES", new List<double>() { 1 }, "OR", new List<object>() { GameAction.IN_INVENTORY, 63, 2 }, true);
        QuestObjective ret1 = new QuestObjective(3, false, "Give ore to the old man", "SUCESS", new List<double>() { 3 }, "OR", new List<object>() { GameAction.NPC_GIVE, 63, 2 }, true);

        //Create a fallback objective
        FallbackObjective lostCopper = new FallbackObjective(2, new List<double>() { 3 }, new List<object>() { GameAction.IN_INVENTORY, 63, 2 }, false);

        //Create a reward
        GameReward reward1 = new GameReward(0, 50, 25, 20, 3); //need to change this to a key to give the quest to get the runestone

        //Lists for all the above
        List<QuestObjective> objectives = new List<QuestObjective>() { goToMountain, pickUpCopper, ret1 };
        List<FallbackObjective> fallbacks = new List<FallbackObjective>() { lostCopper };
        List<GameReward> rewards = new List<GameReward>() { reward1 };

        //Create the quest
        return new Quest(2, "Copper for key", "Collect some copper to make a new key", objectives, fallbacks, rewards, player, inventory, db, mc, em);
    }
}
