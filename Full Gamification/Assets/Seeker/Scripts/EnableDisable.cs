using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour {

    public GameObject[] OnEnabling;
    public GameObject[] OnDisabling;

	void Start()
    {
        int x = 0;

        for (x = 0; x < OnEnabling.Length; x++)
        {
            OnEnabling[x].SetActive(true);
        }

        for (x = 0; x < OnDisabling.Length; x++)
        {
            OnDisabling[x].SetActive(false);
        }
    }
}
