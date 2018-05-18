using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject description_display;
    public Text description_text;
    public string description;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        description_text.text = description;
        description_display.SetActive(true);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        description_display.SetActive(false);
    }

}
