using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueInstances : MonoBehaviour {

    public bool is_active;

    public string[] dialogue_lines;
    public GameObject text_box;
    public Text the_text;

    private int current_line;

	// Use this for initialization
	void Start () {
        current_line = 0;
        is_active = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!is_active)
        {
            return;
        }

        is_active = true;

        text_box.SetActive(true);
        the_text.text = dialogue_lines[current_line];

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            current_line++;
        }

        if (current_line >= dialogue_lines.Length)
        {
            text_box.SetActive(false);
            current_line = 0;
            is_active = false;
        }
    }

    public void SetInstance (string[] lines)
    {
        dialogue_lines = lines;
        current_line = 0;
        is_active = true;
    }

    public void SetActive (bool active)
    {
        is_active = active;
    }
}
