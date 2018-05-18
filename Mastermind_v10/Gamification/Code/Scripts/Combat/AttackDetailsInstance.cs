using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public enum AttackType { PHYSICAL, MAGICAL, BUFF, DEBUFF, HEAL, PASSIVE }
public enum AttackTarget { SELF, TEAM, SINGLE, SPLASH }
public enum AttackBuff { NONE, PHYSICAL_ATTACK, PHYSICAL_DEFENSE, MAGICAL_ATTACK, MAGICAL_DEFENSE, DAMAGE_REDUCTION, SPEED }
public enum AttackEffect { NONE, STUN, TAUNT, PARALYZE, POISON, SAVING_THROW }

public class AttackDetailsInstance : MonoBehaviour {
    public int ID;                      //Unique ID number
    public string AttackName;           //Name of attack
    public AttackType Type;             //Attack type
    public AttackTarget Target;         //Attack target
    public float Damage;                //Base damage of attack, OR base HP healed for HEAL attacks
    public float Cooldown;              //Cooldown time
    public AttackBuff BuffType;         //Type of buff, if any
    public float BuffChance;            //Chance of buff activating, in percent (like 0.5)
    public float BuffDuration;          //Duration of buff, in seconds
    public float BuffPotency;           //Strength of buff
    public AttackEffect Effect;         //Attack effect, if any
    public float EffectChance;          //Chance of effect activating
    public float EffectDuration;        //Duration of effect
    public float EffectPotency;         //Potency of effect

    public Sprite Icon;                 //Pretty icon used in GUI
    public string AnimationName;        //Name of animation used when attacking
    
    //Dictionary that maps ID numbers to the resource path of the actual attack
    public static Dictionary<int, string> SkillPathnames = new Dictionary<int, string>()
    {
        { 1, "Attacks/Punch" },
        { 2, "Attacks/Heavy Attack" },
        { 3, "Attacks/Wide Slash" },
        { 4, "Attacks/Heavy Sweep" },
        { 5, "Attacks/Fire Bolt" },
        { 6, "Attacks/Ice Bolt" },
        { 7, "Attacks/Fireball" },
        { 8, "Attacks/Blizzard" },
        { 9, "Attacks/Precise Strike" },
        { 0, "Attacks/Freezing Mist" },
        { 11, "Attacks/Crushing Blow" },
        { 12, "Attacks/Overload" },
        { 13, "Attacks/Poison Dart" },
        { 14, "Attacks/Toxic Bolt" },
        { 15, "Attacks/Bandage" },
        { 16, "Attacks/Recover" },
        { 17, "Attacks/First Aid" },
        { 18, "Attacks/Heal Wounds" },
        { 19, "Attacks/Adrenaline Rush" },
        { 20, "Attacks/Defensive Stance" },
        { 21, "Attacks/Concentrate" },
        { 22, "Attacks/Meditate" },
        { 23, "Attacks/Fortify" },
        { 24, "Attacks/Battlecry" },
        { 25, "Attacks/Shield Wall" },
        { 26, "Attacks/Invigorate" },
        { 27, "Attacks/Ward" },
        { 28, "Attacks/Protection" },
        { 29, "Attacks/Intimidate" },
        { 30, "Attacks/Deafening Roar" },
        { 31, "Attacks/Distract" },
        { 32, "Attacks/Weaken" },
        { 33, "Attacks/Withering Touch" },
        { 34, "Attacks/Threaten" },
        { 35, "Attacks/Strong Arm" },
        { 36, "Attacks/Thick Skin" },
        { 37, "Attacks/Focused" },
        { 38, "Attacks/Strong Will" },
        { 39, "Attacks/Sure Footed" }
    };

    public string TooltipText()
    {
        StringBuilder sb = new StringBuilder();

        string type = null;
        string targets = null;
        string effect = null;
        string effectType = null;
        string buff = null;
        string buffType = null;
        switch (BuffType)
        {
            case AttackBuff.DAMAGE_REDUCTION:
                buffType = "Damage";
                break;
            case AttackBuff.MAGICAL_ATTACK:
                buffType = "Magical Attack";
                break;
            case AttackBuff.MAGICAL_DEFENSE:
                buffType = "Magical Defense";
                break;
            case AttackBuff.PHYSICAL_ATTACK:
                buffType = "Physical Attack";
                break;
            case AttackBuff.PHYSICAL_DEFENSE:
                buffType = "Physical Defense";
                break;
            case AttackBuff.SPEED:
                buffType = "Speed";
                break;
        }
        switch (Effect)
        {
            case AttackEffect.PARALYZE:
                effectType = "Paralyze";
                break;
            case AttackEffect.POISON:
                effectType = "Poison";
                break;
            case AttackEffect.STUN:
                effectType = "Stun";
                break;
            case AttackEffect.TAUNT:
                effectType = "Taunt";
                break;
        }
        switch (Type)
        {
            case AttackType.BUFF:
                type = "Buff";
                break;
            case AttackType.DEBUFF:
                type = "Debuff";
                break;
            case AttackType.HEAL:
                type = "Heal";
                break;
            case AttackType.MAGICAL:
                type = "Magic";
                break;
            case AttackType.PASSIVE:
                type = "Passive";
                break;
            case AttackType.PHYSICAL:
                type = "Physical";
                break;
        }
        switch (Target)
        {
            case AttackTarget.SELF:
                targets = "Self";
                break;
            case AttackTarget.SINGLE:
                targets = "Single";
                break;
            case AttackTarget.SPLASH:
                targets = "Splash";
                break;
            case AttackTarget.TEAM:
                targets = "Team";
                break;
        }
        if (BuffType != AttackBuff.NONE)
        {
            buff = (BuffPotency > 0 == (BuffType != AttackBuff.DAMAGE_REDUCTION) ? "Increase " : "Decrease ")
                + buffType + " by " + (Mathf.Abs(BuffPotency) * 100).ToString("0\\%");
            if(BuffDuration > 0)
                buff += " for " + BuffDuration.ToString() + " sec";
            if (BuffChance < 1 && BuffChance > 0)
                buff += " (" + (BuffChance * 100).ToString("0\\%") + ")";
        }
        if (Effect != AttackEffect.NONE)
        {
            effect = effectType + " for " + EffectDuration.ToString() + " sec";
            if (EffectChance < 1 && EffectChance > 0)
                effect += " (" + (EffectChance * 100).ToString("0\\%") + ")";
        }

        sb.Append(String.Format("{0,-18} {1,10}", "Skill Type", type) +
                  String.Format("\n{0,-18} {1,10}", "Cooldown", Cooldown + " sec"));
        if (Damage > 0)
        {
            if (Type == AttackType.HEAL)
            {
                sb.Append(String.Format("\n{0,-18} {1,10}", "Potency", Damage + " HP"));
            }
            else
            {
                sb.Append(String.Format("\n{0,-18} {1,10}", "Damage", Damage));
            }
        }
        sb.Append(String.Format("\n{0,-18} {1,10}", "Target", targets));
        if (buff != null)
        {
            sb.Append(String.Format("\n" + buff));
        }
        if (effect != null)
        {
            sb.Append(String.Format("\n" + effect));
        }

        return sb.ToString();
    }
}
