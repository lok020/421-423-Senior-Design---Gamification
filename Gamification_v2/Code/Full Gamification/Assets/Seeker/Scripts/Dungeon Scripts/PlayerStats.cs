using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public int max_health;
    public int current_health;

    public int max_stamina;
    public int current_stamina;

    public int add_insight;         //From potions
    public int insight;

    public int add_dexterity;       //From potions
    public int dexterity;

    public bool autodisable;          //Automatically attempts to disable traps when walking on them.
    public int armor;
    public int armor_durability;

    public bool guaranteed_dodge;
    public bool guaranteed_disable;

    // Use this for initialization
    void Start()
    {
        max_health = GlobalControl.Instance.hp;

        max_stamina = GlobalControl.Instance.stam;

        insight = GlobalControl.Instance.ins;

        dexterity = GlobalControl.Instance.dex;

        current_health = max_health;
        current_stamina = max_stamina;

        add_insight = 0;
        add_dexterity = 0;
        armor = 0;              //How much percentage of damage is reduced.
        armor_durability = 0;   //How long the armor lasts
    }

    // Update is called once per frame
    void Update()
    {
        if (current_stamina < 0)
        {
            current_stamina = 0;
        }
        if (armor_durability <= 0)
        {
            armor = 0;
        }
    }

    public void Hurt(int damage)
    {
        int end_damage = Mathf.RoundToInt(damage * (float)(armor / 100));

        if (end_damage == damage)
        {
            end_damage = damage-1;
        }
        current_health -= damage;
    }

    public void Tired(int fatigue)
    {
        current_stamina -= fatigue;
    }

    public void ResetHealth()
    {
        current_health = max_health;
    }

    public void ResetStamina()
    {
        current_stamina = max_stamina;
    }

    public void HealHealth(float percentage)
    {
        if (current_health > max_health)
        {
            return;
        }

        float heal = max_health * (percentage/100);
        if (heal < 1)
        {
            heal = 1;
        }

        current_health += Mathf.RoundToInt(heal);
        if (current_health > max_health)
        {
            current_health = max_health;
        }
    }

    public void HealStamina(float percentage)
    {
        if (current_stamina > max_stamina)
        {
            return;
        }
        float heal = max_stamina * (percentage / 100);
        if (heal < 1)
        {
            heal = 1;
        }


        current_stamina += Mathf.RoundToInt(heal);
        if (current_stamina > max_stamina)
        {
            current_stamina = max_stamina;
        }
    }

    public void SetArmor(int added_armor, int added_durability)
    {
        armor = added_armor;
        armor_durability = added_durability;
    }

    public void SetAddInsight(float added_insight)
    {
        float add = insight * (added_insight / 100);
        add_insight += Mathf.RoundToInt(add);
    }

    public void SetAddDexterity(float added_dexterity)
    {
        float add = dexterity * (added_dexterity / 100);
        add_dexterity += Mathf.RoundToInt(add);
    }

    public int GetTotalInsight()
    {
        return insight + add_insight;
    }

    public int GetTotalDexterity()
    {
        return dexterity + add_dexterity;
    }

    public void SaladConsumed()
    {
        current_health += 2 * GetTotalDexterity();
        current_stamina += 2 * GetTotalInsight();
        add_dexterity = 0;
        add_insight = 0;
    }

    public void OverloadPotion()
    {
        current_health = (int)((float)max_health * 1.5);
        current_stamina = (int)((float)max_stamina * 1.5);
        SetAddDexterity(100);
        SetAddInsight(100);
    }

    public void SaveStats()
    {
        GlobalControl.Instance.hp = max_health;

        GlobalControl.Instance.stam = max_stamina;

        GlobalControl.Instance.ins = insight;

        GlobalControl.Instance.dex = dexterity;
    }

    public void ItemUsed(int id)
    {
        switch (id)
        {
            case 9:
                HealHealth(5);
                break;
            case 10:
                HealStamina(5);
                break;
            case 11:
                HealStamina(7.5f);
                break;
            case 12:
                HealHealth(7.5f);
                break;
            case 13:
                HealHealth(15.0f);
                break;
            case 14:
                HealStamina(15.0f);
                break;
            case 15:
                HealStamina(25.0f);
                break;
            case 16:
                HealHealth(25.0f);
                break;
            case 17:
                HealHealth(30.0f);
                HealStamina(30.0f);
                break;
            case 37:
                SetArmor(10, 10);
                break;
            case 38:
                SetArmor(20, 25);
                break;
            case 39:
                SetArmor(30, 40);
                break;
            case 40:
                SetArmor(50, 75);
                break;
            case 41:
                SetArmor(75, 100);
                break;
            case 42:
                HealHealth(20);
                break;
            case 43:
                HealStamina(20);
                break;
            case 44:
                HealHealth(40);
                break;
            case 45:
                HealStamina(40);
                break;
            case 46:
                HealHealth(60);
                break;
            case 47:
                HealStamina(60);
                break;
            case 48:
                HealHealth(100);
                HealStamina(100);
                break;
            case 49:
                SetAddDexterity(20);
                break;
            case 50:
                SetAddInsight(20);
                break;
            case 51:
                SetAddDexterity(20);
                break;
            case 52:
                SetAddInsight(40);
                break;
            case 53:
                SetAddDexterity(60);
                break;
            case 54:
                SetAddInsight(60);
                break;
            case 55:
                OverloadPotion();
                break;
            case 60:
                HealHealth(10);
                break;
            case 61:
                HealStamina(10);
                break;
            case 62:
                HealHealth(15);
                SetAddDexterity(5);
                break;
            case 63:
                HealStamina(15);
                SetAddInsight(5);
                break;
            case 64:
                SaladConsumed();
                break;
            case 65:
                HealHealth(60);
                HealStamina(60);
                SetAddDexterity(30);
                SetAddInsight(30);
                break;
            case 66:
                HealHealth(60);
                guaranteed_dodge = true;
                break;
            case 67:
                HealHealth(75);
                guaranteed_disable = true;
                break;
            case 73:
                HealHealth(30);
                break;
            case 74:
                HealStamina(30);
                break;
            case 75:
                HealHealth(40);
                SetAddDexterity(20);
                break;
            case 76:
                HealStamina(40);
                SetAddInsight(20);
                break;
            default:
                break;
        }
    }
}
