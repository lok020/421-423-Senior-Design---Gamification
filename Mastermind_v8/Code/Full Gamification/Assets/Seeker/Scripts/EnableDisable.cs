using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour {

    public float delay = 0;

    public GameObject[] TurnOn;
    public GameObject[] TurnOff;

	void Start()
    {
        Activate();
        //Invoke("Activate", delay);
    }

    void Activate()
    {
        int x = 0;

        if (TurnOn.Length > 0)
        {
            for (x = 0; x < TurnOn.Length; x++)
            {
                TurnOn[x].SetActive(true);
            }
        }

        
        if (TurnOff.Length > 0)
        {
            for (x = 0; x < TurnOff.Length; x++)
            {
                TurnOff[x].SetActive(false);
            }

        }
    }
}
