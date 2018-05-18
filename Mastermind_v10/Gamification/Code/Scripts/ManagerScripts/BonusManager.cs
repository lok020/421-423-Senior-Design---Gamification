using UnityEngine;
using System.Collections.Generic;
using System;

public class BonusManager {
    
    private PlayerStats _stats;
    private Inventory _inventory;
    private NetworkManager _networkManager;

    public BonusManager(PlayerStats playerStats, Inventory playerInventory, NetworkManager db)
    {
        _stats = playerStats;
        _inventory = playerInventory;
        _networkManager = db;
    }

    public void ProcessBonusCode(string code)
    {
        //Get bonus from code. If the function returns null, the code wasn't found
        List<string> rewards = _networkManager.BonusGetBonus(code);
        if (rewards == null) return;
        string bonusCodeDetails = null;
        
        switch(rewards[0])
        {
            //int id, int count
            case "ITEM":
                {
                    int id = int.Parse(rewards[1]);
                    int count = int.Parse(rewards[2]);
                    _inventory.QueueItemForAddition(id, count);
                    if (count == 1) bonusCodeDetails = "Item received!";
                    else bonusCodeDetails = "Items received!";
                    break;
                }
            //int count
            case "SKILLPOINTS":
                {
                    int count = int.Parse(rewards[1]);
                    _stats.AddSkillPoints(count);
                    bonusCodeDetails = count.ToString() + " skill points received!";
                    break;
                }
            //int count
            case "STATPOINTS":
                {
                    int count = int.Parse(rewards[1]);
                    _stats.AddStatPoints(count);
                    bonusCodeDetails = count.ToString() + " stat points received!";
                    break;
                }
        }
        //Save that code has been redeemed
        _networkManager.BonusCodeActivate(code);
        //Lastly, notify the player that the bonus code has been redeemed
        MessageOverlayController messageController = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
        messageController.EnqueueMessage("Bonus code redeemed!");
        messageController.EnqueueMessage(bonusCodeDetails);
    }
}
