using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jiggle : MonoBehaviour {

    public float shake_amount = 0.0f;
    public float shake_time = 0.0f;
    private float reset;
    private Vector3 reset_position;

	// Use this for initialization
	void Start () {
        InvokeRepeating("ShakeCamera", 0.0f, 10.0f);
        reset = shake_time;
        reset_position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (shake_time >= 0)
        {
            GetComponent<AudioSource>().enabled = true;
            float shake_pos = Random.Range(-shake_amount, shake_amount);

            if (transform.position.y > .2 || transform.position.y < -.2)
            {
                transform.position = reset_position;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + shake_pos, transform.position.z);
            }

            shake_time -= Time.deltaTime;
        }
        else
        {
            GetComponent<AudioSource>().enabled = false;
            transform.position = reset_position;
        }
    }

    void ShakeCamera()
    {
        shake_time = reset;
    }
}
