using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour {

    //stat enum, update buffs etc to use this
    public enum Stat
    {
        PHYSICAL_ATTACK, MAGICAL_ATTACK, PHYSICAL_DEFENSE, MAGICAL_DEFENSE, SPEED
    };

    //Main character only info. Ignore this for CPUs, enemies, etc
    public int PlayerLevel { get; private set; }    //Player's level
    public int XP { get; private set; }             //Current player xp
    public int SkillPoints { get; private set; }    //Number of skillpoints held by the player
    public int StatPoints { get; private set; }     //Number of statpoints held by player

    //Base numerical stats. This is before any modifiers
    public int PhysicalAttack { get; private set; }     //Determines damage done by physical attacks
    public int PhysicalDefense { get; private set; }    //Determines damage taken from physical attacks
    public int MagicalAttack { get; private set; }      //Determines damage done by magical attacks
    public int MagicalDefense { get; private set; }     //Determines damage taken from magical attacks
    public float Speed { get; private set; }            //Affects cooldowns

    //Health
    public int Health;// { get; private set; }         //Current health points - if this hits 0, you die
    public int BaseHealth { get; private set; }     //Maximum health when at 100% health
    public int MaxHealth { get; private set; }      //Maximum health when overcharged by healing
    //public int HealthRegen = 0; //Health regenerated every second

    //Cooldown timer
    public float Cooldown;
    public bool ResetCooldown;

    //Position in relevant GameObject list (Players, Enemies, etc)
    public int CombatPosition;

    //Flags
    public bool IsDead { get; private set; }        //Self explanatory
    public bool UnableToMove { get; private set; }  //Inflicted by "Stun" and "Paralyze"
    public bool IsEnemy { get; private set; }       //Whether this is an enemy (true) or ally (false)
    
    //Active buffs and debuffs
    public Dictionary<AttackBuff, float> ActiveBuffs = new Dictionary<AttackBuff, float>();

    //Active effects
    public Dictionary<AttackEffect, float> ActiveEffects = new Dictionary<AttackEffect, float>();

    //Equipment, if any. Can be linked externally (like from player controller)
    public List<Item> Equipment = new List<Item>();

    //Skills - only used by CPUs and enemies
    public Dictionary<AttackDetails, int> Skills;

    //Passives
    public Dictionary<AttackBuff, float> Passives = new Dictionary<AttackBuff, float>();

    //Enchantments, separated by trigger
    public List<Enchantment> PassiveEnchantments = new List<Enchantment>();
    public List<Enchantment> OnHitEnchantments = new List<Enchantment>();
    public List<Enchantment> OnGetHitEnchantments = new List<Enchantment>();
    public List<Enchantment> OnKillEnchantments = new List<Enchantment>();

    //Taunting target, if there is one
    public GameObject TauntTarget { get; private set; }

    //References
    private MessageOverlayController _messageOverlay;
    private NetworkManager _network;

    //Links to PlayerController, if there is one
    private bool _linked = false;

    
    // Use this for initialization
    void Start()
    {
        _messageOverlay = null;
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        PlayerLevel = 1;
        XP = 0;
        SkillPoints = 0;
        StatPoints = 0;
        PhysicalAttack = 10;
        MagicalAttack = 10;
        PhysicalDefense = 10;
        MagicalDefense = 10;
        BaseHealth = 400;
        Speed = 10f;
        IsEnemy = false;
        TauntTarget = null;

        StartCoroutine(SecondTick());
    }

    void Update()
    {
        if(!_linked)
        {
            var playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                Equipment = playerController.equippedGear;
            }
            _linked = true;
        }
        if(Cooldown > 0)
        {
            Cooldown -= Time.deltaTime;
        }
        else
        {
            Cooldown = 0;
        }
    }

    //Applies buff, returns if successful
    public bool AddBuff(AttackBuff buff, float potency, float chance)
    {
        //If the chance roll fails, return false
        if (Random.Range(0f, 1f) > chance) return false;
        //Apply buff
        if (ActiveBuffs.ContainsKey(buff))
        {
            ActiveBuffs[buff] += potency;
        }
        else
        {
            ActiveBuffs[buff] = potency;
        }
        //Buff successful
        return true;
    }

    //Applies effect, returns if successful
    //Unlike buffs, effects cannot stack, so this only adds if the effect isn't already present
    //When paralyzed, no effects will be added
    public bool AddEffect(AttackEffect effect, float potency, float chance)
    {
        //If the chance roll fails, return
        if (Random.Range(0f, 1f) > chance) return false;
        //If currently paralyzed, return
        if (ActiveEffects.ContainsKey(AttackEffect.PARALYZE) &&
            ActiveEffects[AttackEffect.PARALYZE] > 0) return false;
        //If this effect is already active, return (no effect stacking)
        if (ActiveEffects.ContainsKey(effect) && ActiveEffects[effect] > 0) return false;

        //Apply this effect
        ActiveEffects[effect] = potency;

        //If player is paralyzed or stunned, set flag
        if (effect == AttackEffect.PARALYZE || effect == AttackEffect.STUN)
        {
            UnableToMove = true;
        }
        //Success!
        return true;
    }

    public void SetTauntTarget(GameObject target)
    {
        TauntTarget = target;
    }

    // EFFECTIVE STAT CALCULATION
    //These use the same internal function
    public int EffectivePhysicalAttack()
    {
        return Mathf.RoundToInt(EffectiveStatInternal(PhysicalAttack, AttackBuff.PHYSICAL_ATTACK));
    }

    public int EffectivePhysicalDefense()
    {
        return Mathf.RoundToInt(EffectiveStatInternal(PhysicalDefense, AttackBuff.PHYSICAL_DEFENSE));
    }

    public int EffectiveMagicalAttack()
    {
        return Mathf.RoundToInt(EffectiveStatInternal(MagicalAttack, AttackBuff.MAGICAL_ATTACK));
    }

    public int EffectiveMagicalDefense()
    {
        return Mathf.RoundToInt(EffectiveStatInternal(MagicalDefense, AttackBuff.MAGICAL_DEFENSE));
    }
    
    public float EffectiveSpeed()
    {
        return EffectiveStatInternal(Speed, AttackBuff.SPEED);
    }

    //Gets effective cooldown modifier. Lower is better. Capped at 0.5 (half the cooldown)
    //Currently follows an inverse logarithmic curve, where at 10 speed cooldowns are 100% of their value
    //And at 50 speed they are 50%. All players start with 10 speed.
    //The range will be limited to 200% (long cooldown) and 50% (short cooldown).
    //The equation used is 1 - 0.5 log_5 (x/10), where x is Speed and log_5 is logarithm of base 5.
    public float GetEffectiveCooldownModifier()
    {
        double effective = 1 - (0.5) * System.Math.Log(EffectiveSpeed() / 10.0, 5.0);
        //Set upper bound at 2.0
        if (effective > 2.0) effective = 2.0;
        //Set lower bound to 0.5
        else if (effective < 0.5) effective = 0.5;

        //Return this number as a float
        return (float)effective;
    }

    //Heals character. For now this just restores health and cures poison
    public void Heal(int hp)
    {
        //If dead, do not heal
        if (IsDead) return;

        //Heal up to MaxHealth or BaseHealth, whichever is higher
        int cap = MaxHealth > BaseHealth ? MaxHealth : BaseHealth;
        Health = Health + hp > cap ? cap : Health + hp;

        //If poisoned, cure the poison
        if(ActiveEffects.ContainsKey(AttackEffect.POISON))
        {
            ActiveEffects[AttackEffect.POISON] = 0;
        }
    }

    //Deals damage. Handles some status effects as well
    //Make sure to call this *after* applying any buffs or effects
    public void InflictDamage(int damage)
    {
        //If 0 dmg, return
        if (damage <= 0) return;
        //First, reduce health
        Health -= damage;
        //If Health is now at or below 0, check for saving throw
        if (Health <= 0)
        {
            //Saving throw
            if(ActiveEffects.ContainsKey(AttackEffect.SAVING_THROW))
            {
                //Player survives with 1HP
                RemoveEffect(AttackEffect.SAVING_THROW);
                Health = 1;
                return;
            }
            //Else player dies
            IsDead = true;
            UnableToMove = true;    //This is pretty obvious
            return;
        }
    }

    public float DamageReduction()
    {
        if(ActiveBuffs.ContainsKey(AttackBuff.DAMAGE_REDUCTION))
        {
            return 1.0f - ActiveBuffs[AttackBuff.DAMAGE_REDUCTION];
        }
        return 1.0f;
    }

    //Remove buff
    //To remove a +50% buff, set potency to 0.5 (positive)
    //To remove a -30% debuff, set potency to -0.3 (negative)
    //In other words, the potency should match that of the original buff
    public void RemoveBuff(AttackBuff buff, float potency)
    {
        //Should always be true
        if (ActiveBuffs.ContainsKey(buff))
        {
            ActiveBuffs[buff] -= potency;
        }
    }

    //Remove effect
    //Like with removing buffs, the potency should match that of the origial effect
    public void RemoveEffect(AttackEffect effect)
    {
        //Remove effect if it exists and is active
        if (ActiveEffects.ContainsKey(effect))
        {
            ActiveEffects[effect] = 0;
        }
        //If this means player is no longer stunned or paralyzed, update flag
        if ((!ActiveEffects.ContainsKey(AttackEffect.STUN) || ActiveEffects[AttackEffect.STUN] == 0) &&
           (!ActiveEffects.ContainsKey(AttackEffect.PARALYZE) || ActiveEffects[AttackEffect.PARALYZE] == 0))
        {
            UnableToMove = false;
        }
        //If this means the player is no longer taunted, clear taunt target
        if(!ActiveEffects.ContainsKey(AttackEffect.TAUNT) || ActiveEffects[AttackEffect.TAUNT] == 0)
        {
            SetTauntTarget(null);
        }
    }

    //Checks status every second, applying effects like poison, etc
    private IEnumerator SecondTick()
    {
        yield return new WaitForSeconds(1);
        //Poison - does flat damage
        if(ActiveEffects.ContainsKey(AttackEffect.POISON) && ActiveEffects[AttackEffect.POISON] > 0)
        {
            InflictDamage(Mathf.RoundToInt(ActiveEffects[AttackEffect.POISON]));
        }
        StartCoroutine(SecondTick());
    }

    //Reloads lists of enchantments. Call this whenever equipment is changed
    public void ReloadEnchantments()
    {
        //Clear lists
        PassiveEnchantments.Clear();
        OnHitEnchantments.Clear();
        OnGetHitEnchantments.Clear();
        OnKillEnchantments.Clear();

        //Loop through all equipment and readd enchantments
        foreach(Item i in Equipment)
        {
            if (i == null) continue;
            foreach(Enchantment e in i.Enchantments)
            {
                if (e == null) continue;
                switch(e.EnchantmentTrigger)
                {
                    case Enchantment.Trigger.ON_GET_HIT:
                        OnGetHitEnchantments.Add(e);
                        break;
                    case Enchantment.Trigger.ON_HIT:
                        OnHitEnchantments.Add(e);
                        break;
                    case Enchantment.Trigger.ON_KILL:
                        OnKillEnchantments.Add(e);
                        break;
                    case Enchantment.Trigger.PASSIVE:
                        PassiveEnchantments.Add(e);
                        break;
                }
            }
        }
    }

    //Called when combat starts
    public void LoadPassiveEffects()
    {
        //Loop through every passive enchantment and apply all effects (not buffs)
        foreach(Enchantment e in PassiveEnchantments)
        {
            if(e.Effect != AttackEffect.NONE)
            {
                //Always apply
                AddEffect(e.Effect, e.Potency, 1);
            }
        }
    }

    //Applies enchantments on hitting enemy
    public void ApplyHitEnchantments()
    {
        ApplyEnchantmentsInternal(OnHitEnchantments);
    }

    //Same for getting hit
    public void ApplyGetHitEnchantments()
    {
        ApplyEnchantmentsInternal(OnGetHitEnchantments);
    }

    //Same for killing enemy
    public void ApplyKillEnchantments()
    {
        ApplyEnchantmentsInternal(OnKillEnchantments);
    }
    

    //Calculates effective stat value. HOW THIS WORKS:
    // 1. Get the sum of all numerical stats (base attack, weapon strength, etc)
    // 2. Get the sum of all percent modifiers (buffs, effects, enchantments, etc)
    // 3. Return the final numerical sum scaled by the final percent sum
    private float EffectiveStatInternal(float baseStat, AttackBuff type)
    {
        //Base numerical stat and percent (base percent modifier is 100%, ie no effect)
        int finalNumericalSum = Mathf.RoundToInt(baseStat);
        float finalPercent = 1f;
        //Add all modifiers from equipment
        foreach (Item i in Equipment)
        {
            //Skip null entries of course
            if (i == null) continue;
            //Add base stat
            switch(type)
            {
                case AttackBuff.PHYSICAL_ATTACK:
                    finalNumericalSum += i.PhysicalAttack;
                    break;
                case AttackBuff.PHYSICAL_DEFENSE:
                    finalNumericalSum += i.PhysicalDefense;
                    break;
                case AttackBuff.MAGICAL_ATTACK:
                    finalNumericalSum += i.MagicalAttack;
                    break;
                case AttackBuff.MAGICAL_DEFENSE:
                    finalNumericalSum += i.MagicalAttack;
                    break;
                case AttackBuff.SPEED:
                    finalNumericalSum += i.Speed;
                    break;
            }
        }
        //Add all effects from buffs
        if (ActiveBuffs.ContainsKey(type))
        {
            finalPercent += ActiveBuffs[type];
        }
        //Passives as well
        if(Passives.ContainsKey(type))
        {
            finalPercent += Passives[type];
        }
        //Lastly, passive enchantments
        foreach(Enchantment e in PassiveEnchantments)
        {
            if(e.Buff == type)
            {
                finalPercent += e.Potency;
            }
        }
        //Return the (rounded) product of the final number and final percent
        return finalNumericalSum * finalPercent;
    }

    //Adds effects of enchantment
    private void ApplyEnchantmentsInternal(List<Enchantment> list)
    {
        foreach(Enchantment e in list)
        {
            //Apply buff
            if(e.Buff != AttackBuff.NONE)
            {
                bool success = AddBuff(e.Buff, e.Potency, e.Chance);
                if(success) StartCoroutine(RemoveBuff(e.Buff, e.Duration, e.Potency));
            }
            //Apply effect
            if(e.Effect != AttackEffect.NONE)
            {
                bool success = AddEffect(e.Effect, e.Potency, e.Chance);
                if (success) StartCoroutine(RemoveEffect(e.Effect, e.Duration));
            }
        }
    }

    //Remove buff
    private IEnumerator RemoveBuff(AttackBuff buff, float duration, float potency)
    {
        yield return new WaitForSeconds(duration);
        RemoveBuff(buff, potency);
    }

    //Remove effect
    private IEnumerator RemoveEffect(AttackEffect effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveEffect(effect);
    }

    //Reset to full health, clear flags, etc
    public void Reset()
    {
        Health = BaseHealth;
        ActiveBuffs.Clear();
        ActiveEffects.Clear();
        IsDead = false;
        UnableToMove = false;
    }

    //Called by CombatStats, used for all CPUs and enemies
    public void InitFromScript(int physicalAttack, int physicalDefense, int magicalAttack, int magicalDefense, float speed, int baseHealth, int maxHealth, bool isEnemy)
    {
        PlayerLevel = 1;
        XP = 0;
        SkillPoints = 0;
        StatPoints = 0;
        PhysicalAttack = physicalAttack;
        PhysicalDefense = physicalDefense;
        MagicalAttack = magicalAttack;
        MagicalDefense = magicalDefense;
        Speed = speed;
        BaseHealth = baseHealth;
        MaxHealth = maxHealth;

        IsEnemy = isEnemy;

        //If ally, scale the HP with the player's level
        if(!isEnemy)
        {
            int playerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().PlayerLevel;
            BaseHealth += (8 * playerLevel) - 10;
            MaxHealth += (8 * playerLevel) - 10;
        }

        Health = BaseHealth;
    }

    //Called by PlayerController, only used by the main player
    public void InitFromDB(int playerLevel, int xp, int skillPoints, int statPoints, int physicalAttack, int magicalAttack, int physicalDefense, int magicalDefense, int speed)
    {
        PlayerLevel = playerLevel;
        XP = xp;
        SkillPoints = skillPoints;
        StatPoints = statPoints;
        PhysicalAttack = physicalAttack;
        MagicalAttack = magicalAttack;
        PhysicalDefense = physicalDefense;
        MagicalDefense = magicalDefense;
        Speed = speed;

        BaseHealth = 390 + PlayerLevel * 10;    //Start at 400HP, go up 10HP per level for now
    }

    //takes in a PlayerStats.Stat and uses a skillpoint to level up a stat
    //Effective stats get increased by 1 by default. If you want better scaling, change this
    public void LevelUpStat(Stat stat)
    {
        float newStatValue = 0;
        string statName = null;
        if (StatPoints > 0)
        {
            switch (stat)
            {
                case Stat.PHYSICAL_ATTACK:
                    PhysicalAttack++;
                    newStatValue = PhysicalAttack;
                    statName = "PAtk";
                    break;
                case Stat.MAGICAL_ATTACK:
                    MagicalAttack++;
                    newStatValue = MagicalAttack;
                    statName = "MAtk";
                    break;
                case Stat.PHYSICAL_DEFENSE:
                    PhysicalDefense++;
                    newStatValue = PhysicalDefense;
                    statName = "Pdef";
                    break;
                case Stat.MAGICAL_DEFENSE:
                    MagicalDefense++;
                    newStatValue = MagicalDefense;
                    statName = "MDef";
                    break;
                case Stat.SPEED:
                    Speed++;
                    newStatValue = Speed;
                    statName = "Speed";
                    break;
            }
            StatPoints--;
        }
        _network.UpdatePlayerStat(statName, newStatValue.ToString());
        _network.UpdatePlayerStat("StatPoints", StatPoints.ToString());
    }

    //Same as above, except to decrease the stat
    public void DecreaseStat(Stat stat, float baseStat)
    {
        float newStatValue = 0;
        string statName = null;
        switch (stat)
        {
            case Stat.PHYSICAL_ATTACK:
                if (PhysicalAttack - 1 >= (int)baseStat)
                {
                    PhysicalAttack--;
                    newStatValue = PhysicalAttack;
                    statName = "PAtk";
                    StatPoints++;
                }
                break;
            case Stat.MAGICAL_ATTACK:
                if (MagicalAttack - 1 >= (int)baseStat)
                {
                    MagicalAttack--;
                    newStatValue = MagicalAttack;
                    statName = "MAtk";
                    StatPoints++;
                }
                break;
            case Stat.PHYSICAL_DEFENSE:
                if (PhysicalDefense - 1 >= (int)baseStat)
                {
                    PhysicalDefense--;
                    newStatValue = PhysicalDefense;
                    statName = "Pdef";
                    StatPoints++;
                }
                break;
            case Stat.MAGICAL_DEFENSE:
                if (MagicalDefense - 1 >= (int)baseStat)
                {
                    MagicalDefense--;
                    newStatValue = MagicalDefense;
                    statName = "MDef";
                    StatPoints++;
                }
                break;
            case Stat.SPEED:
                if (Speed - 1 >= baseStat)
                {
                    Speed--;
                    newStatValue = Speed;
                    statName = "Speed";
                    StatPoints++;
                }
                break;
        }
        _network.UpdatePlayerStat(statName, newStatValue.ToString());
        _network.UpdatePlayerStat("StatPoints", StatPoints.ToString());
    }

    //Quality player up
    public void LevelUp()
    {
        //Increase level by 1
        PlayerLevel++;
        //Add two skill point
        AddSkillPoints(2);
        //Add four stat points
        AddStatPoints(4);
        //Save all this
        _network.DBGainLevel();
        _network.UpdatePlayerStat("Level", PlayerLevel.ToString());
        //Notify user
        if(_messageOverlay == null)
        {
            _messageOverlay = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
        }
        _messageOverlay.EnqueueMessage("Level up!");
    }

    //Add experience points
    public void AddXP(int xpPoints)
    {
        XP += xpPoints;
        _network.DBGainXP(xpPoints);
        _network.UpdatePlayerStat("XP", XP.ToString());
    }

    //Add skill points
    public void AddSkillPoints(int skillPoints)
    {
        SkillPoints += skillPoints;
        _network.UpdatePlayerStat("SkillPoints", SkillPoints.ToString());
    }

    //Add skill points
    public void AddStatPoints(int statPoints)
    {
        StatPoints += statPoints;
        _network.UpdatePlayerStat("StatPoints", StatPoints.ToString());
    }

    //Unlocks new skill
    public void UnlockSkill(int skillId, bool activeSkill)
    {
        AddSkillPoints(-1);
        _network.UpdateSkill(skillId, -1, activeSkill);
    }
}
