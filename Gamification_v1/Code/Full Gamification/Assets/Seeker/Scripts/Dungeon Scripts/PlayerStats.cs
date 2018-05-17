using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public int max_health;
    public int current_health;

    public int max_stamina;
    public int current_stamina;

    public int insight;

    public int dexterity;

    public bool autodisable;

    // Use this for initialization
    void Start()
    {
        max_health = GlobalControl.Instance.hp;

        max_stamina = GlobalControl.Instance.stam;

        insight = GlobalControl.Instance.ins;

        dexterity = GlobalControl.Instance.dex;

        current_health = max_health;
        current_stamina = max_stamina;
    }

    // Update is called once per frame
    void Update()
    {
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

        GlobalControl.Instance.stam = max_stamina;

        GlobalControl.Instance.ins = insight;

        GlobalControl.Instance.dex = dexterity;
    }
}
