using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Quest
{
    //Public enums
    //State of the quest: not active, active, completed, failed
    public enum QuestState { INACTIVE, ACTIVE, COMPLETED, FAILED };

    //Variables
    public double ID { get; private set; }                      //ID number
    public string Name { get; private set; }                    //Name
    public string Description { get; private set; }             //Description
    public QuestState State { get
        {
            return _state;
        }
        private set
        {
            _state = value;
            _databaseManager.QuestUpdateState(ID, (int)value);
        }
    }               //Quest state: inactive, active, completed, failed
    public int NumberOfCompletions { get
        {
            return _numberOfCompletions;
        }
        private set
        {
            _numberOfCompletions = value;
            _databaseManager.QuestUpdateCompletionCount(ID, value);
        }
    }        //Number of completions, used for repeatable quests
    public List<GameReward> QuestRewards { get; private set; } //List of possible rewards for this quest

    private QuestState _state;
    private int _numberOfCompletions;
    private List<QuestObjective> _inactive, _active, _completed; //All quest objectives, split across three lists
    private List<FallbackObjective> _activeFallback, _inactiveFallback;    //All fallback objectives
    private PlayerController _playerController;
    private Inventory _playerInventory;
    private MessageOverlayController _messageController;
    private NetworkManager _databaseManager;
    private EventManager _eventManager;
    private List<string> _messagesToDisplay;

    //Initializer, accepts arguments for the above public variables
    public Quest(double id, string name, string description, List<QuestObjective> objectives, List<FallbackObjective> fallbackObjectives,
        List<GameReward> questRewards, PlayerController linkToPlayerController, Inventory linkToPlayerInventory, NetworkManager linkToDatabaseManager, MessageOverlayController linkToMessageController,
        EventManager linkToEventManager)
    {
        //Initialize variables
        ID = id;
        Name = name;
        Description = description;
        _state = QuestState.INACTIVE;
        _numberOfCompletions = 0;
        QuestRewards = questRewards;
        _inactive = objectives;
        _active = new List<QuestObjective>();
        _completed = new List<QuestObjective>();
        _inactiveFallback = fallbackObjectives ?? new List<FallbackObjective>();    //Creates empty list if passed parameter is null
        _activeFallback = new List<FallbackObjective>();
        _playerController = linkToPlayerController;
        _playerInventory = linkToPlayerInventory;
        _databaseManager = linkToDatabaseManager;
        _eventManager = linkToEventManager;
        _messageController = linkToMessageController;
        _messagesToDisplay = new List<string>();
    }

    public void StartQuest()
    {
        //Mark quest as active
        State = QuestState.ACTIVE;

        //This is the first time starting the quest
        if(_completed.Count == 0)
        {
            //Add stage 0 quest to _active
            QuestObjective stage0 = new QuestObjective(0.0, true, "", "NO", null, "OR",
                new List<object> { true }, true);
            _active.Add(stage0);
            _databaseManager.QuestUpdateObjective(ID, 0, 'i', 'a');
        }
        //This is a repeat quest
        else
        {
            //Move all quest objectives to _inactive
            _inactive.AddRange(_active);
            _inactive.AddRange(_completed);

            //Reset fallback objectives as well
            _inactiveFallback.AddRange(_activeFallback);

            //Clear the active and completed lists of objectives db side
            _databaseManager.QuestClearObjectives(ID);

            //Find stage 0 in _inactive and activate it
            QuestObjective stage0 = null;
            foreach(QuestObjective qo in _inactive)
            {
                if(qo.ID == 0.0)
                {
                    stage0 = qo;
                    break;
                }
            }
            if(stage0 == null)
            {
                //Add stage 0 quest to _active
                stage0 = new QuestObjective(0.0, true, "", "NO", null, "OR",
                    new List<object> { true }, true);
                _active.Add(stage0);
                _databaseManager.QuestUpdateObjective(ID, 0, 'i', 'a');
            }

            ActivateObjective(stage0);
        }

        //Notify the player that the quest has started
        _messageController.EnqueueMessage("Starting quest: " + Name);
        
        //Start the quest
        UpdateQuest(null);
    }

    //Only used in testing for quick restarts
    public void RestartQuest()
    {
        //Move all quest objectives to _inactive
        _inactive.AddRange(_active);
        _inactive.AddRange(_completed);
        
        //Reset fallback objectives as well
        _inactiveFallback.AddRange(_activeFallback);

        //Clear the various lists
        _active.Clear();
        _completed.Clear();
        _activeFallback.Clear();

        //Find stage 0 in _inactive and activate it
        QuestObjective stage0 = null;
        foreach (QuestObjective qo in _inactive)
        {
            if (qo.ID == 0.0)
            {
                stage0 = qo;
                break;
            }
        }

        //Clear the active and completed lists of objectives db side
        _databaseManager.QuestClearObjectives(ID);

        ActivateObjective(stage0);

        //Notify the player that the quest has started
        _messageController.EnqueueMessage("Starting quest: " + Name);

        //Start the quest
        UpdateQuest(null);
    }

    //Received from QuestManager, this updates the quest depending on the world event received
    public void UpdateQuest(List<object> worldEvent)
    {
        //First update all the objectives
        UpdateObjectivesLoop(worldEvent);

        //Then update the fallback objectives
        bool b = UpdateFallbackObjectives(worldEvent);
        
        //If no fallback objectives were completed, enqueue all messages in the MessagesToDisplay list
        if(!b)
        {
            foreach(string message in _messagesToDisplay)
            {
                _messageController.EnqueueMessage(message);
            }
        }
        _messagesToDisplay.Clear();

        //Distribute rewards if the quest was completed

        //If any quests were completed successfully, distribute the rewards
        if (State == QuestState.COMPLETED)
        {
            _databaseManager.DBCompleteQuest();
            if (QuestRewards != null)
            {
                foreach (GameReward reward in QuestRewards)
                {
                    if (ObjectiveWasCompleted(reward.RequiredObjective))
                    {
                        _playerController.AddXP(reward.XPReward);
                        _playerInventory.AddGold(reward.GoldReward);
                        _databaseManager.DBGetQuestRewards(reward.ItemRewardCount);
                        if (reward.ItemReward > 0)
                        {
                            _playerInventory.AddItemToInventory(reward.ItemReward, reward.ItemRewardCount);
                        }
                    }
                }
            }
            _eventManager.Event(new List<object>() { GameAction.QUEST_COMPLETED, ID });
        }
    }

    //Get list of current objective descriptions
    public void GetActiveObjectives()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < _active.Count; i++)
        {
            sb.Append(_active[i].ID.ToString());
            if (i < _active.Count - 1)
            {
                sb.Append(", ");
            }
        }
        //Debug.Log("Current objective" + (_active.Count > 1 ? "s: " : ": ") + sb.ToString());
    }

    //Get list of current objective descriptions
    public void GetCompletedObjectives()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < _completed.Count; i++)
        {
            sb.Append(_completed[i].ID.ToString());
            if (i < _completed.Count - 1)
            {
                sb.Append(", ");
            }
        }
        //Debug.Log("Completed objective" + (_completed.Count > 1 ? "s: " : ": ") + sb.ToString());
    }

    //Get whether the given objective was completed
    public bool ObjectiveWasCompleted(double id)
    {
        var completedObjectiveIDs = from qo in _completed select qo.ID;
        return completedObjectiveIDs.Contains(id);
    }

    //====== HELPER FUNCTIONS ========

    //Looping function that recursively updates objectives until there are no more changes to be made
    private void UpdateObjectivesLoop(List<object> worldEvent) { 
        //First see if this completes any active objectives; if so, add them to a list
        List<QuestObjective> justCompleted = new List<QuestObjective>();
        foreach (QuestObjective qo in _active)
        {
            if (ConditionCompleted(qo.Condition, worldEvent, qo.CompleteConditionIfTrue))
            {
                //Debug.Log("Completed " + qo.ID);
                justCompleted.Add(qo);
            }
        }
        //If any objectives were completed, process them, otherwise skip ahead
        if (justCompleted.Count > 0)
        {
            //Move the newly completed objectives from _active to _completed. 
            foreach (QuestObjective qo in justCompleted)
            {
                CompleteObjective(qo);
                //Notify the user that this objective was completed, if it's not hidden
                if(!qo.Hidden)
                {
                    _messageController.EnqueueMessage("Completed: " + qo.Description);
                }
                //If this completes the quest, complete it now and notify the user
                if (qo.Completes == QuestObjective.QuestObjectiveCompletion.FAIL)
                {
                    State = QuestState.FAILED;
                    _messagesToDisplay.Add("Quest '" + Name + "' failed! :(");
                }
                if (qo.Completes == QuestObjective.QuestObjectiveCompletion.SUCCESS)
                {
                    State = QuestState.COMPLETED;
                    NumberOfCompletions++;
                    _messagesToDisplay.Add("Quest '" + Name + "' complete!");
                    if (NumberOfCompletions > 1)
                    {
                        _messagesToDisplay.Add(" Number of completions: " + NumberOfCompletions);
                    }
                }
            }

            //Now check every single inactive objective to see if it can be activated
            List<QuestObjective> canBeActivated = new List<QuestObjective>();
            foreach(QuestObjective qo in _inactive)
            {
                if(CanBeActivated(qo))
                {
                    canBeActivated.Add(qo);
                }
            }

            //Move these newly activated objectives from _inactive to _active and notify the user
            foreach(QuestObjective qo in canBeActivated)
            {
                ActivateObjective(qo);
                _messagesToDisplay.Add(qo.Description);
            }

            //Finally, trigger an update for all the newly added objectives, in case they are set to auto-complete
            foreach(QuestObjective qo in canBeActivated)
            {
                ConditionCompleted(qo.Condition, worldEvent, qo.CompleteConditionIfTrue);
            }
        }
        if(justCompleted.Count > 0 && State == QuestState.ACTIVE)
            GetActiveObjectives();
    }

    //Controls fallback objectives. Activates and deactivates them, and processes them as necessary
    //Returns true if any fallback objectives were completed
    private bool UpdateFallbackObjectives(List<object> worldEvent)
    {
        var activeObjectiveIDs = from qo in _active select qo.ID;
        bool r = false;

        //Start by finding if any inactive ones should be activated
        List<FallbackObjective> tempFallbackList = new List<FallbackObjective>();
        foreach (FallbackObjective fo in _inactiveFallback)
        {
            if (fo.AffectedIDs.Intersect(activeObjectiveIDs).Any())
            {
                tempFallbackList.Add(fo);
            }
        }

        //If so, activate them
        foreach (FallbackObjective fo in tempFallbackList)
        {
            _inactiveFallback.Remove(fo);
            _activeFallback.Add(fo);
            //Debug.Log("Activated fallback objective!");
        }

        //Then see if any active ones should be deactivated
        tempFallbackList.Clear();
        foreach (FallbackObjective fo in _activeFallback)
        {
            if (!fo.AffectedIDs.Intersect(activeObjectiveIDs).Any())
            {
                tempFallbackList.Add(fo);
            }
        }

        //If so, deactivate them
        foreach (FallbackObjective fo in tempFallbackList)
        {
            _activeFallback.Remove(fo);
            _inactiveFallback.Add(fo);
            //Debug.Log("Deactivated fallback objective!");
        }

        tempFallbackList.Clear();
        //If no condition was specified, exit now
        if (worldEvent == null) return false;
        //Check every active fallback objective to see if their condition is completed. If so, record them
        foreach (FallbackObjective fo in _activeFallback)
        {
            if (ConditionCompleted(fo.Condition, worldEvent, fo.CompleteIfTrue))
            {
                tempFallbackList.Add(fo);
                //Debug.Log("Activated fallback objective condition is true.");
            }
        }
        //If any fallback objectives failed, process them
        foreach (FallbackObjective fo in tempFallbackList)
        {
            r = true;
            //Debug.Log("Deactivating objectives:");
            //Deactivate all objectives on the AffectedIDs list
            foreach (double id in fo.AffectedIDs)
            {
                QuestObjective qo = _active.Find(x => x.ID == id);
                //qo is in _active
                if(qo != null)
                {
                    _active.Remove(qo);
                    _inactive.Add(qo);
                    _databaseManager.QuestUpdateObjective(ID, qo.ID, 'a', 'i');
                }
                //qo is in _completed
                else
                {
                    qo = _completed.Find(x => x.ID == id);
                    _completed.Remove(qo);
                    _inactive.Add(qo);
                    _databaseManager.QuestUpdateObjective(ID, qo.ID, 'c', 'i');
                }
                //Debug.Log(id);
            }
            //Activate the fallback objective if it isn't already active
            //Debug.Log("Activating objective: " + fo.FallbackID);
            QuestObjective fallback = _completed.Find(x => x.ID == fo.FallbackID);
            if (fallback == null) continue;
            ActivateObjective(fallback);
            _messageController.EnqueueMessage(fallback.Description);
        }
        return r;
    }

    //Used to evaluate whether a worldEvent completed the objective
    //Objective format: (bool) or (QuestObjectiveAction, var)
    private bool ConditionCompleted(List<object> objectiveEvent, List<object> worldEvent, bool completeIfTrue)
    {
        //First case, first parameter is a bool, it'll either be true or false
        if (objectiveEvent[0] is bool)
        {
            bool b = (bool) objectiveEvent[0];
            return b == completeIfTrue;     //Objective is completed if it's true and completes on true, or false and completes on false
        }
        if (worldEvent == null) return false;

        //Second case, first parameter is a QuestObjectiveAction, following parameters depend on the action typebool value = false;
        bool value = false;
        GameAction action = (GameAction)objectiveEvent[0];
        GameAction worldAction = (GameAction)worldEvent[0];

        //Only bother checking if they are the right kind of action (for example, don't check the inventory if the player just entered a new area)
        //The one exception is IN_INVENTORY, which should be checked after every action that affects the inventory (which is almost all of them)
        if (action == worldAction || action == GameAction.IN_INVENTORY)
        {
            //Now check each kind of action
            switch (action)
            {
                //Crafted - checks that the right item was crafted and >= the required quantity were crafted
                //Destroyed - correct item and quantity were destroyed
                //Dropped - correct item and quantity were dropped
                //Looted - correct item and quantity were looted (obtained from world, monster drop, item chest, etc)
                //NPC traded - added or removed by an NPC during conversation
                //Quest consumed - special type of action used for destroying "Quest" objects, works the same as the other actions
                //Traded - correct item and quantity were either traded away or received from trade
                //All of these have the same exact logic
                case GameAction.CRAFTED:
                case GameAction.CONSUMED:
                case GameAction.DISCARDED:
                case GameAction.DROPPED:           //Not implemented
                case GameAction.LOOTED:            //Not implemented
                case GameAction.NPC_GIVE:
                case GameAction.NPC_RECEIVE:
                case GameAction.PICKED_UP:
                case GameAction.QUEST_CONSUMED:    //Not implemented
                case GameAction.TRADE_GIVE:        //Not implemented
                case GameAction.TRADE_RECEIVED:    //Not implemented
                    {
                        int objId = (int)objectiveEvent[1];
                        int objQuantity = (int)objectiveEvent[2];
                        int worldId = (int)worldEvent[1];
                        int worldQuantity = (int)worldEvent[2];
                        if (objId == worldId && objQuantity <= worldQuantity) { value = true; }
                        break;
                    }
                //Cutscene played - cutscene of given name has finished
                //In area - player is in the scene with the given name
                //Quest marker - player has collided with marker of given name
                //Talk to - simply talk to the NPC
                //Unlocked - lock unlocked of given name
                case GameAction.CUTSCENE_PLAYED:
                case GameAction.IN_AREA:
                case GameAction.QUEST_MARKER:
                case GameAction.TALK_TO:
                case GameAction.UNLOCKED:
                    {
                        string objArea = (string)objectiveEvent[1];
                        string worldArea = (string)worldEvent[1];
                        if (objArea == worldArea) { value = true; }
                        break;
                    }
                //In inventory - simply checks if the right item and quantity are currently in the player's inventory
                case GameAction.IN_INVENTORY:
                    {
                        int objId = (int)objectiveEvent[1];
                        int objQuantity = (int)objectiveEvent[2];
                        if (_playerInventory.ItemInInventory(objId) && _playerInventory.ItemCount(objId) >= objQuantity) { value = true; }
                        break;
                    }
            } //end switch
        } //end if

        return value == completeIfTrue;  //Returns true if value is true and objective completes on true, or value is false and it completes on false
    }

    //Checks if an objective can be activated
    private bool CanBeActivated(QuestObjective qo)
    {
        bool anyFound = false;
        foreach (double id in qo.RequiredObjectives)
        {
            bool found = false;
            foreach (QuestObjective q in _completed)
            {
                if (q.ID == id)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                anyFound = true;
            }
            if (found && qo.RequiredObjectiveRelationship == QuestObjective.QuestObjectiveEval.OR)
            {
                return true;
            }
            if(!found && qo.RequiredObjectiveRelationship == QuestObjective.QuestObjectiveEval.AND)
            {
                return false;
            }
        }
        return anyFound;    //If this stage is reached, either it's an AND and all were found, or it's an OR and none were found
    }

    //Activate objective
    private void ActivateObjective(QuestObjective qo)
    {
        _inactive.Remove(qo);
        _active.Add(qo);
        _databaseManager.QuestUpdateObjective(ID, qo.ID, 'i', 'a');
    }

    //Complete objective
    private void CompleteObjective(QuestObjective qo)
    {
        _active.Remove(qo);
        _completed.Add(qo);
        _databaseManager.QuestUpdateObjective(ID, qo.ID, 'a', 'c');
    }

    //Links quest to message overlay controller
    public void LinkMessageController(MessageOverlayController controller)
    {
        _messageController = controller;
    }

    //Loads quest state from database
    public void UpdateFromDB(int state, int numCompletions, List<double> active, List<double> completed)
    {
        //State and NumberOfCompletions are easy to update
        _state = (Quest.QuestState)state;
        _numberOfCompletions = numCompletions;
        //If objective 0 isn't in the list, add it
        bool addObjective0 = true;
        foreach(QuestObjective qo in _inactive)
        {
            if(qo.ID == 0)
            {
                addObjective0 = false;
                break;
            }
        }
        if(addObjective0)
        {
            _inactive.Add(new QuestObjective(0.0, true, "", "NO", null, "OR",
                new List<object> { true }, true));
        }
        //Move all active objectives from _inactive to _active
        List<QuestObjective> temp = new List<QuestObjective>();
        foreach(QuestObjective qo in _inactive)
        {
            if(active.Contains(qo.ID))
            {
                temp.Add(qo);
            }
        }
        foreach(QuestObjective qo in temp)
        {
            _inactive.Remove(qo);
            _active.Add(qo);
        }
        //Move all complete objectives from _inactive to _completed
        temp.Clear();
        foreach (QuestObjective qo in _inactive)
        {
            if (completed.Contains(qo.ID))
            {
                temp.Add(qo);
            }
        }
        foreach (QuestObjective qo in temp)
        {
            _inactive.Remove(qo);
            _completed.Add(qo);
        }
        //Update fallback objectives
        UpdateFallbackObjectives(null);
    }
}
