using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vines : MonoBehaviour {
    
    public GameObject player;
    public GameObject notification;
    public GameObject choice;
    private bool found;

    public int difficulty;
    public int damage;
    public float armor;
    public bool has_fruit;


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            choice.SetActive(true);
            if (!gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                notification.GetComponent<TextInfo>().AddText("The vines strike at you!");
                Dodge();
                choice.GetComponent<SecondDungeonPanel>().SetObject(gameObject);
                choice.GetComponent<SecondDungeonPanel>().SetText("Attempt to dig up vines?");
                found = true;
            }
            else
            {
                choice.GetComponent<SecondDungeonPanel>().SetObject(gameObject);
                choice.GetComponent<SecondDungeonPanel>().SetText("Attempt to dig up vines?");
            }
        }
    }

    private void Dodge()
    {
        float x = 0.0f;
        
        // Use player stamina. If player doesn't have 3 or more stamina, use all and apply it to dodge chance
        if (player.GetComponent<PlayerStats>().current_stamina <= 2)
        {
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().GetTotalDexterity()) * ((player.GetComponent<PlayerStats>().current_stamina) / 3);
            player.GetComponent<PlayerStats>().current_stamina = 0;
        }
        else
        {
            player.GetComponent<PlayerStats>().Tired(3);
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().GetTotalDexterity());
        }

        // Rolls to see if player dodges trap

        // If fails roll
        if (x + (player.GetComponent<PlayerStats>().GetTotalInsight() * .1) <= difficulty)
        {
            // If the player's dexterity roll was fast enough, take reduced damage
            if (x >= Mathf.CeilToInt(difficulty * 0.8f))
            {
                player.GetComponent<PlayerStats>().Hurt(Mathf.FloorToInt(damage * 0.5f * (1 - (armor/100))));
                notification.GetComponent<TextInfo>().AddText("You weren't fast enough to dodge the vines unscathed.");

            }

            // Take full damage
            else
            {
                player.GetComponent<PlayerStats>().Hurt(Mathf.CeilToInt((damage * (1 - (armor/100)))));
                notification.GetComponent<TextInfo>().AddText("The vines lash and lands a direct hit.");
            }
        }

        // Roll succeeds, take no damage
        else
        {
            notification.GetComponent<TextInfo>().AddText("You were able to dodge the vines!");
        }
    }

    private void DigUp()
    {
        float x = 0.0f;

        // If no stamina, reduce chance of success
        if (player.GetComponent<PlayerStats>().current_stamina <= 0)
        {
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().GetTotalInsight() * 0.2f);
        }

        else
        {
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().GetTotalInsight());

            player.GetComponent<PlayerStats>().Tired(1);
        }
        
        if (x + (player.GetComponent<PlayerStats>().GetTotalDexterity() * 0.1f) <= difficulty)
        {
            notification.GetComponent<TextInfo>().AddText("The vines thrash at you.");
            Dodge();
            notification.GetComponent<TextInfo>().AddText("You failed to dig up the vines.");
        }

        // If success, completely remove trap.
        else
        {
            gameObject.SetActive(false);
            notification.GetComponent<TextInfo>().AddText("You removed the vines!");
            if (has_fruit)
            {
                notification.GetComponent<TextInfo>().AddText("You found a vine fruit!");
            }
        }
    }

    public void AnswerYes()
    {
        DigUp();
        choice.SetActive(false);
    }

    public void AnswerNo()
    {
        if (!found)
        {
            return;
        }
        Dodge();
        choice.SetActive(false);
    }
}
