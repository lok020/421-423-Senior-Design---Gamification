using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour {
    
    public GameObject fade_object;
    public float fade_speed = 0.0f;
    
    private Color alphat;
    public int fade_direction = -1; //The direction to fade: in = -1, out = 1

    public bool start_immedietly;
    public bool do_flash;

    void Start()
    {
        if (fade_object != null)
        {
            fade_object.GetComponent<Image>().enabled = true;
            alphat = fade_object.GetComponent<Image>().color;
            if (fade_direction == 1)
            {
                alphat.a = 0;
                fade_object.GetComponent<Image>().color = alphat;
            }
            else if (fade_direction == -1)
            {
                alphat.a = 1;
                fade_object.GetComponent<Image>().color = alphat;
            }
        }

        if (!start_immedietly)
        {
            fade_direction = 0;
        }
    }

    void Update()
    {
        if (fade_direction == 0 && !do_flash)
        {
            return;
        }
        else
        {
            fade_object.GetComponent<Image>().enabled = true;
        }

        if (do_flash)
        {
            if (alphat.a >= 1.0f)
            {
                fade_direction = -1;
            }

            alphat.a += fade_direction * fade_speed * Time.deltaTime;
            alphat.a = alphat.a = Mathf.Clamp01(alphat.a);

            fade_object.GetComponent<Image>().color = alphat;

            if (do_flash == true && fade_direction == -1 && alphat.a <= 0.0f)
            {
                do_flash = false;
            }
        }
        else
        {
            if (alphat.a <= 1.0f && alphat.a >= 0.0f)
            {
                alphat.a += fade_direction * fade_speed * Time.deltaTime;
                alphat.a = Mathf.Clamp01(alphat.a);

                fade_object.GetComponent<Image>().color = alphat;
                if (alphat.a == 0.0 && (fade_direction == 1 || fade_direction == -1))
                {
                    fade_direction = 0;
                    fade_object.GetComponent<Image>().enabled = false;
                }
            }
        }


    }


    public void BeginFade (int direction)
    {
        fade_object.GetComponent<Image>().enabled = true;
        fade_direction = direction;
    }

    public void DoFlashFade()
    {
        do_flash = true;
        fade_direction = 1;
    }

}
