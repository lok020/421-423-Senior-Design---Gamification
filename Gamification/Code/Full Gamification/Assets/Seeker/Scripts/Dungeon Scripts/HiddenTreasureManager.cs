using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HiddenTreasureManager : MonoBehaviour {

    public Sprite opened;
    private bool received = false;

    void OnTriggerEnter2D(Collider2D other)         // Opens loot interface when sees chest
    {
        if (other.gameObject.name == "Player")
        {
            this.GetComponent<SpriteRenderer>().sprite = opened;    // Changes looks of chest to signify it's been looked in

            if (!received)
            {
                // Give player active coins 20 * dungeon level.
                player.Incre.coin.active += GlobalControl.Instance.dungeon.level * 20 * bal.getActiveCoinBonus();
                received = true;
            }
        }
    }
}