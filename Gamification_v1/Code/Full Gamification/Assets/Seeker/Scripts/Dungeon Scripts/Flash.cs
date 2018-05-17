using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Flash : MonoBehaviour {

    private GameObject player;

    public GameObject flash_object;
    public float flash_speed = 0.0f;
    
    private Color alphat;
    public int flash_direction = 1; //The direction to flash: in = -1, out = 1

    public bool start_immedietly;

    public UnityEvent on_flash;
    public UnityEvent after_flash;

    void Start()
    {
        player = GameObject.Find("Player");
        if (!start_immedietly)
        {
            flash_direction = 0;
        }

        if (flash_object != null)
        {
            flash_object.GetComponent<Image>().enabled = true;
            alphat = flash_object.GetComponent<Image>().color;
            if (flash_direction == 1)
            {
                alphat.a = 0;
                flash_object.GetComponent<Image>().color = alphat;
            }
        }
    }

    void Update()
    {
        if (flash_direction == 0 || (flash_direction == -1 && alphat.a <= 0.0f))
        {
            player.GetComponent<PlayerMovement>().can_move = true;
            return;
        }
        else
        {
            player.GetComponent<PlayerMovement>().can_move = false;
            flash_object.GetComponent<Image>().enabled = true;
        }

        if (alphat.a <= 1.0f && alphat.a >= 0.0f)
        {
            alphat.a += flash_direction * flash_speed * Time.deltaTime;
            alphat.a = Mathf.Clamp01(alphat.a);

            flash_object.GetComponent<Image>().color = alphat;
            if (alphat.a == 0.0 && (flash_direction == 1 || flash_direction == -1))
            {
                flash_direction = 0;
                after_flash.Invoke();
                flash_object.GetComponent<Image>().enabled = false;
            }
        }

        if (alphat.a >= 1.0f && flash_direction == 1)
        {
            if (on_flash != null)
            {
                on_flash.Invoke();
            }
            alphat.a = 1.0f;
            flash_direction = -1;
        }
    }


    public void Beginflash ()
    {
        flash_direction = 1;
    }
}
