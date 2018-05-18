using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceMovement : MonoBehaviour {

    public bool moving;
    public Vector2 move_dir;
    public GameObject player;
    public float move_timer;
    private float move_counter;

    // Use this for initialization
    void Start () {
        move_counter = move_timer;
        moving = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            player.GetComponent<PlayerMovement>().force_move = true;
            move_counter -= Time.deltaTime;
            player.GetComponent<Rigidbody2D>().velocity = move_dir;

            if (move_counter <= 0f)
            {
                player.GetComponent<PlayerMovement>().force_move = false;
                moving = false;
                move_counter = move_timer;
            }
        }
        
	}

    public void StartMovement()
    {
        moving = true;
    }
}
