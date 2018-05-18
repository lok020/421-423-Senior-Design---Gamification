using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SquareControl : MonoBehaviour, IPointerClickHandler {

    public int pen_num;
    public List<int> pencil_num;

    public SudokuControl sudoku_main;

    public Text pen_box;
    public Text pencil_box;

    public bool can_change;

	// Use this for initialization
	void Start () {
        pen_num = 0;
        pencil_num = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && can_change)
        {
            sudoku_main.SelectSquare(gameObject);
        }
    }

    public void PenValueChange(int num)
    {
        if (can_change)
        {
            pen_num = num;
            pen_box.text = pen_num.ToString();
            pen_box.enabled = true;

            pencil_num.Clear();
            pencil_box.text = "";
            pencil_box.enabled = false;
        }
    }

    public void PencilValueChange(int num)
    {
        if (!can_change)
        {
            return;
        }

        pen_num = 0;
        pen_box.text = "";
        pen_box.enabled = false;

        if (pencil_num.Contains(num))
        {
            pencil_num.Remove(num);
        }
        else
        {
            pencil_num.Add(num);
        }
        pencil_box.text = "";

        if (pencil_num.Count == 0)
        {
            return;
        }

        pencil_num.Sort();

        //Sorts for text box
        for (int x = 0; x < pencil_num.Count - 1; x++)
        {
            if (x == 3 || x == 6)
            {
                pencil_box.text += "\n" + pencil_num[x] + " ";
            }
            else
            {
                pencil_box.text += pencil_num[x] + " ";
            }
        }
        if (pencil_num.Count - 1 == 3 || pencil_num.Count - 1 == 6)
        {
            pencil_box.text += "\n" + pencil_num[pencil_num.Count - 1];
        }
        else
        {
            pencil_box.text += pencil_num[pencil_num.Count - 1];
        }

        pencil_box.enabled = true;
    }
}
