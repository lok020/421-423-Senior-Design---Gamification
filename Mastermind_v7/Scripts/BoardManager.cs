using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    private Text selectedBox;
    private Button previousButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectBox(Text thisObject)
    {
        selectedBox = thisObject;
    }
    public void SelectButton(Button but)
    {
        if(previousButton != null)
        {
            ColorBlock cb2 = previousButton.colors;
            Color c2;
            c2 = new Color32(255, 255, 255, 255);
            cb2.normalColor = c2;
            cb2.highlightedColor = c2;
            previousButton.colors = cb2;
        }
        
        previousButton = but;
        ColorBlock cb = but.colors;
        Color c;
        c = new Color32(24, 159, 255, 255);
        cb.normalColor = c;
        cb.highlightedColor = c;
        but.colors = cb;

    }

    public void ChangeNumber(int num)
    {
        selectedBox.text = num.ToString();
    }
}
