using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrapManager : MonoBehaviour {

    public int level = 1;
    public int power = 2;
    public int speed = 2;
    public int disable = 5;
    public int experience = 10;
    public Sprite[] traps;

    private GameObject player;
    private GameObject notification;
    private int trap_type;

    private bool showGUI = false;

	// Use this for initialization
	void Awake () {
        player = GameObject.Find("Player");
        notification = GameObject.Find("Notifications");
	}
	
    // Initialize type of trap and strength
    void Start()
    {
        trap_type = Random.Range(0, 3);

        // 0 is Pitfall. Dodge: Hard
        // 1 is Spikes. Disable: Hard
        // 2 is Rubble. Damage: Hard
        switch (trap_type)
        {
            case 0:
                speed *= level + 1;
                this.GetComponent<SpriteRenderer>().sprite = traps[0];
                break;
            case 1:
                disable *= level + 1;
                this.GetComponent<SpriteRenderer>().sprite = traps[1];
                break;
            case 2:
                power *= level + 1;
                this.GetComponent<SpriteRenderer>().sprite = traps[2];
                break;

        }

        
    }

	// Update is called once per frame
	void Update () {
		if (disable <= 0)
        {
            gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            if (!gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                FirstEncounter();
            }
            else
            {
                showGUI = true;
            }
        }
    }


    // Dodging requires 3 stamina max
    private void Dodge()
    {
        float x = 0.0f;

        // Use player stamina. If player doesn't have 3 or more stamina, use all and apply it to dodge chance
        if (player.GetComponent<PlayerStats>().current_stamina <= 2)
        {
            notification.GetComponent<TextInfo>().AddText("You're too tired to put full strength into dodging.");
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().dexterity) * ((player.GetComponent<PlayerStats>().current_stamina)/3);
            player.GetComponent<PlayerStats>().current_stamina = 0;
        }
        else
        {
            player.GetComponent<PlayerStats>().Tired(3);
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().dexterity);
        }
        
        // Rolls to see if player dodges trap

        // If fails roll
        if (x + (player.GetComponent<PlayerStats>().insight * .1) <= speed)
        {
            // If the player's dexterity roll was fast enough, take reduced damage
            if (x >= Mathf.CeilToInt(speed * 0.75f))
            {
                player.GetComponent<PlayerStats>().Hurt(Mathf.FloorToInt(power * 0.5f));
                notification.GetComponent<TextInfo>().AddText("You weren't fast enough to escape completely unscathed.");

            }

            // Take full damage
            else
            {
                player.GetComponent<PlayerStats>().Hurt(power);
                notification.GetComponent<TextInfo>().AddText("You weren't fast enough to dodge the trap.");
            }
        }

        // Roll succeeds, take no damage
        else
        {
            player.GetComponent<PlayerStats>().dexterity_exp += Mathf.CeilToInt(experience * 0.1f);
            notification.GetComponent<TextInfo>().AddText("You were able to dodge the trap!");
        }
    }


    // Disabling traps require 1 stamina
    private void DisableTrap()
    {
        float x = 0.0f;

        // If no stamina, reduce chance of success
        if (player.GetComponent<PlayerStats>().current_stamina <= 0)
        {
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().insight * 0.2f);
            notification.GetComponent<TextInfo>().AddText("You sluggishly attempt to disable the trap");
        }

        else
        {
            x = Random.Range(0.0f, player.GetComponent<PlayerStats>().insight);

            player.GetComponent<PlayerStats>().Tired(1);
        }

        // If failed to disable trap completely, reduce the trap's durability
        if (x + (player.GetComponent<PlayerStats>().dexterity * 0.1f) <= disable)
        {
            player.GetComponent<PlayerStats>().stamina_exp += Mathf.CeilToInt(experience * 0.1f);
            notification.GetComponent<TextInfo>().AddText("You failed to completely disable the trap.");
            disable -= Mathf.FloorToInt(x);
        }

        // If success, completely remove trap.
        else
        {
            player.GetComponent<PlayerStats>().stamina_exp += experience;
            disable = 0;
            notification.GetComponent<TextInfo>().AddText("You've disabled the trap!");
        }

    }

    // Activate trap on first encounter of the trap.
    public void FirstEncounter ()
    {
        Dodge();

        // If autodisable trap option is on, automatically attempt to deactivate trap
        if (player.GetComponent<PlayerStats>().autodisable)
        {
            DisableTrap();
        }
        else
        {
            showGUI = true;
        }
    }


    // If the player runs into a trap they've already activated but didn't disable, ask if they wish to disable it again.
    // If they choose to attempt to disable the trap and fail, they must dodge again and can move past trap.
    // If they don't attempt to disable the trap, the player must dodge still but can move past the trap.
    void OnGUI()
    {
        if (showGUI)
        {
            player.GetComponent<PlayerMovement>().can_move = false;
            GUI.Box(new Rect(Screen.width * 0.7f, Screen.height * 0.05f, Screen.width * 0.25f, Screen.height * 0.25f), "Attempt to disable \ntrap?");
            if (GUI.Button(new Rect(Screen.width * 0.71f, Screen.height * 0.175f, Screen.width * 0.075f, Screen.height * 0.075f), "Yes"))
            {
                DisableTrap();
                if (disable > 0)
                {
                    if (Random.Range(0.0f, player.GetComponent<PlayerStats>().insight) * 0.4f < disable)
                    {
                        notification.GetComponent<TextInfo>().AddText("You've triggered the trap again!");
                        Dodge();
                    }
                    else
                    {
                        notification.GetComponent<TextInfo>().AddText("You managed to at least not trigger the trap again.");
                    }
                }
                player.GetComponent<PlayerMovement>().can_move = true;
                showGUI = false;
            }
            if (GUI.Button(new Rect(Screen.width * 0.865f, Screen.height * 0.175f, Screen.width * 0.075f, Screen.height * 0.075f), "No"))
            {
                Dodge();
                player.GetComponent<PlayerMovement>().can_move = true;
                showGUI = false;
            }
        }
    }
}
