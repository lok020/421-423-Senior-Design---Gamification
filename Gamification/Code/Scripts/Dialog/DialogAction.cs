using System;
using UnityEngine;

[Serializable]
public class DialogAction
{
    public enum DialogActionEnum
    {
        START_QUEST,
        ADD_ITEM,
        REMOVE_ITEM,
        ADD_GOLD
    };

    public DialogActionEnum Action;
    public double Number;
    public GameObject Item;
}