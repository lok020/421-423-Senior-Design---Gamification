using UnityEngine;
using System.Collections;

public class CircularInventoryTestMovement : MonoBehaviour {
    int collisionnumber = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D()
    {
        Debug.Log("Changing Direction");
        switch (collisionnumber)
        {
            case 0:
                GetComponent<PlayerController>().MovementVector= new Vector3(0, 1, 1);
                collisionnumber++;
                break;
            case 1:
                GetComponent<PlayerController>().MovementVector = new Vector3(-1, 0, 1);
                collisionnumber++;
                break;
            case 2:
                GetComponent<PlayerController>().MovementVector = new Vector3(0, -1, 1);
                collisionnumber++;
                break;
            default:
                collisionnumber++;
                break;
        }
    }
}
