using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {

    public int store_type; //1 = forge, 2 = herb, 3 = bakery
    public GameObject[] stock1;
    public GameObject[] stock2;
    public GameObject[] stock3;
    public GameObject[] stock4;

	// Use this for initialization
	void Start () {
        GameObject[][] stockholder = { stock1, stock2, stock3, stock4 };
        foreach (var i in stockholder)
        {
            foreach (var j in i)
            {
                j.SetActive(false);
            }
        }

		switch (store_type)
        {
            case 1:
                for (int x = 0; x < GlobalControl.Instance.forge_level; x++)
                {
                    for (int y = 0; y < stockholder[x].Length; y++)
                    {
                        stockholder[x][y].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int x = 0; x < GlobalControl.Instance.herb_level; x++)
                {
                    for (int y = 0; y < stockholder[x].Length; y++)
                    {
                        stockholder[x][y].SetActive(true);
                    }
                }
                break;
            case 3:
                for (int x = 0; x < GlobalControl.Instance.bakery_level; x++)
                {
                    for (int y = 0; y < stockholder[x].Length; y++)
                    {
                        stockholder[x][y].SetActive(true);
                    }
                }
                break;
            default:
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
