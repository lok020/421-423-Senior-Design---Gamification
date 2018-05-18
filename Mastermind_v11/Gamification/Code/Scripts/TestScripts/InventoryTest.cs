using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryTest : MonoBehaviour
{
    GameObject[] testitems;
    //bool finished = false;
    int numitems=0;
    //int numpassed = 0;
    int numtestitems = 4;

    // Use this for initialization
    void Start()
    {
        testitems = new GameObject[numtestitems];
    }

    // Update is called once per frame
    void Update()
    {
        if(numitems == numtestitems)
            test();
    }



    public void test()
    {
        var inventory= GetComponent<Inventory>();
        for (int i = 0; i < numtestitems; i++)
        {
            //if (testitems[i].GetComponent<ItemDetails>().name == GetComponent<Inventory>().inventory[i].name && testitems[i].GetComponent<ItemDetails>().id == GetComponent<Inventory>().inventory[i].id && testitems[i].GetComponent<ItemDetails>().type == GetComponent<Inventory>().inventory[i].type)
            if (inventory.ItemInInventory(i))
            {
                Debug.Log(i);
                Debug.Log("Inventory Test Passed");
            }
        }
        numitems = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        //Check if the tag of the trigger collided with is a pickup.
        if (other.tag == "Pickup")
        {
            Debug.Log("Adding to test array");
            testitems[numitems]=other.gameObject;
            numitems++;
        }
    }
}
