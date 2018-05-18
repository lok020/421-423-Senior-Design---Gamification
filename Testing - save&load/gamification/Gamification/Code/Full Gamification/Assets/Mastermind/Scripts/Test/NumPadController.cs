using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumPadController : MonoBehaviour {

    private KeyCode[] number_codes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };
    private KeyCode[] keypad_number_codes = { KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9 };

    public SudokuControl sudoku_main;
    public bool pen_mode; //true = pen, false = pencil

    public Image pen_selector;
    public Image pencil_selector;

	// Use this for initialization
	void Start () {
        ModeChange(false);
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < number_codes.Length; i++)
        {
            if (Input.GetKeyDown(number_codes[i]) || Input.GetKeyDown(keypad_number_codes[i]))
            {
                if (pen_mode)
                {
                    ChangePenValue(i + 1);
                }
                else
                {
                    ChangePencilValue(i + 1);
                }
            }
        }
    }

    public void AcquiredNumber(int num)
    {
        if (pen_mode)
        {
            ChangePenValue(num);
        }
        else
        {
            ChangePencilValue(num);
        }
    }

    public void ChangePenValue(int num)
    {
        sudoku_main.PenChange(num);
    }

    public void ChangePencilValue(int num)
    {
        sudoku_main.PencilChange(num);
    }

    public void ModeChange(bool mode)
    {
        Color selected = new Color(0, 255, 0);
        Color deselected = new Color(255, 255, 255);
        pen_mode = mode;
        if (pen_mode)
        {
            pen_selector.color = selected;
            pencil_selector.color = deselected;
        }
        else
        {
            pen_selector.color = deselected;
            pencil_selector.color = selected;
        }
    }
}
