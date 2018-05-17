using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Popup : MonoBehaviour {
    public float time=100;
    public Text text;
    public float moveX, moveY;
    public Outline outline;
    bool local = true;
    public bool showing = true;

    public void Set(string Message, float Time, Color Color, int Size, Vector3 position, bool local, float movex = 0 , float movey=0)
    {
        //Debug.Log(position);
        if (local)
        {
            text.rectTransform.localPosition = position;
            local = true;
        }
        else
        {
            text.rectTransform.position = position;
            local = false;
        }
        showing = true;
        text.rectTransform.localScale = new Vector3(1, 1, 1);
        text.horizontalOverflow = HorizontalWrapMode.Overflow; 
        //Debug.Log(text.rectTransform.position);
        text.text = Message;
        text.fontSize = Size;
        text.color = Color;
        text.supportRichText = true;
        time = Time;
        moveX = movex;
        moveY = movey;
        text.alignment = TextAnchor.MiddleCenter;
    }

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
        if (!showing)
        {
            Destroy(gameObject);
        }
        else if (time > 0)
        {
            text.enabled = true;
            if (local)
            {
                text.rectTransform.localPosition += new Vector3(moveX, moveY, 0);
            }
            else
            {
                text.rectTransform.position += new Vector3(moveX, moveY, 0);
            }
            text.rectTransform.position += new Vector3(moveX, moveY, 0);
            time -= Time.deltaTime;
            text.gameObject.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
