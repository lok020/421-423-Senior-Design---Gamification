using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IncrementalManager : MonoBehaviour {

    private update up = new update();
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void useStamina(int i)
    {
        if (player.Incre.stamina.cur >= i)
        {
            player.Incre.stamina.cur--;
        } 
        else
        {
            //update up = new update();
        }
    }
    public void gainExp(int i)
    {
        player.Incre.exp.cur += i;
    }
}


