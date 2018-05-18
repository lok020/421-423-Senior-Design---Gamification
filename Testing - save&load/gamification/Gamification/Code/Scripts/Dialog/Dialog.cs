using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog {

    public double RequiredQuestID;
    public double RequiredObjectiveID;
    public double DontRunAfterQuestID;
    public double DontRunAfterObjectiveID;
    public bool IsRandom;
    public List<string> Strings;
    public List<DialogAction> Actions;

    private bool _completed = false;
    private int _index = 0;
    
    
    public string GetNext(EventManager eventManager, Inventory inventory)
    {
        string str = "";
        if(IsRandom)
        {
            System.Random rnd = new System.Random();
            str = Strings[rnd.Next(0, Strings.Count)];
        }
        else
        {
            if(_index >= Strings.Count)
            {
                _index = 0;
            }
            str = Strings[_index++];
        }
        if (!_completed)
        {
            foreach (DialogAction a in Actions)
            {
                switch (a.Action)
                {
                    case DialogAction.DialogActionEnum.ADD_ITEM:
                        inventory.AddItemToInventory(a.Item.GetComponent<Item>(), (int)a.Number);
                        eventManager.Event(new List<object>() { GameAction.NPC_RECEIVE, a.Item.GetComponent<Item>().ID, (int)a.Number });
                        break;
                    case DialogAction.DialogActionEnum.REMOVE_ITEM:
                        inventory.RemoveItemFromInventory(a.Item.GetComponent<Item>().ID, (int)a.Number);
                        eventManager.Event(new List<object>() { GameAction.NPC_GIVE, a.Item.GetComponent<Item>().ID, (int)a.Number });
                        break;
                    case DialogAction.DialogActionEnum.START_QUEST:
                        eventManager.StartQuest(a.Number);
                        break;
                    case DialogAction.DialogActionEnum.ADD_GOLD:
                        inventory.AddGold((int)a.Number);
                        break;
                }
            }
            _completed = true;
        }
        return str;
    }
}
