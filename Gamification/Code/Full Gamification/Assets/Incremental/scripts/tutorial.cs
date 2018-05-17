using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class tutorial : MonoBehaviour
{
    public List<GameObject> tutorials;
    private int index;
    private int prevIndex;
    // Use this for initialization
    void Start ()
    {
        index = 0;
        prevIndex = 0;
        //player.Incre.needTutorial = false;
        foreach(var tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }
        if(player.Incre.needTutorial)
        {
            tutorials[index].SetActive(true);
            prevIndex = index;
            player.Incre.needTutorial = false;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void nextTutorial()
    {
        index++;
        if(index < tutorials.Count)
        {
            tutorials[prevIndex].SetActive(false);
            tutorials[index].SetActive(true);
            prevIndex = index;
        }
        else
        {
            tutorials[prevIndex].SetActive(false);   //end tutorial
        }
    }
}
