using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Use this class to set the initial stats for CPUs and enemies. Make sure
//it is below the PlayerStats script!
public class CombatStats : MonoBehaviour {

    //Base numerical stats
    public int PhysicalAttack = 10;  //Determines damage done by physical attacks
    public int PhysicalDefense = 10; //Determines damage taken from physical attacks
    public int MagicalAttack = 10;   //Determines damage done by magical attacks
    public int MagicalDefense = 10;  //Determines damage taken from magical attacks
    public float Speed = 10f;        //Affects cooldowns

    //Health
    public int BaseHealth = 100;    //Maximum health when at 100% health
    public int MaxHealth = 100;     //Maximum health when overcharged by healing
    //public int HealthRegen = 0;     //Health regenerated every second

    //Flag - is this an enemy (true) or ally (false)
    public bool IsEnemy;

    //Active buffs and debuffs
    public Dictionary<AttackBuff, float> ActiveBuffs = new Dictionary<AttackBuff, float>();

    //Active effects
    public Dictionary<AttackEffect, float> ActiveEffects = new Dictionary<AttackEffect, float>();
    
    //Loads these stats into the player stats
    public void Start()
    {
        var stats = GetComponent<PlayerStats>();
        stats.InitFromScript(PhysicalAttack, PhysicalDefense, MagicalAttack, MagicalDefense, Speed, BaseHealth, MaxHealth, IsEnemy);
    }
}
