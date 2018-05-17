using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Cutscene : MonoBehaviour {

    public string Name;
    public double LengthInSeconds = 0;          //How long the cutscene will last
    public double StartLength = 1.0, EndLength = 1.0;       //Length of transition time, for starting and ending
    public List<GameObject> CutsceneObjects = new List<GameObject>();   //List of game objects used by the cutscene. These will not be hidden when the scene starts
    public float TargetZoom = 5;

    public double RequiredQuestID, RequiredObjectiveID, DontRunAfterQuestID, DontRunAfterObjectiveID;

    private int _state;                 //State. 0: not running, 1: starting, 2: running, 3: ending
    private int _listHead;
    private double _timeRemaining;
    private List<CutsceneAction> Actions;
    private List<GameObject> _disabledObjects;
    private GameObject cam;
    private float OldZoom;
    private bool NotifyQuestManagerUponCompletion;
    private GameObject player;
    private GameObject CombatDetails;
    private string LevelToLoadOnEnd;
    private SceneManager LevelSceneManager;
    private int LevelSpawnPointID;

    // Use this for initialization
    void Start () {
        _state = 0;     //Not running
        _timeRemaining = 0;
        _disabledObjects = new List<GameObject>();
        //Add all attached CutsceneActions to a local list
        Actions = new List<CutsceneAction>();
        Actions.AddRange(GetComponents<CutsceneAction>());
        NotifyQuestManagerUponCompletion = false;
        CombatDetails = null;
        LevelToLoadOnEnd = null;
	}

    //Start cutscene
    public void StartCutscene()
    {
        if (_state != 0) return;
        //Debug.Log("Starting cutscene!");
        //Set up cutscene
        //Resest the pointer to the list head
        _listHead = 0;
        //Clearn _disabledObjects
        _disabledObjects.Clear();
        //Sort Actions so the elements are all in order of when they will be triggered
        Actions.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
        //Set state
        _state = 1;
        //Set time remaining, for fade in
        _timeRemaining = StartLength;

        //Set up world
        //Disable player movement
        player = GameObject.FindGameObjectWithTag("Player");
        var playerController = player.GetComponent<PlayerController>();
        playerController.InCutscene = true;
        playerController.MovementVector = new Vector3(0, 0, 0);
        playerController.UpdatePlayerLabel();
        //Move player to scene trigger
        Vector2 playerMove = new Vector2(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
        float distance = playerMove.magnitude;
        playerController.MovementVector = new Vector3(playerMove.x, playerMove.y, 0);
        playerController.ScriptedMovementSpeed = (float) (distance / StartLength);
        playerController.ScriptedTimeRemaining = (float) StartLength;
        //Center camera on the scene trigger
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam != null)
        { 
            //Pan out camera
            if (cam.GetComponent<CameraFollow>() != null)
            {
                cam.GetComponent<Transform>().position = transform.position;
                cam.GetComponent<CameraFollow>().SetTarget(transform);
                //Get current zoom
                OldZoom = cam.GetComponent<Camera>().orthographicSize;
                //Start zooming camera to correct spot
                cam.GetComponent<CameraFollow>().SetTargetZoom(TargetZoom);
            }
        }

        //Hide all game objects in the scene that aren't marked for use
        foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>()) {
            if ((g.CompareTag("NPC") || g.CompareTag("Pickup") || g.CompareTag("CombatPortal")) == false) continue;
            if (!CutsceneObjects.Contains(g))
            {
                g.SetActive(false);
                _disabledObjects.Add(g);
            }
            //Disable any current dialog being spoken
            else
            {
                var dialogController = g.GetComponentInChildren<DialogController>();
                if (dialogController != null)
                {
                    dialogController.Speak("", 0);
                }
            }
        }
    } 

    //Ends the cutscene and resumes normal gameplay
    private void EndCutscene()
    {
        //End cutscene
        //Set state
        _state = 0;

        //Change world back to normal
        //Enable player movement
        var playerController = player.GetComponent<PlayerController>();
        playerController.InCutscene = false;
        playerController.UpdatePlayerLabel();
        //Re-enable all disabled objects
        foreach (GameObject g in _disabledObjects)
        {
            g.SetActive(true);
        }
        //Notify quest manager if needed
        if(NotifyQuestManagerUponCompletion)
        {
            var eventManager = player.GetComponent<EventManager>();
            eventManager.Event(new List<object>() { GameAction.CUTSCENE_PLAYED, Name });
            //Debug.Log("Notifying completion!");
        }
        //Load a level if one was specified
        if(LevelToLoadOnEnd != null)
        {
            playerController.TriggerSceneChange(LevelSceneManager, LevelToLoadOnEnd, LevelSpawnPointID);
        }
        //Lastly, trigger combat if necessary
        if(CombatDetails != null)
        {
            GameObject copyOfCombatDetails = Instantiate(CombatDetails);
            playerController.TriggerCombat(copyOfCombatDetails);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //If scene is not running, don't update anything
        if (_state == 0) return;

        //Scene is running, so update _timeRemaining
        _timeRemaining -= Time.deltaTime;

        //Scene has just started, so show the cutscene bars that cover the top and bottom 10% of the scene
        if(_state == 1)
        {
            //Show bars
            MoveBars((StartLength - _timeRemaining) / StartLength);
            //Move onto next state if ready
            if (_timeRemaining <= 0)
            {
                _timeRemaining = LengthInSeconds;
                _state = 2;
            }
        }
        //Scene is playing
        else if(_state == 2)
        {
            //Move onto next state if ready
            if (_timeRemaining <= 0)
            {
                _timeRemaining = EndLength;
                _state = 3;
                //Switch cameras
                var cam = GameObject.FindGameObjectWithTag("MainCamera");
                if (cam != null)
                {
                    var camera = cam.GetComponent<CameraFollow>();
                    if (camera != null)
                    {
                        camera.SetTarget(player.transform);
                        camera.Locked = true;
                        //Start zooming camera to correct spot
                        camera.SetTargetZoom(OldZoom);
                    }
                }
            }
            //If one or more cutscene actions are ready to take place, process them
            if (_listHead >= Actions.Count) return;
            while (Actions[_listHead].StartTime <= (LengthInSeconds - _timeRemaining))
            {
                //Debug.Log("t: " + (LengthInSeconds - (_timeRemaining)) + ", i:" + _listHead + ", c: " + Actions.Count + ", processing action: " + Actions[_listHead].Action);
                ProcessAction(Actions[_listHead]);
                _listHead++;
                if (_listHead >= Actions.Count) return;
            }
        }
        //Scene is ending, hide aforementioned bars
        else if(_state == 3)
        {
            //Hide bars
            MoveBars((_timeRemaining)/EndLength);
            if (_timeRemaining <= 0)
            {
                EndCutscene();
            }
        }
	}

    //Processes actual action - moves NPC, triggers animation, etc
    private void ProcessAction(CutsceneAction action)
    {
        switch(action.Action)
        {
            //Move camera
            //Args: float x_dir, float y_dir, float moveSpeed, float time
            case CutsceneAction.CAction.CAMERA_MOVE:
                {
                    var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
                    camera.Locked = false;
                    camera.Pan((float)action.Arg1, (float)action.Arg2, (float)action.Arg3, (float)action.Arg4);
                    break;
                }
            //Start combat once the scene is over
            //Args: GameObject combatDetails
            case CutsceneAction.CAction.COMBAT_START:
                {
                    CombatDetails = (GameObject)action.Arg1;
                    break;
                }
            //Activate game object
            //Args: GameObject object
            case CutsceneAction.CAction.GAMEOBJECT_ACTIVATE:
                {
                    ((GameObject)action.Arg1).SetActive(true);
                    break;
                }
            //Deactivate game object
            //Args: GameObject object
            case CutsceneAction.CAction.GAMEOBJECT_DEACTIVATE:
                {
                    ((GameObject)action.Arg1).SetActive(false);
                    break;
                }
            //Animate NPC
            //Args: GameObject npc, string animName
            case CutsceneAction.CAction.NPC_ANIMATE:
                {
                    var anim = ((GameObject)action.Arg1).GetComponent<Animator>();
                    anim.Play((string)action.Arg2);
                    break;
                }
            //Make NPC face certain direction
            //Args: GameObject npc, int direction
            case CutsceneAction.CAction.NPC_FACE:
                {
                    var npc = ((GameObject)action.Arg1).GetComponent<NPC_Controller>();
                    npc.Face((int)action.Arg2);
                    break;
                }
            //Move NPC
            //Args: GameObject npc, float x_dir, float y_dir, float moveSpeed, float time
            case CutsceneAction.CAction.NPC_MOVE:
                {
                    var npc = ((GameObject)action.Arg1).GetComponent<NPC_Controller>();
                    npc.Move((float)action.Arg2, (float)action.Arg3, (float)action.Arg5);
                    npc.MoveSpeed = (float)action.Arg4;
                    break;
                }
            //Make NPC speak
            //Args: GameObject npc, string dialog, float time
            case CutsceneAction.CAction.NPC_SPEAK:
                {
                    var npc = ((GameObject)action.Arg1).GetComponent<NPC_Controller>();
                    if (npc != null)
                    {
                        npc.Speak((string)action.Arg2, (float)action.Arg3);
                    }
                    else
                    {
                        var dialog = ((GameObject)action.Arg1).GetComponent<DialogController>();
                        dialog.Speak((string)action.Arg2, (float)action.Arg3);
                    }
                    break;
                }
            //Animate player
            //Args: string animName
            case CutsceneAction.CAction.PLAYER_ANIMATE:
                {
                    var anim = player.GetComponent<Animator>();
                    anim.Play((string)action.Arg1);
                    break;
                }
            //Make player face certain direction
            //Args: int direction
            case CutsceneAction.CAction.PLAYER_FACE:
                {
                    var playerController = player.GetComponent<PlayerController>();
                    playerController.Face((int)action.Arg1);
                    break;
                }
            //Move player
            //Args: float x_dir, float y_dir, float moveSpeed, float time
            case CutsceneAction.CAction.PLAYER_MOVE:
                {
                    var playerController = player.GetComponent<PlayerController>();

                    playerController.MovementVector = new Vector3((float)action.Arg1, (float)action.Arg2, 0);
                    playerController.ScriptedMovementSpeed = (float)action.Arg3;
                    playerController.ScriptedTimeRemaining = (float)action.Arg4;
                    break;
                }
            //Make player speak
            //Args: string dialog, float time
            case CutsceneAction.CAction.PLAYER_SPEAK:
                {
                    var dialogController = player.GetComponent<DialogController>();
                    dialogController.Speak((string)action.Arg1, (float)action.Arg2);
                    break;
                }
            //Complete a quest objective
            case CutsceneAction.CAction.QUEST_COMPLETEOBJECTIVE:
                {
                    NotifyQuestManagerUponCompletion = true;
                    break;
                }
            //Start a new quest
            case CutsceneAction.CAction.QUEST_STARTQUEST:
                {
                    var eventManager = player.GetComponent<EventManager>();
                    eventManager.StartQuest((double)action.Arg1);
                    break;
                }
            //Add an item
            case CutsceneAction.CAction.INVENTORY_ADDITEM:
                {
                    var inventory = player.GetComponent<Inventory>();
                    inventory.AddItemToInventory(((GameObject)action.Arg1).GetComponent<Item>().ID, (int)action.Arg2);
                    break;
                }
            //Remove an item
            case CutsceneAction.CAction.INVENTORY_REMOVEITEM:
                {
                    var inventory = player.GetComponent<Inventory>();
                    inventory.RemoveItemFromInventory(((GameObject)action.Arg1).GetComponent<Item>().ID, (int)action.Arg2);
                    break;
                }
            //Load level when cutscene ends
            case CutsceneAction.CAction.LOADLEVEL:
                {
                    if (((GameObject)action.Arg1) != null) LevelSceneManager = ((GameObject)action.Arg1).GetComponent<SceneManager>();
                    LevelToLoadOnEnd = (string)action.Arg2;
                    LevelSpawnPointID = (int)action.Arg3;
                    break;
                }
            //Changes sprite on objects with "QuestSprite" script attached
            case CutsceneAction.CAction.SPRITE_CHANGE:
                {
                    var sprite = ((GameObject)action.Arg1).GetComponent<QuestSprite>();
                    sprite.ChangeSprite();
                    break;
                }
            //Teleports NPC to specific coordinates
            case CutsceneAction.CAction.NPC_TELEPORT:
                {
                    var transform = ((GameObject)action.Arg1).gameObject.transform;
                    transform.position = new Vector3((float)action.Arg2, (float)action.Arg3, transform.position.z);
                    break;
                }
        }
    }

    private void MoveBars(double percentage)
    {
        foreach (var x in GameObject.FindGameObjectsWithTag("CutsceneOverlay"))
        {
            var bar = x.GetComponent<RectTransform>();
            //Top bar
            if (bar.position.z > 9.0f)
            {
                bar.anchoredPosition = new Vector3(0.0f, (float)(percentage * 80 * -1) + 50, 10.0f);
            }
            //Bottom bar
            else
            {
                bar.anchoredPosition = new Vector3(0.0f, (float)(percentage * 80) - 50, 0.0f);
            }
        }
    }
}
