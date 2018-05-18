using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour {

    //Movement          
    public float MoveSpeed = 8.0f;      //Movement speed
    public Vector3 MovementVector;      //Movement direction vector
    public float ScriptedMovementSpeed = 0.0f;  //Movement speed for current cutscene
    public float ScriptedTimeRemaining = 0.0f;  //Time remaining for scripted movement
    public int NumberOfSpritesBehind = 0;   //Used by SortingLayerControl
    private int _targetSpawnID = -1;     //Spawn point to move player to

    //Animation
    public int Direction = 0;           //What direction the player is facing
    public string DefaultSortingLayer;  //Sorting layer and order, used by SortingLayerControl
    public int DefaultSortingOrder;
    private Animator _animator;
    private int _targetZoom = 5;         //Camera zoom settings
    private int _minimumZoom = 2;
    private int _maximumZoom = 10;

    //Stats
    public PlayerStats Stats;           //Stores the base stats of players before calculations
    public string Name;                 //Player's name
    public int BaseHealth = 100;        //Player's base health
    public int Health = 100;            //Player's current health
    public int MaxHealth = 100;         //Maximum HP (if player is overhealed)
    public int PhysicalAttack = 1;      //Player's current physical attack stat
    public int MagicalAttack = 1;       //Magical attack
    public int PhysicalDefense = 1;     //Physical defense
    public int MagicalDefense = 1;      //Magical defense
    public float Speed = 1F;            //Speed, influences cooldowns

    //Flags
    public bool IsTest = false;         //Used by some tests
    //public bool collected = false;   //invariant for testing pickups
    public bool InCombat = false;       //If player is in combat, certain functions are restricted
    public bool CPU = false;            //Flag if player is computer controlled
    public bool IsDead = false;         //Flag to determine if player is "dead"
    public bool FailedCombat = false;   //If player ran away
    public bool InCutscene = false;     //If player is in a cutscene, certain functions are restricted

    //Player locations
    private Vector3 _entrancePosition;  //Position of entrance (player spawns here if they die)
    private Vector3 _combatPosition;    //Position from which combat was entered

    //combat
    public GameObject combatManager;    //holds info about the combat instance to create if first player to join
    public GameObject combatDetails;    //stores information needed to start combat instance
    public GameObject combatUI; //used to display healthbars cooldowns etc in combat
    public int friendlyTarget = 0, enemyTarget = 0; //player's target
    public BuffIcon BuffIcon;
    public bool fromCombat = false;
    public float xpModifier = 1f;   //modifier for experience gain
    public PlayerAI AI;

    //equipped lists
    public List<AttackDetails> attacks;     //stores player's equipped attacks
    public List<AttackDetails> passives;    //stores player's equipped passive abilities
    public List<Item> equippedGear;         //stores player's equipped gear
    public List<GameObject> party;          //list of players to help player in battle

    //Chat system
    private string textMessage = null;              //Text message to be sent
    private int textMessageSendCount = 0;           //Number of times current message has been sent
    private static int timesToSendTextMessage = 3;  //Send text message 3 times, in case of packet loss
    private object textLock = new object();         //Lock object used for the above
    public bool IsTypingChatMessage = false;

    //Etc, mostly cached Components
    public ObjectSpawner objectSpawner;
    public SceneManagerInstance nextLevel;  //loads info about the scene to load
    public Canvas PlayerInventory; //houses all of the games main menus
    private NetworkManager networkManager;
    private BonusManager bonusManager;

    

    /********************************************************************************
    ------------------------------- UNITY SECTION -----------------------------------
    *********************************************************************************
    -This section contains all functions used by the Unity Engine
    ********************************************************************************/


    //Use this for initialization
    void Start()
    {
        //Make player persist
        DontDestroyOnLoad(this);

        //Fetch animator and database manager
        _animator = GetComponent<Animator>();

        networkManager = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        //Initialize some stuff
        Stats = GetComponent<PlayerStats>();
        LoadLevel();
        //Get sorting layer and order
        DefaultSortingLayer = GetComponent<SpriteRenderer>().sortingLayerName;
        DefaultSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        //Initialize inventory
        for (int i = 0; i < 12; i++)
        {
            equippedGear.Add(null);
        }
        //If CPU then start CPU script and return (CPUs don't need to do anything else)
        if (CPU)
        {
            Health = BaseHealth;
            StartAI();
            return;
        }
        //Instantiate inventory (if not in combat)
        if(!InCombat)
        {
            Instantiate(PlayerInventory);
        }
        //Attach camera, only if CameraFollow script is attached to it
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null)
        {
            if (camera.GetComponent<CameraFollow>() != null)
            {
                camera.GetComponent<CameraFollow>().SetTarget(transform);
            }
        }

        //Load equipment and player stats from database (make sure to load stats first)
        networkManager.RetrievePlayerStats(gameObject);
        networkManager.RetrievePlayerEquipment(this);
        
        //Scale player's health AFTER the stats and equipment have loaded
        Health = BaseHealth;

        //Activate any bonuses
        bonusManager = new BonusManager(Stats, GetComponent<Inventory>(), networkManager);
        bonusManager.ProcessBonusCode(networkManager.EnteredBonusCode);

        //Update player label
        UpdatePlayerLabel();
    }

    //Movement is done on every physics step (otherwise the player and camera spasm)
    void FixedUpdate()
    {
        //Movement code. Movement is handled two different ways depending on if it's scripted or not
        //If inside a cutscene (or anywhere that movement is scripted)
        if (InCutscene)
        {
            if (ScriptedTimeRemaining > 0)
            {
                float t = Time.deltaTime;
                if (t > ScriptedTimeRemaining) t = ScriptedTimeRemaining;
                Move(MovementVector.x, MovementVector.y, ScriptedMovementSpeed, t);
                ScriptedTimeRemaining -= t;
            }
            else
            {
                MovementVector = new Vector3(0, 0, 0);
            }
        }
        //Else move according to inputs
        else if (!InCombat && !CPU)
        {
            //Do not move if currently typing a message
            if (IsTypingChatMessage)
            {
                Move(0, 0, MoveSpeed, Time.deltaTime);
            }
            else
            {
                Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), MoveSpeed, Time.deltaTime);
            }
            //Sync this across the network
            networkManager.UdpUpdateLevelName(Application.loadedLevelName);
            networkManager.UdpUpdatePosition(transform.position.x, transform.position.y);
        }
    }

    //Everything else (combat, UI, animations, camera controls, etc) is done less often, to help the framerate
    void Update()
    {
        //Log mouse clicks to database
        if (!CPU && Input.GetMouseButtonDown(0))
        {
            networkManager.DBClick();
            GetComponent<EventManager>().Event(new List<object>() { GameAction.MOUSE_CLICK });
        }

        //Combat
        if(InCombat)
        {
            //Buff icons
            BuffIcon.Set(Stats);
        }
        if (InCombat && !CPU)
        {
            ParseCombatInput();
        }

        //Update animation
        UpdateAnimation();

        //If not already done, link PlayerController to UdpManager within NetworkManager
        networkManager.UdpLinkPlayerController(this);
    }

    //Zoom camera
    void OnGUI()
    {
        if(!CPU && !InCutscene)
        {
            if(Event.current.type == EventType.ScrollWheel)
            {
                ZoomCamera(-Event.current.delta.y / 20);
            }
        }
    }

    //Updates animation
    void UpdateAnimation() {
        //Don't update for CPUs
        if (CPU) return;
        //Get direction
        float x = MovementVector.x;
        float y = MovementVector.y;
        if (x < 0 && Math.Abs(x) >= Math.Abs(y))        //NW, W or SW
        {
            Direction = 1;  //West
        }
        else if (x > 0 && Math.Abs(x) >= Math.Abs(y))   //NE, E or SE
        {
            Direction = 3;  //East
        }
        else if (y > 0 && Math.Abs(y) >= Math.Abs(x))   //N
        {
            Direction = 2;  //North
        }
        else if (y < 0)                                 //S
        {
            Direction = 0;  //South
        }
        //Update animation
        _animator.SetInteger("Direction", Direction);
        _animator.SetBool("Moving", (Math.Abs(x) + Math.Abs(y) > 0));
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is a pickup.
        if (other.tag == "Pickup")
        {
            DontDestroyOnLoad(other.gameObject);
            //If item is successfully added to the inventory
            if (GetComponent<Inventory>().AddItemToInventory(other.gameObject.GetComponent<Item>()))
            {
                //Disable the object the player collided with.
                RemoveFromSceneManager(other.gameObject);
                Destroy(other);
                other.gameObject.SetActive(false);
                //Notify the event system
                Item item = other.gameObject.GetComponent<Item>();
                GetComponentInParent<EventManager>().Event(new List<object>() { GameAction.PICKED_UP, item.ID, 1 });
                //Notify the database as well 
                networkManager.DBPickupItems(1); 
            }
        }
        //Transport player to another scene
        else if (other.tag == "Portal")
        {
            var info = other.GetComponent<InstanceSwitch>();
            TriggerSceneChange(info.sceneManager, info.instanceName, info.SpawnPointID);
        }
        else if (other.tag == "CombatPortal")
        {
            TriggerCombat(other.gameObject);
        }
        //Show speech icon
        else if (other.tag == "NPC" && !InCutscene)
        {
            foreach (Image i in GetComponentsInChildren<Image>())
            {
                if (i.name == "Speech Icon")
                {
                    i.enabled = true; break;
                }
            }
        }
        //Trigger a cutscene
        else if (other.tag == "CutscenePortal" || (other.tag == "SpawnPoint" && other.GetComponent<Cutscene>() != null))
        {
            //If there is a "Start Quest on Collision" attached, process it first
            if (other.GetComponent<StartQuestOnCollision>() != null)
            {
                GetComponent<EventManager>().StartQuest(other.GetComponent<StartQuestOnCollision>().QuestID);
                //Debug.Log("Starting quest " + other.GetComponent<StartQuestOnCollision>().QuestID);
            }
            //Check if we should activate the cutscene. If so, start it
            var questManager = GetComponent<QuestManager>();
            var cutscene = other.gameObject.GetComponent<Cutscene>();
            //Debug.Log("Cutscene?");
            bool canStart = questManager.ObjectiveCompleted(cutscene.RequiredQuestID, cutscene.RequiredObjectiveID);
            bool doNotRun = questManager.ObjectiveCompleted(cutscene.DontRunAfterQuestID, cutscene.DontRunAfterObjectiveID);
            //Debug.Log("Can start? " + canStart + " Do not run? " + doNotRun);
            if (canStart == true && doNotRun == false)
            {
                //Debug.Log("Yes!");
                cutscene.StartCutscene();
            }
            //else Debug.Log("No!");
        }
        //Unlock a locked object
        else if (other.tag == "Lock")
        {
            var key = other.GetComponent<LockedItem>().Key;
            var inventory = GetComponent<Inventory>();
            //Check if user has required key
            if (inventory.ItemCount(key.GetComponent<Item>().ID) > 0)
            {
                //Remove the key from the user's inventory
                inventory.RemoveItemFromInventory(key.GetComponent<Item>().ID, 1);
                //Unlock the lock
                other.GetComponent<LockedItem>().Unlock();
                //Update the event manager
                GetComponent<EventManager>().Event(new List<object>() { GameAction.UNLOCKED, other.GetComponent<LockedItem>().Name });
            }
        }
        else if (other.tag == "Shortcut")
        {
            //Debug.Log("Shortcut Found!");
            networkManager.DBShortcutUsed(other.name);
            GetComponent<EventManager>().Event(new List<object>() { GameAction.SECRET_AREA });
        }
        else if(other.tag == "BulletinBoard")
        {
            //SHow read icon
            foreach (Image i in GetComponentsInChildren<Image>())
            {
                if (i.name == "Read Icon")
                {
                    i.enabled = true; break;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Talk to NPC
        if (other.tag == "NPC")
        {
            if (Input.GetKey(KeyCode.E))
            {
                foreach (Image i in GetComponentsInChildren<Image>())
                {
                    if (i.name == "Speech Icon")
                    {
                        i.enabled = false; break;
                    }
                }
                GetComponent<EventManager>().Event(new List<object>() { GameAction.TALK_TO, other.gameObject.GetComponent<NPC_Controller>().Name });
                other.gameObject.GetComponent<NPC_Controller>().Speak(GetComponent<Inventory>());
            }
        }
        //Show billboard
        else if (other.tag == "BulletinBoard")
        {
            if (Input.GetKey(KeyCode.E))
            {
                foreach (Image i in GetComponentsInChildren<Image>())
                {
                    if (i.name == "Read Icon")
                    {
                        i.enabled = false; break;
                    }
                }
                GameObject.Find("Bulletin Board UI").GetComponentsInChildren<Canvas>(true)[0].gameObject.SetActive(true);
                networkManager.DBBulletinBoardAccessed();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Hide speech icon
        if (other.tag == "NPC")
        {
            foreach (Image i in GetComponentsInChildren<Image>())
            {
                if (i.name == "Speech Icon")
                {
                    i.enabled = false; break;
                }
            }
        }
        //Hide read icon
        else if (other.tag == "Bulletin Board")
        {
            foreach (Image i in GetComponentsInChildren<Image>())
            {
                if (i.name == "Read Icon")
                {
                    i.enabled = false; break;
                }
            }
        }
    }

    public IEnumerator LoadLevel()
    {

        AsyncOperation async = Application.LoadLevelAsync("Forest");
        yield return async;
        Debug.Log("Level " + "Forest" + " Loaded Successfully");
    }

    void OnLevelWasLoaded(int level)
    {
        //Let network manager know the level has changed
        networkManager.UdpUpdateLevelName(Application.loadedLevelName);
        //add origin and destination variable to determine side of level to move to
        //can input switch to determine spawn from level
        if (Application.loadedLevelName == "Combat")
        {
            combatDetails.SetActive(false);
            combatManager = GameObject.FindGameObjectWithTag("CombatManager");

            //if entering combat, we want to join the combat
            if (combatManager.GetComponent<CombatManager>().JoinCombat(gameObject))
            {
                InCombat = true;
            }
            else//if joining combat fails reload old level
            {
                ReloadLevel();
            }
        }
        else
        {
            //Move player to appropriate spawn point if no target spawn id is set (< 0)
            if (_targetSpawnID >= 0)
            {
                Vector3 position = Vector3.zero;
                foreach (var point in GameObject.FindGameObjectsWithTag("SpawnPoint"))
                {
                    if (point.GetComponent<SpawnPoint>().ID == _targetSpawnID)
                    {
                        position = point.transform.position;
                        break;
                    }
                }
                if (position != Vector3.zero)
                {
                    transform.position = position;
                    _entrancePosition = position;
                    //Notify database of current location
                    networkManager.UpdatePlayerStat("CurrentPositionX", position.x.ToString());
                    networkManager.UpdatePlayerStat("CurrentPositionY", position.y.ToString());
                }
            }
            //If loading scene for first time (like on login), set entrance position to current position
            if(_entrancePosition == Vector3.zero)
            {
                _entrancePosition = transform.position;
            }
            if (GameObject.FindGameObjectWithTag("Spawner") && !fromCombat)
            {
                objectSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<ObjectSpawner>();
                objectSpawner.SpawnRandomObjectsFromSpawnPoints(nextLevel.objects, nextLevel.spawnChances, nextLevel);

            }
            else if (GameObject.FindGameObjectWithTag("Spawner") && fromCombat)
            {
                //If we failed combat, move back to the start of the level
                if (FailedCombat)
                {
                    //Respawn character at location entrance
                    transform.localPosition = _entrancePosition;
                }
                else
                {
                    //Place player back where they were
                    transform.localPosition = _combatPosition;
                }
                //Reset health
                PostBattleRecovery();
                objectSpawner.ShowObjects();
            }
            //Update camera zoom
            var camera = GameObject.FindGameObjectWithTag("MainCamera");
            if (camera.GetComponent<CameraFollow>() != null) camera.GetComponent<Camera>().orthographicSize = _targetZoom;
            fromCombat = false;
        }
        //Update the player's label
        UpdatePlayerLabel();
    }


    /********************************************************************************
    ------------------------------- CONTROL SECTION ---------------------------------
    *********************************************************************************
    -This section contains all functions related to movement, camera controls, input
     parsing, etc.
    ********************************************************************************/

    // Takes in x and y components for direction vector and calculates and moves the player
    public void Move(float x, float y, float movespeed, float time)
    {
        MovementVector = new Vector3(x, y, 0);
        MovementVector.Normalize();
        transform.position += MovementVector * movespeed * time;
    }

    //Faces player in specific direction (animation will update on next Update)
    public void Face(int direction)
    {
        Direction = direction;
        MovementVector = new Vector3(0, 0, 0);
    }

    //Zoom camera
    private void ZoomCamera(float delta)
    {
        if (delta != 0.0f)
        {
            _targetZoom += (delta < 0.0f ? 1 : -1);
            //Enforce minimum and maximum zoom levels
            if (_targetZoom < _minimumZoom) _targetZoom = _minimumZoom;
            if (_targetZoom > _maximumZoom) _targetZoom = _maximumZoom;
        }
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null)
        {
            if (camera.GetComponent<CameraFollow>() == null) return;
            var c = camera.GetComponent<Camera>();
            c.orthographicSize = _targetZoom;
        }
    }

    //Handles all combat-related player input
    void ParseCombatInput()
    { 
        //Targeting
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                //Debug.Log("targeting");
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    var enemy = hit.transform.gameObject.GetComponent<EnemyController>();
                    enemyTarget = enemy.Stats.CombatPosition;
                    StartCoroutine(enemy.CombatUI.GetComponent<CombatUI>().ShowTarget(2));

                }
                else if (hit.transform.gameObject.tag.Contains("Player"))
                {
                    var friendly = hit.transform.gameObject.GetComponent<PlayerController>();
                    friendlyTarget = friendly.Stats.CombatPosition;
                    StartCoroutine(friendly.combatUI.GetComponent<CombatUI>().ShowTarget(2));
                }
                //Debug.Log("Friendly: " + friendlyTarget + ", enemy: " + enemyTarget);
            }
        }
        //See if an attack was selected
        int attackSelected = -1;    //Anything below 0 is "not valid"
        for (int i = 0; i < attacks.Capacity; i++)
        {
            if (Input.GetButtonDown("Attack" + (i + 1)) && attacks.Count > i)
            { 
                attackSelected = i;
                break;
            }
        }
        //If attack is not on cooldown, use whatever attack was selected
        if (Stats.Cooldown <= 0 && attackSelected >= 0)
        {
            DoAttack(attackSelected);
        }
        else
        {
            //If any attacks were selected, show the "Not Ready" message
            if (attackSelected >= 0)
            {
                float x = Camera.main.pixelWidth * .5f;
                float y = Camera.main.pixelHeight * 5f;
                Vector3 pos = new Vector3(x, y, 0);
                combatManager.GetComponent<CombatManager>().ShowPopup("Not Ready!", 1.5f, Color.red, 20, Camera.main.ScreenToWorldPoint(pos), true);
            }
        }
    }

    public void TriggerSceneChange(SceneManager sceneManager, string levelName, int spawnPointID)
    {
        if (objectSpawner)
        {
            objectSpawner.CleanObjects();
            Destroy(objectSpawner);
        }
        //Load the scene manager if one is defined
        if (sceneManager != null)
        {
            nextLevel = new SceneManagerInstance(sceneManager);
        }
        //Fix sorting layer and order
        GetComponent<SpriteRenderer>().sortingLayerName = DefaultSortingLayer;
        GetComponent<SpriteRenderer>().sortingOrder = DefaultSortingOrder;
        NumberOfSpritesBehind = 0;
        //Get spawning position
        string newLevel = levelName;
        _targetSpawnID = spawnPointID;
        //Load level
        Application.LoadLevel(newLevel);
        //Notify event system that location has changed
        GetComponent<EventManager>().Event(new List<object>() { GameAction.IN_AREA, newLevel });
        //Save all this to the database
        networkManager.DBSwitchInstance();
        networkManager.UpdatePlayerStat("CurrentScene", levelName);
        networkManager.UpdatePlayerStat("CurrentSceneSpawn", spawnPointID.ToString());
        if (sceneManager != null && sceneManager.name != null)
        {
            networkManager.UpdatePlayerStat("CurrentSceneManager", sceneManager.name);
        }
        else
        {
            networkManager.UpdatePlayerStat("CurrentSceneManager", "");
        }
    }


    /********************************************************************************
    ------------------------------- COMBAT SECTION ---------------------------------
    *********************************************************************************
    -This section contains all functions related to player and CPU combat
    ********************************************************************************/

    public void TriggerCombat(GameObject trigger)
    {
        //Fix sorting order
        NumberOfSpritesBehind = 0;
        GetComponent<SpriteRenderer>().sortingLayerName = DefaultSortingLayer;
        GetComponent<SpriteRenderer>().sortingOrder = DefaultSortingOrder;
        DontDestroyOnLoad(objectSpawner);
        objectSpawner.HideObjects();
        //move player to combat instance
        //save current position
        _combatPosition = transform.position;
        if (IsTest)
        {
            nextLevel = new SceneManagerInstance(GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>());
        }
        else
        {
            nextLevel.name = Application.loadedLevelName;
        }
        //save portal as combat details for loading combat
        combatDetails = trigger;
        //removeFromSceneManager(other.gameObject);
        //protect components
        DoNotDestroy();
        //Heal
        PostBattleRecovery();
        //load fight
        RemoveFromSceneManager(trigger);
        Application.LoadLevel("Combat");
    }
    
    public void RunAway()
    {
        combatManager.GetComponent<CombatManager>().RunAway = true;
    }

    public void ReloadLevel()
    {
        DoNotDestroy();
        Application.LoadLevel(nextLevel.name);
    }

    //protects components from being destroyed on scene swap,
    public void DoNotDestroy()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(combatDetails);
        DontDestroyOnLoad(gameObject.GetComponent<Inventory>());
        DontDestroyOnLoad(Stats);
    }

    //removes collected/killed objects from the scene
    public void RemoveFromSceneManager(GameObject other)
    {
        if (!IsTest)
        {
            int index = nextLevel.objectLocations.IndexOf(other.transform.position);
            if (index >= 0)
            {
                nextLevel.RemoveObject(index);
            }
        }
    }

    //logs and adds xp. xpAdded is used to save the total amount of XP added by the end, returns true if player levels up
    public bool AddXP(int xpAdded)
    {
        //How much XP is ultimately to be added after scaling, etc
        int addedXp = 0;
        //First calculate how much XP will actually be added after scaling
        //If at levelcap slow xp gain
        if (Stats.PlayerLevel == LimitController.levelCap)
        {
            int scaledXPAdded = (int)(xpAdded * xpModifier * .2);
            //Don't give xp if they have enough to get the level above cap 
            if (scaledXPAdded + Stats.XP > LimitController.LevelReqs[Stats.PlayerLevel + 1])
                addedXp = LimitController.LevelReqs[Stats.PlayerLevel + 1] - Stats.XP;
            else
            {
                addedXp = scaledXPAdded;
            }

        }
        //Not at level cap, so gain XP normally
        else
        {
            addedXp = (int)(xpAdded * xpModifier);
        }
        //Update this stat
        Stats.AddXP(addedXp);

        //Quality up if enough XP has been earned
        if(Stats.PlayerLevel != LimitController.levelCap && Stats.XP >= LimitController.LevelReqs[Stats.PlayerLevel + 1])
        {
            Stats.LevelUp();
            return true;
        }
        return false;
    }

    //If player failed combat, reset their flags and recover their HP
    public void PostBattleRecovery()
    {
        Stats.Reset();
        IsDead = false;
        FailedCombat = false;
    }



    //Execute attack and determine cooldowns (rename?)
    public void DoAttack(int attack)
    {
        //if empty attack do nothing
        if (attacks[attack].AttackName == "Empty")
        {
            return;
        }
        DoAttack(attacks[attack]);
    }

    public void DoAttack(AttackDetails attack)
    {
        var combat = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
        Stats.Cooldown = combat.ExecuteAttack(gameObject, attack, combat.Players, combat.Enemies, friendlyTarget, enemyTarget);
    }

    //Equip from database
    public void EquipFromDatabase(Item gear)
    {
        equippedGear[(int)gear.GearType] = gear;
        Stats.ReloadEnchantments();
    }

    //Equip gear piece (will swap out existing equipment if there is any)
    public void Equip(Item gear)
    {
        //Remove this gear from inventory
        GetComponent<Inventory>().RemoveItemFromInventory(gear.ID, 1);
        //Swap this with whatever is currently in that slot
        Item piece = equippedGear[(int)gear.GearType];
        equippedGear[(int)gear.GearType] = gear;
        //Update slot number
        gear.Slot = (int)gear.GearType;
        //If the slot was occupied, add the item back to the inventory
        if (piece != null)
        {
            GetComponent<Inventory>().AddItemToInventory(piece);
        }
        //This will add a new entry (if nothing is equipped) or overwrite an existing one
        networkManager.AddEquipment(gear);
        //Reload enchantments
        Stats.ReloadEnchantments();
    }

    //Unequip gear piece
    public void Unequip(Item gear)
    {
        //Add to inventory
        GetComponent<Inventory>().AddItemToInventory(gear);
        //Clear equipment slot
        equippedGear[(int)gear.GearType] = null;
        //Save change
        networkManager.RemoveEquipment((int)gear.GearType);
    }

    //Use a consumable item (implement more effects)
    public void Consume(Item item)
    {
        /*
        if(item.Heal > 0) Stats.Heal(item.Heal);
        if (item.Cooldown == true)
        {
            Stats.Cooldown = 0;
            Stats.ResetCooldown = true;
        }
        */
    }

    // add ability to load attacks from database (now its just hardcoded into player prefab)
    public void LoadAttacks(List<GameObject> actives)
    {
        attacks = new List<AttackDetails>(actives.Count);
        for (int i = 0; i < actives.Count; i++)
        {
            if (actives[i].GetComponent<HotBarSlot>().skill != null)
            {
                attacks.Add(new AttackDetails(actives[i].GetComponent<HotBarSlot>().skill.GetComponent<AttackDetailsInstance>()));
            }
        }
    }

    public void LoadPassives(List<GameObject> _passives)
    {
        passives = new List<AttackDetails>(_passives.Count);
        Stats.Passives = Stats.Passives.ToDictionary(p => p.Key, p => 0.0f);
        for (int i = 0; i < _passives.Count; i++)
        {
            if (_passives[i].GetComponent<HotBarSlot>().skill != null)
            {
                var passive = new AttackDetails(_passives[i].GetComponent<HotBarSlot>().skill.GetComponent<AttackDetailsInstance>());
                Stats.Passives[passive.BuffType] = passive.BuffPotency;
            }
        }
    }
    
    //start attached ai
    public void StartAI()
    {
        AI.StartAI();
    }

    /*public void EquipPassive(AttackDetails passive)
    {
        passives.Add(passive);
    }

    public void RemovePassive(int slot)
    {
        if (slot > passives.Count)
        {
            passives.RemoveAt(slot);
        }
    }*/

    //Get player's text in a thread-safe way
    public string GetTextMessage()
    {
        lock(textLock)
        {
            if(textMessageSendCount > 0)
            {
                textMessageSendCount--;
            }
            if(textMessageSendCount == 0)
            {
                textMessage = null;
            }
            return textMessage;
        }
    }

    //Set player's text in a thread-safe way
    public void SetTextMessage(string message)
    {
        lock(textLock)
        {
            textMessage = message;
            textMessageSendCount = timesToSendTextMessage;
        }
    }

    //Set player's username
    public void SetUsername(string newName)
    {
        //Update name
        Name = newName;
        UpdatePlayerLabel();
    }

    //Update player's label (call this if the username or level change)
    public void UpdatePlayerLabel()
    {
        foreach (Text t in GetComponentsInChildren<Text>(true))
        {
            if (t.name == "Player Label")
            {
                //Hide label if in combat or cutscene
                if (InCombat || InCutscene)
                {
                    t.gameObject.SetActive(false);
                }
                //Else show it and update it
                else
                {
                    t.gameObject.SetActive(true);
                    t.text = Name + " (" + Stats.PlayerLevel + ")";
                }
            }
        }
    }
}
