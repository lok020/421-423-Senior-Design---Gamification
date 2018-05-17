using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour {

    public GameObject target;
    public Vector2 set_distance;
    public float follow_distance;

    private int x_dir = 0;
    private int y_dir = 0;

    public float move_speed;

    private Vector3 path;

    private Vector3 positioning;

    private Animator animate;
    private Rigidbody2D my_rigid_body;

    private bool moving;

    private Vector2 last_move;
    

    // Use this for initialization
    void Start()
    {
        animate = GetComponent<Animator>();
        my_rigid_body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moving = false;
        
        if (Vector3.Distance(transform.position,path) > 0.3)
        {
            moving = true;


            if (transform.position.x - path.x > 0.1)
            {
                x_dir = -1;
            }
            else if (transform.position.x - path.x < -0.1)
            {
                x_dir = 1;
            }
            else
            {
                x_dir = 0;
            }

            if (transform.position.y - path.y > 0.1)
            {
                y_dir = -1;
            }
            else if (transform.position.y - path.y < -0.1)
            {
                y_dir = 1;
            }
            else
            {
                y_dir = 0;
            }

            last_move = new Vector2(x_dir, y_dir);

            transform.Translate(new Vector3(x_dir * move_speed * Time.deltaTime, y_dir * move_speed * Time.deltaTime, 0f));
        }

        if (Vector3.Distance(target.transform.position, positioning) >= 0.2)
        {
            if (Vector3.Distance(positioning, target.transform.position) > follow_distance)
            {
                path = positioning;
                positioning = target.transform.position;
            }
        }

        
        animate.SetFloat("MoveX", x_dir);
        animate.SetFloat("MoveY", y_dir);
        animate.SetBool("IsMoving", moving);
        animate.SetFloat("LastMoveX", last_move.x);
        animate.SetFloat("LastMoveY", last_move.y);
    }

    public void SetTarget()
    {
        positioning = target.transform.position;
        path = new Vector3(target.transform.position.x + set_distance.x, target.transform.position.y + set_distance.y, target.transform.position.z);
    }
}
