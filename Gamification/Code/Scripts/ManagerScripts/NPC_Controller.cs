using UnityEngine;
using System.Collections;
using System;

public class NPC_Controller : MonoBehaviour {

    public float MoveSpeed = 2.0f;
    public string Name;

    private int direction;       // 0, 1, 2, 3 = S, W, N, E respectively
    private float time;
    private Vector3 move;

    private Animator anim;
    private DialogController dialogController;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        dialogController = GetComponent<DialogController>();
	}
	
	// Move character
	void FixedUpdate () {
        //Move NPC
        if (time > 0)
        {
            float step = Time.deltaTime;
            transform.position += move * MoveSpeed * step;
            time -= step;
        }
    }

    void Update()
    {
        double x = 0;
        double y = 0;
        if (time > 0)
        {
            x = move.x;
            y = move.y;
        }
        //Debug.Log("x = " + x + ", y = " + y);
        //Get direction
        if (y > 0 && Math.Abs(y) >= Math.Abs(x))         //NW, N, or NE
        {
            direction = 2;  //North
        }
        else if (x < 0 && Math.Abs(x) >= Math.Abs(y))   //W or SW
        {
            direction = 1;  //West
        }
        else if (x > 0 && Math.Abs(x) >= Math.Abs(y))   //E or SE
        {
            direction = 3;  //East
        }
        else if (y < 0)                                 //S or idle
        {
            direction = 0;  //South
        }
        //Update animation
        if (direction < 0)
        {
            direction += 4;
            anim.enabled = true;
            anim.SetInteger("Direction", direction);
            anim.SetTime(0.01);
        }
        else if (x == 0 && y == 0)
        {
            anim.enabled = false;
        }
        else
        {
            anim.enabled = true;
            anim.SetInteger("Direction", direction);
        }
    }

    public void Move(float x, float y, float time)
    {
        this.time = time;
        move = new Vector3(x, y, 0);
        move.Normalize();
    }

    public void Face(int Direction)
    {
        direction = Direction - 4;  //Update will update the direction if it is set to a negative number
    }

    public void Speak(Inventory inventory)
    {
        dialogController.Speak(inventory);
    }

    public void Speak(string dialog, float time)
    {
        dialogController.Speak(dialog, time);
    }
}
