using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour {
    //Storage
    public List<GameObject> Players;    //All players in combat
    public List<GameObject> Enemies;    //All enemies in combat
    private List<GameObject> _deadPlayers;  //Where dead players go
    private GameObject CombatDetails;   //All init details for instance
    private EventManager _eventManager; //Event manager
    //public Sprite background;   //stores the background sprite
    //public List<Button> skillButtons;   //stores the buttons for the skill system (for setting icons) 

    //Combat variables
    public bool RunAway = false;        //If players wants to run away
    private int MaxPlayers = 5;         //Max player party size
    private float _baseTimer = 5f;      //Pre combat countdown length
    private float _startTimer = 5f;     //Actual counter for the countdown
    private float _failedTime = 3f;     //Time to wait before exiting battle after failing
    private float _runAwayTime = 2f;    //Time to wait before exiting battle after running away
    private bool _isEvent = false;      //If this is an event (this will be fleshed out later on)
    //public int maxEnemies = 3;  //Max number of enemies

    //Position variables (not working right?)
    private float _playerX = .1f;       //X position of players
    private float _enemyX = .8f;        //X position of enemies

    //Miscellaneous
    public new Camera camera;           //Easier access to Camera.Main
    private int _postBattle = 0;        //Post battle flag, controls event sequence
    private bool _started = false;      //If battle has started
    private bool _firstStart = true;    //If battle has just started
    private bool _anyoneJoined = false; //Don't do anything until at least one player has joined
    private int _numberDead = 0;        //Keeps track of number of players who have died
    public Canvas canvas;           //Canvas for popups, doesn't do anything?
    private NetworkManager _networkManager; //Network manager
    private int _countdownVisual = 0;   //Int to display remaining seconds in countdown
    private Vector3 _playerScale;       //Original player scale
    

    public void Start () {
        //Set start timer
        _startTimer = _baseTimer;
        //Get camera
        camera = Camera.main;
        //Get network manager
        _networkManager = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        //Etc
        _postBattle = 0;
        _deadPlayers = new List<GameObject>();
    }
    
	public void Update () {

        //Do nothing if no one has joined (not sure if this is necessary)
        if (!_anyoneJoined) return;

        //If combat has started
        if (_started)
        {
            //If combat has just started
            if (_firstStart)
            {
                //Tell player that combat has started
                ShowPopupInMiddleOfScreen("Combat Start!", 2f, Color.white, 34, true, -.001f, .002f);
                _firstStart = false;
            }
            //Battle won!
            if (Enemies.Count == 0)
            {
                //First part of end of battle - distribute rewards
                if(_postBattle == 0)
                {
                    //Tell player that combat has finished
                    ShowPopupInMiddleOfScreen("Combat Cleared!", 2f, Color.white, 20, true, -.001f, .002f);
                    //Notify database that battle has been won
                    _networkManager.DBWinBattle();
                    //Give rewards
                    GiveRewards();
                    //Wait until rewards have all been distributed
                    _postBattle = 1;
                }
                //Save stuff, clean up, load last level
                if(_postBattle==2)
                {
                    EndCombat(true);
                }
            }
            //Battle lost :(
            else if(_numberDead == Players.Count)
            {
                //Notify player that battle has ended
                if(_postBattle == 0)
                {
                    //Notify player
                    ShowPopupInMiddleOfScreen("Combat Failed!", _failedTime, Color.red, 20, true, -.001f, .002f);
                    //Save the loss
                    _networkManager.DBLoseBattle();
                    //Move on in a few seconds
                    _postBattle = 1;
                    StartCoroutine(WaitForSeconds(_failedTime));
                }
                //Wait a couple seconds, then clean up and reset everything
                if(_postBattle == 2)
                {
                    EndCombat(false);
                }
            }
            //Player ran away
            else if (RunAway == true)
            {
                //First part, player has run away
                if (_postBattle == 0)
                {
                    //Notify player
                    ShowPopupInMiddleOfScreen("Ran Away Safely!", _runAwayTime, Color.white, 20, true, -.001f, .002f);
                    //Save this to server
                    _networkManager.DBRunAway();
                    //Move on in a few seconds
                    _postBattle = 1;
                    StartCoroutine(WaitForSeconds(_runAwayTime));
                }
                //End combat
                if (_postBattle == 2)
                {
                    EndCombat(false);
                }
            }
        }
        //Combat has not started
        else
        {
            _startTimer -= Time.deltaTime;
            //Show new countdown number every second
            if(_countdownVisual != Mathf.CeilToInt(_startTimer) && Mathf.CeilToInt(_startTimer) != 0)
            {
                _countdownVisual = Mathf.CeilToInt(_startTimer);
                ShowPopupInMiddleOfScreen(_countdownVisual.ToString(), 1f, Color.red, 50, true, -.001f, .002f);
            }
            //Start battle when timer hits 0
            if (_startTimer < 0)
            {
                _started = true;
            }
        }
	}

    //Adds player to current combat instance. Countdown starts when at least one has joined
    //Returns if joining combat was successful or not
    public bool JoinCombat(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.ReloadEnchantments();
        stats.LoadPassiveEffects();

        //If this is the first player to join (this should always be the human player), set some flags
        if (Players.Count == 0)
        {            
            //Right now, this starts the countdown
            _anyoneJoined = true;

            //Load in combat details
            CombatDetails = controller.combatDetails;
            //Hide the portal that took us here
            CombatDetails.SetActive(false);
            //Set friendly target to yourself
            controller.friendlyTarget = 0;
            //Shrink sprite size
            _playerScale = player.transform.localScale;
            player.transform.localScale = new Vector3(2f, 2f, 2f);
            //Save reference to event manager
            _eventManager = player.GetComponent<EventManager>();
            //Load level
            LoadLevel();

            //Disable player camera and update the player's animation
            controller.Face(3);
        }

        //If party is full, reject them
        if (Players.Count > MaxPlayers)
        {
            Debug.Log("Party Full");
            //display error message to player
            return false;
        }
        //Else add them to the party

        //Set targets
        int index = Players.Count;
        controller.friendlyTarget = index;
        
        //Add player to player list
        Players.Add(player);

        //Set screen and combat positions
        stats.CombatPosition = index;
        float x = _playerX * camera.pixelWidth;
        float y = GetSpot(Players) * camera.pixelHeight;
        Vector3 pos = new Vector3(x, y, 0);
        player.transform.position = Camera.main.ScreenToWorldPoint(pos);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        
        //First player (again, should be human player)
        if(index == 0)
        {
            //Load CPUs in this player's party
            if (Players.Count < MaxPlayers)
            {
                foreach (GameObject go in controller.party)
                {
                    JoinCombat(Instantiate(go));
                }
            }
        }

        //Set up healthbars and stuff
        GameObject combatUI = Instantiate((Resources.Load("CombatUI") as GameObject));
        combatUI.GetComponent<CombatUI>().Set(stats, true, controller.Name);
        controller.combatUI = combatUI;
        controller.BuffIcon = Instantiate((Resources.Load("BuffIcon") as GameObject)).GetComponent<BuffIcon>();
        stats.Cooldown = 0;

        //If first player, move combat UI up a bit
        if(index == 0)
        {
            controller.transform.position += new Vector3(0, 0.1f, 0);
            combatUI.transform.position += new Vector3(0, 0.55f, 0);
        }

        //Player successfully added to combat
        return true;
    }

    //Gets height for characters
    private float GetSpot(IList<GameObject> list)
    {
        switch (list.Count)
        {
            case 1:
                return .833F;
            case 2:
                return .667F;
            case 3:
                return .5F;
            case 4:
                return .333F;
            case 5:
                return .167F;
        }
        return 0;
    }

    //Loads level, including any enemies
    private void LoadLevel()
    {
        CombatDetails cd = CombatDetails.GetComponent<CombatDetails>();

        //Load background
        GetComponent<SpriteRenderer>().sprite = cd.background;
        _isEvent = cd.isEvent;
        
        //Load enemies
        foreach (GameObject e in cd.enemies)
        {
            //Instantiate enemy
            GameObject enemy = Instantiate(e);
            EnemyController controller = enemy.GetComponent<EnemyController>();
            PlayerStats stats = enemy.GetComponent<PlayerStats>();
            stats.ReloadEnchantments();
            stats.LoadPassiveEffects();

            int index = Enemies.Count;

            //Add enemy to combat manager
            Enemies.Add(enemy);

            //Set enemy position
            float x = _enemyX * camera.pixelWidth;
            float y = GetSpot(Enemies) * camera.pixelHeight;
            Vector3 pos = new Vector3(x, y, 0);
            enemy.transform.position = camera.ScreenToWorldPoint(pos);
            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, 0);
            enemy.SetActive(true);
            stats.CombatPosition = index;

            //Set up healthbars and stuff
            GameObject combatUI = Instantiate((Resources.Load("CombatUI") as GameObject));
            combatUI.GetComponent<CombatUI>().Set(stats, false, controller.Name);
            controller.CombatUI = combatUI;
            controller.BuffIcon = Instantiate((Resources.Load("BuffIcon") as GameObject)).GetComponent<BuffIcon>();
            enemy.GetComponent<PlayerStats>().Cooldown = 0;
        }  
    }

    /// <summary>
    /// Executes an attack, returns the attack's cooldown.
    /// </summary>
    /// <param name="attacker">The game object initiating the attack. Usually "this" or "gameObject".</param>
    /// <param name="attack">Attack to be executed.</param>
    /// <param name="allies">List of the attacker's allies.</param>
    /// <param name="enemies">List of the attacker's enemies</param>
    /// <param name="allyTarget">ID for allied target.</param>
    /// <param name="enemyTarget">ID for enemy target.</param>
    public float ExecuteAttack(GameObject attacker, AttackDetails attack, List<GameObject> allies, List<GameObject> enemies, int allyTarget, int enemyTarget) { 
        
        var attackerStats = attacker.GetComponent<PlayerStats>();

        bool isMainPlayer = attacker.tag == "Player";

        //If battle hasn't started, player is dead, unable to attack, etc, return
        if (attackerStats.UnableToMove || !_started || attackerStats.IsDead || _postBattle != 0)
        {
            return 0;
        }
        //Do not attack if all enemies are dead
        if (Enemies.Count == 0 || Players.Count == 0)
            return 0;

        //All attacks follow the same logic:
        // - Get targets
        // - Calculate damage (if attack is a type that does damage)
        // - Apply buffs
        // - Apply effects
        // - Inflict damage
        //Quirks:
        // - All splash attacks do 50% dmg to the adjacent foes
        // - Heal's damage stat is actually HP to heal

        //Find the affected targets. For splash, [0] is the target, [1] and [2] are adjacent
        List<GameObject> targets = GetTargets(attacker, attack, allies, enemies, allyTarget, enemyTarget);

        //Play sound effect and animation
        AudioSource.PlayClipAtPoint(Resources.Load("Sounds/PlayerChop") as AudioClip, Vector3.zero);
        //attacker.GetComponent<Animator>().Play(attack.AnimationName);
        //Show attack popup
        ShowPopup(attack.AttackName, 1.2f, Color.white, 14, attacker.transform.position, false);
        //Notify DB of attack
        if (isMainPlayer)
        {
            _networkManager.DBExecuteAttack();
        }

        //Apply damage/healing, buffs, effects to all targets
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var targetStats = target.GetComponent<PlayerStats>();
            //Debug.Log(attacker.name + " target: " + target.name);

            float splashFactor = (i > 0 && attack.Target == AttackTarget.SPLASH ? 0.5f : 1f);
            float potency = attack.Damage;
            int damage = 0;
            //Calculate physical damage
            if (attack.Type == AttackType.PHYSICAL)
            {
                //Calculate damage to do (always do at least 1 damage)
                int playerAttack = attackerStats.EffectivePhysicalAttack();
                int enemyDefense = targetStats.EffectivePhysicalDefense();
                float attackScaled = (float)playerAttack / 10;
                float damageReduction = targetStats.DamageReduction();
                damage = Mathf.RoundToInt((potency - enemyDefense) * attackScaled * splashFactor * damageReduction);
                if (damage < 1) damage = 1;
                //Event
                if (isMainPlayer)
                {
                    _eventManager.Event(new List<object>() { GameAction.ATTACK_MELEE, attack.ID, damage });
                }
            }
            //Calculate magical damage (always do at least 1 damage)
            else if(attack.Type == AttackType.MAGICAL)
            {
                //Calculate damage to do
                int playerAttack = attackerStats.EffectiveMagicalAttack();
                int enemyDefense = targetStats.EffectiveMagicalDefense();
                float attackScaled = (float)playerAttack / 10;
                float damageReduction = targetStats.DamageReduction();
                damage = Mathf.RoundToInt((potency - enemyDefense) * attackScaled * splashFactor * damageReduction);
                if (damage < 1) damage = 1;
                if (isMainPlayer)
                {
                    _eventManager.Event(new List<object>() { GameAction.ATTACK_MAGIC, attack.ID, damage });
                }
            }
            //Calculate healing
            else if(attack.Type == AttackType.HEAL)
            {
                //Scales based on magical attack
                float attackScaled = (float)attackerStats.EffectiveMagicalAttack() / 10;
                damage = Mathf.RoundToInt(potency * attackScaled * splashFactor);
                if (isMainPlayer)
                {
                    _eventManager.Event(new List<object>() { GameAction.ATTACK_HEAL, attack.ID, damage });
                }
            }
            //Apply any buffs or effects to target
            //bool buffSuccess =
            ApplyBuff(target, attack.BuffType, attack.BuffDuration, attack.BuffPotency, attack.BuffChance);
            bool effectSuccess = ApplyEffect(target, attacker, attack.Effect, attack.EffectDuration, attack.EffectPotency, attack.EffectChance);
            //If PHYSICAL or MAGICAL attack, inflict damage and show popup
            if (attack.Type == AttackType.PHYSICAL || attack.Type == AttackType.MAGICAL)
            {
                targetStats.InflictDamage(damage);
                ShowPopup(damage.ToString(), 1.2f, Color.red, 30, target.transform.position, false, -.001f, .002f);
                //Apply any on-hit bonuses to the attacker and on-get-hit bonuses to the target
                attackerStats.ApplyHitEnchantments();
                targetStats.ApplyGetHitEnchantments();
            }
            //If HEAL attack, heal and show popup
            else if(attack.Type == AttackType.HEAL)
            {
                targetStats.Heal(damage);
                ShowPopup(damage.ToString(), 1.2f, Color.green, 30, target.transform.position, false, -.001f, .002f);
            }
            //Remove any stun effect unless it was just applied
            if (!effectSuccess || attack.Effect != AttackEffect.STUN)
            {
                RemoveEffect(target, AttackEffect.STUN, 0);
            }
            //If target is now dead, update combat counters
            if(targetStats.IsDead)
            {
                //Apply kill bonus
                attackerStats.ApplyKillEnchantments();

                //Main player - end combat immediately
                if(target.tag == "Player")
                {
                    //Set health to 0
                    targetStats.Heal(targetStats.Health * -1);
                    //On the next tick, combat will end
                    _numberDead = Players.Count;
                }
                //CPU ally - increment counter
                else if(target.tag == "CPU_Player")
                {
                    //Set health to 0
                    targetStats.Heal(targetStats.Health * -1);
                    _numberDead++;
                    //Move to "dead allies" list
                    Players.Remove(target);
                    _deadPlayers.Add(target);
                    //Recalculate positions
                    RecalculatePositions();
                }
                //Enemy - remove them
                else
                {
                    _networkManager.DBDefeatEnemies(1);
                    //If player made killing blow, send event
                    if(attacker.tag == "Player")
                    {
                        _eventManager.Event(new List<object>() { GameAction.MONSTER_KILLED });
                    }
                    var controller = target.GetComponent<EnemyController>();
                    controller.CombatUI.SetActive(false);
                    controller.BuffIcon.gameObject.SetActive(false);
                    target.SetActive(false);
                    Enemies.Remove(target);
                    RecalculatePositions();
                }
                //If this killed a target of a taunt, clear the taunt
                if(attackerStats.TauntTarget == target)
                {
                    attackerStats.SetTauntTarget(null);
                    attackerStats.RemoveEffect(AttackEffect.TAUNT);
                }
            }
        }

        //Return the attack's cooldown
        return attack.Cooldown * attackerStats.GetEffectiveCooldownModifier();
    }

    private bool ApplyBuff(GameObject target, AttackBuff buff, float duration, float potency, float chance)
    {
        if (buff == AttackBuff.NONE) return false;
        //Apply buff
        bool success = target.GetComponent<PlayerStats>().AddBuff(buff, potency, chance);
        if (!success) return false;
        //If it succeeded, show a popup and remove the buff in a few seconds
        string message = (potency > 0 ? "+" : "-") + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(buff.ToString().Replace("_", " ").ToLower());
        Color color = (potency > 0 ? Color.green : Color.red);
        ShowPopup(message, 2, color, 15, target.transform.position, false, -.001f, .002f);
        StartCoroutine(RemoveBuff(target, buff, duration, potency));
        return true;
    }

    private bool ApplyEffect(GameObject target, GameObject attacker, AttackEffect effect, float duration, float potency, float chance)
    {
        if (effect == AttackEffect.NONE) return false;
        //Apply effect
        bool success = target.GetComponent<PlayerStats>().AddEffect(effect, potency, chance);
        if (!success) return false;
        //If it succeeded, show a popup and remove the effect in a few seconds
        string message = null;
        switch(effect)
        {
            case AttackEffect.PARALYZE: message = "Paralyzed!"; break;
            case AttackEffect.POISON:   message = "Poisoned!";  break;
            case AttackEffect.STUN:     message = "Stunned!";   break;
            case AttackEffect.TAUNT:    message = "Taunted!";   break;
        }
        ShowPopup(message, 2, Color.red, 15, target.transform.position, false, -.001f, .002f);
        StartCoroutine(RemoveEffect(target, effect, duration));
        //If this was a taunt, update the taunt target
        if(effect == AttackEffect.TAUNT)
        {
            target.GetComponent<PlayerStats>().SetTauntTarget(attacker);
        }
        return true;
    }

    private IEnumerator RemoveBuff(GameObject target, AttackBuff buff, float duration, float potency)
    {
        yield return new WaitForSeconds(duration);
        target.GetComponent<PlayerStats>().RemoveBuff(buff, potency);
    }

    private IEnumerator RemoveEffect(GameObject target, AttackEffect effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        target.GetComponent<PlayerStats>().RemoveEffect(effect);
    }
    
    //give rewards to player after combat as directed by the combat portal's specifications
    public void GiveRewards()
    {
        CombatDetails details = CombatDetails.GetComponent<CombatDetails>();
        foreach (GameObject player in Players)
        {
            if (!player) continue;
            if (!player.GetComponent<PlayerController>().CPU)//dont give rewards to cpu players
            {
                List<int> attemptedRewards = new List<int>();
                int rewardsGiven = 0;
                List<GameObject> rewards = new List<GameObject>();
                List<float> rates = details.rewardRates;
                List<GameObject> item = details.rewards;
                List<bool> guaranteedDrops = details.guaranteedDrop;

                //Add guaranteed drop items as well
                for (int i = 0; i < guaranteedDrops.Count; i++)
                {
                    if (guaranteedDrops[i] == true)
                    {
                        rewards.Add(item[i]);
                        rewardsGiven++;
                        attemptedRewards.Add(i);
                    }
                }
                //loop through rates giving cooresponding reward if rng says so, beaks if maxrewards given is met
                for (int i = 0; i < rates.Count; i++)
                {
                    if (attemptedRewards.Count >= rates.Count) break;
                    if (rewardsGiven >= details.maxRewards) break;
                    float reward = Random.Range(0f, 1f);
                    //randomly select an item
                    int index;
                    while (attemptedRewards.Contains(index = Random.Range(0, rates.Count)))
                        if (attemptedRewards.Contains(index)) continue;
                    //if ((reward < rates[index] || (details.minRewards != 0 && details.minRewards - rewardsGiven == rates.Count - i)) && guaranteedDrops[index] == false)
                    if (reward<rates[index] || (details.minRewards != 0 && details.minRewards - rewardsGiven == rates.Count - i))
                    {
                        //Debug.Log("Adding " + item[index] + " to inventory as battle reward");
                        rewards.Add(item[index]);
                        rewardsGiven++;
                    }
                    attemptedRewards.Add(index);
                }

                //Add rewards to inventory
                foreach(GameObject reward in rewards)
                {
                    player.GetComponent<Inventory>().AddItemToInventory(reward.GetComponent<Item>());
                    player.GetComponent<EventManager>().Event(new List<object>() { GameAction.LOOTED, reward.GetComponent<Item>().ID, 1 });
                }

                _networkManager.DBGetCombatRewards(rewardsGiven);
                //give xp and gold
                int xp = Random.Range(details.minXp, CombatDetails.GetComponent<CombatDetails>().maxXp);
                int gold = Random.Range(details.minGold, CombatDetails.GetComponent<CombatDetails>().maxGold);
                player.GetComponent<Inventory>().AddGold(gold);
                bool levelup = player.GetComponent<PlayerController>().AddXP(xp);
                
                //display what was given
                StartCoroutine(PrintRewards(rewards, xp, gold,levelup));
            }
        }
    }

    //prints list of rewards, xp and gold
    IEnumerator PrintRewards(List<GameObject> rewards, int xp, int gold, bool levelup)
    {
        yield return new WaitForSeconds(2f);
        float x = camera.pixelWidth * .5f, y = camera.pixelHeight * 5f;
        Vector3 pos = new Vector3(x, y, 0);
        foreach (GameObject reward in rewards)
        {
            ShowPopup(reward.GetComponent<Item>().Name + " found!", 2f, Color.cyan, 20, camera.ScreenToWorldPoint(pos), true, -.001f, .002f);
            yield return new WaitForSeconds(2f);
        }
        ShowPopup("Gained " + xp.ToString() + " xp!", 2f, Color.cyan, 20, camera.ScreenToWorldPoint(pos), true, -.001f, .002f);
        yield return new WaitForSeconds(2f);
        ShowPopup("Gained " + gold.ToString() + " gold!", 2f, Color.cyan, 20, camera.ScreenToWorldPoint(pos), true, -.001f, .002f);
        yield return new WaitForSeconds(2f);
        if (levelup)
        {
            ShowPopup("Level Up!", 2f, Color.cyan, 20, camera.ScreenToWorldPoint(pos), true, -.001f, .002f);
            yield return new WaitForSeconds(2f);
        }
        _postBattle = 2;
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _postBattle = 2;
    }

    //uses popup class to display a popup message
    public Popup ShowPopup(string message, float time, Color color, int size, Vector3 position, bool local, float moveX = 0, float moveY=0)
    {
        GameObject popup = Instantiate(Resources.Load("Popup") as GameObject);
        popup.transform.SetParent(canvas.transform);
        popup.GetComponent<Popup>().Set(message,time,color,size,position,local,moveX,moveY);
        return popup.GetComponent<Popup>();
    }

    //Shows popup in middle of screen
    private Popup ShowPopupInMiddleOfScreen(string message, float time, Color color, int size, bool local, float moveX, float moveY)
    {
        //Calculate position of middle of screen
        float x = camera.pixelWidth * .5f;
        float y = camera.pixelHeight * 5f;
        Vector3 pos = new Vector3(x, y, 0);
        return ShowPopup(message, time, color, size, camera.ScreenToWorldPoint(pos), local, moveX, moveY);
    }

    //hardcoded for player 1 atm, will be fixed with multiplayer
    //for button use
    public void AttackHelper(AttackDetails attack)
    {
        if(Players[0].GetComponent<PlayerStats>().Cooldown <= 0)
            Players[0].GetComponent<PlayerController>().DoAttack(attack);
        else
        {
            float x = camera.pixelWidth * .5f, y = camera.pixelHeight * 5f;
            Vector3 pos = new Vector3(x, y, 0);
            ShowPopup("Not Ready!", 1.5f, Color.red, 20, camera.ScreenToWorldPoint(pos), true);
        }
    }

    //When anyone dies, reset everyone's positions
    private void RecalculatePositions()
    {
        //Reset player positions
        for(int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<PlayerStats>().CombatPosition = i;
        }
        //Reset enemy positions
        for(int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].GetComponent<PlayerStats>().CombatPosition = i;
        }
    }
    
    private List<GameObject> GetTargets(GameObject attacker, AttackDetails attack, List<GameObject> allies, List<GameObject> enemies, int allyTarget, int enemyTarget)
    {
        //Create a list of targets. The first target will be the primary one (for splash attacks)
        List<GameObject> targets = new List<GameObject>();

        switch(attack.Target)
        {
            case AttackTarget.SELF:
                targets.Add(attacker);
                break;
            case AttackTarget.SINGLE:
                targets.Add(enemies[enemyTarget]);
                break;
            case AttackTarget.SPLASH:
                targets.Add(enemies[enemyTarget]);
                if(enemyTarget > 0)
                {
                    targets.Add(enemies[enemyTarget - 1]);
                }
                if(enemyTarget < (enemies.Count - 1))
                {
                    targets.Add(enemies[enemyTarget + 1]);
                }
                break;
            case AttackTarget.TEAM:
                foreach(GameObject go in allies)
                {
                    targets.Add(go);
                }
                break;
        }
        return targets;
    }
    
    //End combat
    private void EndCombat(bool playerWon)
    {
        //TO DO - handle event code via if(_isEvent)

        var player = Players[0].GetComponent<PlayerController>();

        //Clear combat flags and such
        player.Stats.Reset();
        player.fromCombat = true;
        player.InCombat = false;
        //Save if player succeeded in combat or not
        player.FailedCombat = !playerWon;
        //Destroy extra players
        for (int i = 1; i < Players.Count; i++)
        {
            Destroy(Players[i]);
        }
        //If player won, destroy the combat detailsx
        if(playerWon)
        {
            Destroy(CombatDetails);
        }
        //Else add the combat instance back to the world
        else
        {
            player.nextLevel.objects.Add(CombatDetails);
            player.nextLevel.objectLocations.Add(CombatDetails.transform.position);
        }
        //Resize sprite
        player.transform.localScale = _playerScale;
        //Finally, reload the level
        player.ReloadLevel();
    }
}
