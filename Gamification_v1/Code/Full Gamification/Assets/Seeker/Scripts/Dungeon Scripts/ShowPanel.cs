using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShowPanel : MonoBehaviour {
    
    public UnityEvent action_on;
    public UnityEvent action_off;

    private GameObject player;
    private GameObject inventory;
    private GameObject choice;
    private bool move = true;

    void Awake ()
    {
        player = GameObject.Find("Player");
        inventory = GameObject.Find("InventoryDungeon");
        choice = GameObject.Find("Choice Panel");
    }

    void Update()
    {
        player.GetComponent<PlayerMovement>().can_move = move;
    }

	public void OpenPanel()
    {
        move = false;
        choice.SetActive(true);
        choice.GetComponent<PanelChoice>().SetObject(gameObject);
        choice.GetComponent<PanelChoice>().SetText("Leave this dungeon?");
    }

    public void Leave()
    {
        inventory.GetComponent<InventoryManager>().SaveInventory();
        action_on.Invoke();
    }

    public void Stay()
    {
        action_off.Invoke();
        move = true;
    }
}
