using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShowPanel : MonoBehaviour {

    public bool showGUI = false;

    public UnityEvent action_on;
    public UnityEvent action_off;

    private GameObject player;
    private GameObject inventory;

    void Awake ()
    {
        player = GameObject.Find("Player");
        inventory = GameObject.Find("InventoryDungeon");
    }


	void OnGUI()
    {
        if (showGUI)
        {
            player.GetComponent<PlayerMovement>().can_move = false;
            GUI.Box(new Rect(Screen.width * 0.75f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.2f), "Do you want to exit?");
            if (GUI.Button (new Rect (Screen.width * 0.76f, Screen.height * 0.125f, Screen.width * 0.075f, Screen.height * 0.075f), "Yes"))
            {
                inventory.GetComponent<InventoryManager>().SaveInventory();
                action_on.Invoke();
                showGUI = false;
            }
            if (GUI.Button(new Rect(Screen.width * 0.865f, Screen.height * 0.125f, Screen.width * 0.075f, Screen.height * 0.075f), "No"))
            {
                action_off.Invoke();
                player.GetComponent<PlayerMovement>().can_move = true;
                showGUI = false;
            }
        }
    }

    public void TurnOnGUI()
    {
        showGUI = true;
    }
}
