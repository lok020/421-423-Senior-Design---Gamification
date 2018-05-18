using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class tutorial : MonoBehaviour
{

    public List<GameObject> tutorials;
    private int index;
	// Use this for initialization
	void Start ()
    {
        index = 0;
        foreach(var tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }
        if(player.needTutorial)
        {
            tutorials[index].SetActive(true);
            player.needTutorial = false;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void nextTutorial()
    {
        if(index < tutorials.Count)
        {
            tutorials[index++].SetActive(false);
            tutorials[index].SetActive(true);
        }
        else
        {
            tutorials[index].SetActive(true);   //end tutorial
        }
    }
}
