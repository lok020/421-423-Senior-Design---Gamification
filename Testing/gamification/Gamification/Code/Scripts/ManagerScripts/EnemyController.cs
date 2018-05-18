using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

    //Main stats. Ignore any main player-specific fields
    public PlayerStats Stats;
    
    public string Name;
    public string animationName;
    public BuffIcon BuffIcon;
    public List<AttackDetailsInstance> lAtks, gAtks, hAtks;
    public float LowAtkBias = 0.5f;
    public float GenAtkBias = 0.3f;
    public float HighAtkBias = 0.2f;
    public GameObject CombatUI;
    public float allyHealThreshold=.5f;
    public float lowHealthThreshold=.3f , highHealthThreshold = .8f;
    public int healsRemaining = 1;

    private List<AttackDetails> _lowAtks, _genAtks, _highAtks;
    private CombatManager _combatManager;
    private int _enemyTarget, _friendlyTarget;
    private float _atkTimer;
    private bool _atkWaited;

    // Use this for initialization
    void Awake () {
        Stats = GetComponent<PlayerStats>();
        _combatManager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
        LoadAttacks();
        //Hides an annoying warning
        var anim = GetComponent<Animator>();
        foreach(AnimatorControllerParameter param in anim.parameters)
        {
            if(param.name == "Combat")
            {
                anim.SetBool("Combat", true);
            }
        }
        _atkWaited = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Buff icons
        BuffIcon.Set(Stats);

        //Attack - can only do this if we are able to move and cooldown is done
        if (Stats.Cooldown <= 0 && !Stats.UnableToMove && _atkWaited)
        {
            //Get target
            Target();

            //Do not heal if above high health threshold (or if no heals remain)
            bool doNotHeal = (float)Stats.Health / Stats.BaseHealth >= highHealthThreshold || healsRemaining == 0;
            //Must heal if below low health threshold (if possible)
            bool mustHeal = (float)Stats.Health / Stats.BaseHealth < lowHealthThreshold && healsRemaining > 0;

            //Roll to perform low, general, or high attack
            float attackRoll = Random.Range(0, 1);
            AttackDetails attack = null;
            
            //If high attack, use high attack
            if(attackRoll < HighAtkBias && _highAtks.Count > 0 && !mustHeal)
            {
                var list = doNotHeal ? _highAtks.FindAll(x => x.Type != AttackType.HEAL) : _highAtks;
                if(list.Count > 0)
                {
                    attack = list[Random.Range(0, list.Count)];
                }
            }
            //If general roll or high roll failed, use general attack
            if (attackRoll < HighAtkBias + GenAtkBias && _genAtks.Count > 0 && attack == null && !mustHeal)
            {
                var list = doNotHeal ? _genAtks.FindAll(x => x.Type != AttackType.HEAL) : _genAtks;
                if (list.Count > 0)
                {
                    attack = list[Random.Range(0, list.Count)];
                }
            }
            //If low roll or above rolls failed, use low attack
            if (_lowAtks.Count > 0 && attack == null && !mustHeal)
            {
                var list = doNotHeal ? _lowAtks.FindAll(x => x.Type != AttackType.HEAL) : _lowAtks;
                if (list.Count > 0)
                {
                    attack = list[Random.Range(0, list.Count)];
                }
            }
            //If we need to heal, get the best healing attack available
            if(mustHeal)
            {
                var list = _highAtks.FindAll(x => x.Type == AttackType.HEAL);
                list.AddRange(_genAtks.FindAll(x => x.Type == AttackType.HEAL));
                list.AddRange(_lowAtks.FindAll(x => x.Type == AttackType.HEAL));
                if (list.Count > 0)
                {
                    attack = list[0];
                }
            }

            //If attack is still null, either we have no heals left or no attacks at all, in which case do nothing for a second
            if(attack == null)
            {
                Debug.Log("ERROR - NO ATTACKS AVAILABLE");
                Stats.Cooldown = 1;
                return;
            }

            //Apply attack
            var combat = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
            Stats.Cooldown = combat.ExecuteAttack(gameObject, attack, combat.Enemies, combat.Players, _friendlyTarget, _enemyTarget);

            //Update heal counter
            if (attack.Type == AttackType.HEAL)
            {
                healsRemaining--;
                if (healsRemaining <= 0)
                {
                    _highAtks.Remove(attack);
                    _genAtks.Remove(attack);
                    _lowAtks.Remove(attack);
                }
            }
            //Wait for next attack
            _atkWaited = false;
        }
        //Wait a bit between attacks
        if (Stats.Cooldown <= 0 && _atkWaited == false)
        {
            _atkTimer = Random.Range(0, 1.5f);
        }
        _atkTimer -= Time.deltaTime;
        if (Stats.Cooldown <= 0 && _atkTimer < 0)
        {
            _atkWaited = true;
        }
	}

    void LoadAttacks()
    {
        _lowAtks = new List<AttackDetails>();
        _genAtks = new List<AttackDetails>();
        _highAtks = new List<AttackDetails>();
        foreach (AttackDetailsInstance a in lAtks)
        {
            _lowAtks.Add(new AttackDetails(a));
        }
        foreach (AttackDetailsInstance a in gAtks)
        {
            _genAtks.Add(new AttackDetails(a));
        }
        foreach (AttackDetailsInstance a in hAtks)
        {
            _highAtks.Add(new AttackDetails(a));
        }
    }

    //targeting, returns true if enemy is taunted thus attack is forced or a heal is needed
    //Targeting. If taunted by a player it targets them, else it targets randomly.
    //If healing is needed, target itself, else target a random ally. TO DO - heal allies.
    public void Target()
    {
        _friendlyTarget = Random.Range(0, _combatManager.Enemies.Count);

        //If taunted, 80% chance it targets them, else it targets randomly
        if (Stats.TauntTarget != null && (Random.Range(0, 1) < 0.8))
        {
            _enemyTarget = _combatManager.Players.IndexOf(Stats.TauntTarget);
        }
        else
        {
            _enemyTarget = Random.Range(0, _combatManager.Players.Count);
        }
    }
}
