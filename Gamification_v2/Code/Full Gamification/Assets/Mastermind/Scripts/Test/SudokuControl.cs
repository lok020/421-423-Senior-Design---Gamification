using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SudokuControl : MonoBehaviour {


    private int[,] solution = new int[9,9];
    public SquareControl[] all_number_squares;
    public GameObject selected_square;

	// Use this for initialization
	void Start () {

        // This simply makes the solution 1-9 for each row
        /*for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                solution[x, y] = y + 1;
            }
        }*/

        //Board Initializer
        InitializeBoard();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void InitializeBoard()
    {
        // Input the FULL solution starting from top row.
        /*for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                solution[x,y] = //Whatever it should be.
            }
        }
        
        int z = 0;
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (//If this is a GIVEN number)
                {
                    all_number_squares[z].PenValueChange(//Whatever the given number is);
                    all_number_squares[z].can_change = false;
                }
            }
        }*/
    }

    public void CheckBoard()
    {
        bool solved = true;
        int z = 0;
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (all_number_squares[z].pen_num != solution[x,y])
                {
                    all_number_squares[z].gameObject.GetComponent<Image>().color = new Color(255, 0, 0);
                    solved = false;
                }
                z++;
            }
        }
        if (solved)
        {
            Debug.Log("Solved!");
        }
        else
        {
            Debug.Log("Not Solved");
        }
    }

    public void SelectSquare(GameObject ss)
    {
        if (selected_square != null)
        {
            selected_square.GetComponent<Image>().color = new Color(255, 255, 255);
        }
        selected_square = ss;
        selected_square.GetComponent<Image>().color = new Color(0, 255, 255);
    }

    public void PenChange(int num)
    {
        if (selected_square != null)
        {
            selected_square.GetComponent<SquareControl>().PenValueChange(num);
        }
    }

    public void PencilChange(int num)
    {
        if (selected_square != null)
        {

            selected_square.GetComponent<SquareControl>().PencilValueChange(num);
        }
    }
}
