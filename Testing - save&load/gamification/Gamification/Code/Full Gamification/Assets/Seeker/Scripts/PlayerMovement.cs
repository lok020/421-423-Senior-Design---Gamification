using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public bool force_move = false;

    public float move_speed;

    private Animator animate;
    private Rigidbody2D my_rigid_body;

    private bool moving;

    private Vector2 last_move;

    public bool can_move;

	// Use this for initialization
	void Start () {
        animate = GetComponent<Animator>();
        my_rigid_body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (force_move)
        {
            moving = true;
            if (my_rigid_body.velocity.x != 0)
            {
                last_move.x = my_rigid_body.velocity.x;
            }
            if (my_rigid_body.velocity.y != 0)
            {
                last_move.y = my_rigid_body.velocity.y;
            }

            animate.SetFloat("MoveX", my_rigid_body.velocity.x);
            animate.SetFloat("MoveY", my_rigid_body.velocity.y);
        }

        if (!can_move)
        {
            moving = false;
            my_rigid_body.velocity = new Vector2(0f, 0f);
            animate.SetFloat("MoveX", 0f);
            animate.SetFloat("MoveY", 0f);
            animate.SetBool("PlayerMoving", moving);
            return;
        }

        else
        {
            moving = false;

            if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -.5f)
            {
                //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * move_speed * Time.deltaTime, 0f, 0f));
                my_rigid_body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * move_speed, my_rigid_body.velocity.y);
                moving = true;
                last_move = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            }
            if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -.5f)
            {
                //transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical") * move_speed * Time.deltaTime, 0f));
                my_rigid_body.velocity = new Vector2(my_rigid_body.velocity.x, Input.GetAxisRaw("Vertical") * move_speed);
                moving = true;
                last_move = new Vector2(0f, Input.GetAxisRaw("Vertical"));
            }

            if (Input.GetAxisRaw("Horizontal") < 0.5f && Input.GetAxisRaw("Horizontal") > -0.5f)
            {
                my_rigid_body.velocity = new Vector2(0f, my_rigid_body.velocity.y);
            }

            if (Input.GetAxisRaw("Vertical") < 0.5f && Input.GetAxisRaw("Vertical") > -0.5f)
            {
                my_rigid_body.velocity = new Vector2(my_rigid_body.velocity.x, 0f);
            }

            animate.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
            animate.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        }




        animate.SetBool("PlayerMoving", moving);
        animate.SetFloat("LastMoveX", last_move.x);
        animate.SetFloat("LastMoveY", last_move.y);
    }
}
