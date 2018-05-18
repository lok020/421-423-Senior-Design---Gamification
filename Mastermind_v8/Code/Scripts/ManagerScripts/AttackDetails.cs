using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AttackDetails
{
    public int ID;                      //Unique ID number
    public string AttackName;           //Name of attack
    public AttackType Type;             //Attack type
    public AttackTarget Target;         //Attack target
    public float Damage;                //Base damage (for attacks) or base HP healed for HEAL attacks
    public float Cooldown;              //Cooldown time
    public AttackBuff BuffType;         //Type of buff, if any
    public float BuffChance;            //Chance of buff activating, in percent (like 0.5)
    public float BuffDuration;          //Duration of buff, in seconds
    public float BuffPotency;           //Strength
    public AttackEffect Effect;         //Attack effect, if any
    public float EffectChance;          //Chance of effect activating
    public float EffectDuration;        //Duration of effect
    public float EffectPotency;         //Effect strength

    public Sprite Icon;                 //Pretty icon used in GUI
    public string AnimationName;        //Name of animation used when attacking

    public AttackDetails(AttackDetailsInstance original)
    {
        AttackName = original.AttackName;
        Type = original.Type;
        Target = original.Target;
        Damage = original.Damage;
        Cooldown = original.Cooldown;
        BuffType = original.BuffType;
        BuffChance = original.BuffChance;
        BuffDuration = original.BuffDuration;
        BuffPotency = original.BuffPotency;
        Effect = original.Effect;
        EffectChance = original.EffectChance;
        EffectDuration = original.EffectDuration;
        EffectPotency = original.EffectPotency;

        Icon = original.Icon;
        AnimationName = original.AnimationName;
    }

    //max buff values, need to balance
    public static float GetMaxBuff(string buff)
    {
        switch (buff)
        {
            case "DamageReduction":
                return 1f;
            case "Haste":
                return 2f;
            case "Cover":
                return 4f;
            case "NullDamage":
                return 5f;
            case "Poison":
                return .2f;
            case "Paralyze":
                return .6f;
            case "Confusion":
                return .5f;
            default:
                return .8f;
        }
    }
}
