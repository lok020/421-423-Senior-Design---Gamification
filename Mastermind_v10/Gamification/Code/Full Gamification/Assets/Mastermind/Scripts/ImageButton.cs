using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageButton : MonoBehaviour {

    public Image imageGo;

    //public Sprite[] button_image;
    //public Sprite unmute;
    //public Sprite mute;

	// Use this for initialization
	void Start () {
        //gameObject.GetComponent<Image>().sprite = unmute;
	}
	
    public void changeimage()
    {
        //gameObject.GetComponent<Image>().sprite = mute;

        imageGo.sprite = Resources.Load<Sprite>("mute");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
