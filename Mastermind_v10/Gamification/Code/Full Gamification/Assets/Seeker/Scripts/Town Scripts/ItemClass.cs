using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Serializable so it will show up in the inspector.
[Serializable]
public class ItemClass
{
    public int id;
    public int value;
    public string name;
    public bool usable;
    public Sprite image;
    public string description;
    public bool discovered;
    

    //Constructor to set the values.
    public ItemClass(int a, int b, string c, bool d, Sprite e, string f, bool g)
    {
        id = a;
        value = b;
        name = c;
        usable = d;
        image = e;
        description = f;
        discovered = g;
    }

}
