using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jiggle : MonoBehaviour {

    public bool shake_once = false;
    public float shake_dist = 0.0f;
    public float shake_amount = 0.0f;
    public float shake_time = 0.0f;
    private float reset;
    private Vector3 reset_position;

	// Use this for initialization
	void Start () {
        if (shake_once)
        {
            GetComponent<CameraControl>().pause = true;
            reset_position = transform.position;
        }
        else
        {
            InvokeRepeating("ShakeCamera", 0.0f, 10.0f);
            reset = shake_time;
            reset_position = transform.position;
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (shake_once)
        {

            if (shake_time >= 0)
            {
                GetComponent<AudioSource>().enabled = true;
                float shake_pos = Random.Range(-shake_amount, shake_amount);
                int shake_dir = Random.Range(0, 3);

                if ((transform.position.y > reset_position.y + shake_dist || transform.position.y < reset_position.y - shake_dist) || (transform.position.x > reset_position.x + shake_dist || transform.position.x < reset_position.x - shake_dist))
                {
                    transform.position = reset_position;
                }

                switch (shake_dir)
                {
                    case 0:
                        transform.position = new Vector3(transform.position.x, transform.position.y + shake_pos, transform.position.z);
                        break;
                    case 1:
                        transform.position = new Vector3(transform.position.x + shake_pos, transform.position.y, transform.position.z);
                        break;
                    case 2:
                        transform.position = new Vector3(transform.position.x + shake_pos, transform.position.y + shake_pos, transform.position.z);
                        break;
                    default:
                        break;
                }

                shake_time -= Time.deltaTime;
            }
            else
            {
                transform.position = reset_position;
                GetComponent<CameraControl>().pause = false;
                GetComponent<Jiggle>().enabled = false;
            }
        }

        else
        {
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

    }

    void ShakeCamera()
    {
        shake_time = reset;
    }
}
