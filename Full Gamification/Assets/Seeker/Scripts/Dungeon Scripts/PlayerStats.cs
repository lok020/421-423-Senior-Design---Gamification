using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public int max_health;
    public int current_health;
    public int health_next_level_exp;
    public int health_exp;

    public int max_stamina;
    public int current_stamina;
    public int stamina_next_level_exp;
    public int stamina_exp;

    public int insight;
    public int insight_next_level_exp;
    public int insight_exp;

    public int dexterity;
    public int dexterity_next_level_exp;
    public int dexterity_exp;

    public bool autodisable;

    // Use this for initialization
    void Start()
    {
        max_health = GlobalControl.Instance.hp;
        health_exp = GlobalControl.Instance.hpxp;
        health_next_level_exp = GlobalControl.Instance.hpnextxp;

        max_stamina = GlobalControl.Instance.stam;
        stamina_exp = GlobalControl.Instance.stamxp;
        stamina_next_level_exp = GlobalControl.Instance.stamnextxp;

        insight = GlobalControl.Instance.ins;
        insight_exp = GlobalControl.Instance.insxp;
        insight_next_level_exp = GlobalControl.Instance.insnextxp;

        dexterity = GlobalControl.Instance.dex;
        dexterity_exp = GlobalControl.Instance.dexxp;
        dexterity_next_level_exp = GlobalControl.Instance.dexnextxp;

        current_health = max_health;
        current_stamina = max_stamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (health_exp >= health_next_level_exp)
        {
            max_health += 10;
            current_health += 10;
            health_exp -= health_next_level_exp;
            //Need Health EXP rate function
        }

        if (stamina_exp >= stamina_next_level_exp)
        {
            max_stamina += 5;
            current_stamina += 5;
            stamina_exp -= stamina_next_level_exp;
            //Need Stamina EXP rate function
        }

        if (insight_exp >= insight_next_level_exp)
        {
            insight++;
            insight_exp -= insight_next_level_exp;
            //Need Insight EXP rate function
        }

        if (dexterity_exp >= dexterity_next_level_exp)
        {
            dexterity++;
            dexterity_exp -= dexterity_next_level_exp;
            //Need Dexterity EXP rate function
        }

        if (current_stamina < 0)
        {
            current_stamina = 0;
        }
    }

    public void Hurt(int damage)
    {
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


    public void SaveStats()
    {
        GlobalControl.Instance.hp = max_health;
        GlobalControl.Instance.hpxp = health_exp;
        GlobalControl.Instance.hpnextxp = health_next_level_exp;

        GlobalControl.Instance.stam = max_stamina;
        GlobalControl.Instance.stamxp = stamina_exp;
        GlobalControl.Instance.stamnextxp = stamina_next_level_exp;

        GlobalControl.Instance.ins = insight;
        GlobalControl.Instance.insxp = insight_exp;
        GlobalControl.Instance.insnextxp = insight_next_level_exp;

        GlobalControl.Instance.dex = dexterity;
        GlobalControl.Instance.dexxp = dexterity_exp;
        GlobalControl.Instance.dexnextxp = dexterity_next_level_exp;
    }
}
