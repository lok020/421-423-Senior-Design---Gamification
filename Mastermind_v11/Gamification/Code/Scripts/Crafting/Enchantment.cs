using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Enchantment : MonoBehaviour {

    //Currently Effect_Self and Effect_Team are not used
    public enum Type
    {
        BUFF_SELF, BUFF_TEAM, DEBUFF_ENEMY, 
        EFFECT_SELF, EFFECT_TEAM, EFFECT_ENEMY
    }

    public enum Trigger
    {
        PASSIVE, ON_HIT, ON_KILL, ON_GET_HIT
    }

    //First index is EnchantmentType, second is Quality
    //Passives do not use this table
    public static float[,] ChanceTable =
    {
        { 0.20f, 0.25f, 0.30f, 0.35f, 0.40f },
        { 0.10f, 0.12f, 0.15f, 0.18f, 0.20f },
        { 0.15f, 0.18f, 0.20f, 0.22f, 0.25f },
        { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
        { 0.10f, 0.12f, 0.15f, 0.18f, 0.20f },
        { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
    };

    //First index is EnchantmentType, second is Quality
    //Passives do not use this table
    public static float[,] DurationTable =
    {
        {  5f,  7f, 10f, 12f, 15f },
        { 10f, 15f, 20f, 25f, 30f },
        { 10f, 12f, 15f, 18f, 20f },
        {  5f,  7f, 10f, 12f, 15f },
        {  5f, 10f, 14f, 18f, 20f },
        {  2f,  4f,  6f,  8f, 10f },
    };

    //First index is EnchantmentType, second is Quality
    //For effects, only Heal and Poison are affected by higher durations.
    //All other effects are either active or not
    public static float[,] PotencyTable =
    {
        {  0.25f,  0.32f,  0.38f,  0.45f,  0.50f },
        {  0.10f,  0.12f,  0.15f,  0.18f,  0.20f },
        { -0.15f, -0.18f, -0.22f, -0.26f, -0.30f },
        {     5f,     7f,    10f,    12f,    15f },
        {     5f,    10f,    14f,    18f,    20f },
        {     2f,     4f,     6f,     8f,    10f },
    };

    //Dictionary that connects ID number to pathname
    public static Dictionary<int, string> Path = new Dictionary<int, string>()
    {
        //Passive buffs (mixed)
        {  0, "Enchantments/Sharpened" },   // Offensive
        {  1, "Enchantments/Reinforced" },  // Defensive
        {  2, "Enchantments/Channeled" },   // Off
        {  3, "Enchantments/Calming" },     // Def
        {  4, "Enchantments/Balanced O" },  // Off
        {  5, "Enchantments/Balanced D" },  // Def
        //On hit buffs (offensive)
        {  6, "Enchantments/Bloodthirsty" },
        {  7, "Enchantments/Strengthen" },
        {  8, "Enchantments/Conduction" },
        {  9, "Enchantments/Conviction" },
        { 10, "Enchantments/Adrenaline" },
        //On get hit buffs (defensive)
        { 11, "Enchantments/Rage" },
        { 12, "Enchantments/Reactive Armor" },
        { 13, "Enchantments/Targeting" },
        { 14, "Enchantments/Reactive Ward" },
        { 15, "Enchantments/Evasion" },
        //Debuff (offensive)
        { 16, "Enchantments/Shock" },
        { 17, "Enchantments/Puncture" },
        { 18, "Enchantments/Mana Drain" },
        { 19, "Enchantments/Focus Drain" },
        { 20, "Enchantments/Slow" },
        //Passive effects (defensive)
        { 21, "Enchantments/Saving Throw" },
        //On hit effects (offensive)
        { 22, "Enchantments/Toxic Barb" },
        { 23, "Enchantments/Concussive Force" },
        { 24, "Enchantments/Paralytic Shock" },
        //On kill buffs (offensive)
        { 25, "Enchantments/Absorb Strength" },
        { 26, "Enchantments/Absorb Power" },
        { 27, "Enchantments/Absorb Mana" },
        { 28, "Enchantments/Absorb Focus" },
        { 29, "Enchantments/Absorb Agility" },
        //{ 00, "Enchantments/xxxx" },
    };

    //List of ID numbers for all offensive enchantments. Randomized on purpose
    public static List<int> RandomOffensiveEnchantments = new List<int>()
    { 6, 8, 17, 7, 29, 27, 10, 24, 16, 25, 9, 26, 4, 19, 22, 20, 18, 23, 28, 0, 2 };
    //Same for defensive enchantments
    public static List<int> RandomDefensiveEnchantments = new List<int>()
    { 1, 4, 11, 21, 15, 5, 14, 12, 3 };
    //And a combined list, to be used by jewelry
    public static List<int> RandomEnchantments = new List<int>()
    { 6, 12, 23, 0, 22, 26, 5, 17, 10, 20, 11, 15, 2, 3, 14, 29, 8, 7, 13, 27, 4, 28, 21, 16, 1, 19, 25, 18, 24, 9 };


    //Basic info
    public int ID;
    public string Name;
    public Type EnchantmentType;
    public int Quality;
    public Trigger EnchantmentTrigger;
    public AttackBuff Buff;     //Only used for "BUFF" enchantment types
    public AttackEffect Effect; //Only used for "EFFECT" enchantment types
    public bool Offensive;      //True if offensive in nature, false if defensive

    //These will be set automatically by the enchantment type and level
    public float Chance     { get; private set; }
    public float Duration   { get; private set; }
    public float Potency    { get; private set; }

    public void Start()
    {
        CalculateStats();
    }

    public string GetDescription()
    {
        StringBuilder sb = new StringBuilder();
        CalculateStats();

        //First, show the title
        sb.AppendFormat("{0,-18}\n", Name);
        
        //On trigger, X% chance to
        switch(EnchantmentTrigger)
        {
            case Trigger.ON_GET_HIT:
                sb.Append("  On getting hit, ");
                sb.Append((int)(Chance * 100f) + "%\n");
                sb.Append("  chance to ");
                break;
            case Trigger.ON_HIT:
                sb.Append("  On hitting enemy, ");
                sb.Append((int)(Chance * 100f) + "%\n");
                sb.Append("  chance to ");
                break;
            case Trigger.ON_KILL:
                sb.Append("  On killing enemy, ");
                sb.Append((int)(Chance * 100f) + "%\n");
                sb.Append("  chance to ");
                break;
            case Trigger.PASSIVE:
                sb.Append("  Passive\n");
                break;
        }
        //Name of buff or effect
        string buffType = null;
        string effectType = null;
        switch (Buff)
        {
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
            case AttackEffect.SAVING_THROW:
                effectType = "  Survive lethal damage\n  Once per battle";
                break;
        }
        //Buff or effect description
        switch (EnchantmentType)
        {
            case Type.BUFF_SELF:
                if (EnchantmentTrigger == Trigger.PASSIVE)
                {
                    sb.Append("  Increase " + buffType.Replace(" ","\n  "));
                    sb.Append(" by " + ((int)(Potency * 100f)) + "%");
                }
                else
                { 
                    sb.Append("increase\n");
                    sb.Append("  " + buffType + " by\n");
                    sb.Append("  " + ((int)(Potency * 100f)) + "%");
                    sb.Append(" for " + Duration + " sec");
                }
                break;
            case Type.BUFF_TEAM:
                sb.Append("raise team\n");
                sb.Append("  " + buffType + " by\n");
                sb.Append("  " + ((int)(Potency * 100f)) + "%");
                sb.Append(" for " + Duration + " sec");
                break;
            case Type.DEBUFF_ENEMY:
                sb.Append("lower enemy\n");
                sb.Append("  " + buffType + " by\n");
                sb.Append("  " + ((int)(Potency * -100f)) + "%");
                sb.Append(" for " + Duration + " sec");
                break;
            case Type.EFFECT_ENEMY:
                sb.Append(effectType + "\n");
                sb.Append("  enemy ");
                if (Effect == AttackEffect.POISON)
                {
                    sb.Append(((int)Potency) + " points\n  ");
                }
                sb.Append("for " + Duration + " sec");
                break;
            //Currently only Saving Throw
            case Type.EFFECT_SELF:
                sb.Append(effectType);
                break;
            case Type.EFFECT_TEAM:
                sb.Append(effectType + " team\n");
                sb.Append(" ");
                if (Effect == AttackEffect.POISON)
                {
                    sb.Append(" " + ((int)Potency) + " points");
                }
                sb.Append(" for " + Duration + " sec");
                break;
        }

        return sb.ToString();
    }

    //Calculate chance, duration, and potency
    private void CalculateStats()
    {
        CalculateChance();
        CalculateDuration();
        CalculatePotency();
    }

    //Calculate chance
    private void CalculateChance()
    {
        Chance = ChanceTable[(int)EnchantmentType, Quality];
    }

    //Calculate duration
    private void CalculateDuration()
    {
        Duration = DurationTable[(int)EnchantmentType, Quality];
    }

    //Calculate potency
    private void CalculatePotency()
    {
        Potency = PotencyTable[(int)EnchantmentType, Quality];
    }
}
