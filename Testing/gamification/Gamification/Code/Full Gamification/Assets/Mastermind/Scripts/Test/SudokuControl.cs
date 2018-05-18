﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public enum difficult
{
    easy, medium, hard
}

public static class Difficulty
{
    public static difficult diff;
}

public class SudokuControl : MonoBehaviour
{
    private List<List<bool>> canChange;
    private List<List<int>> solution;
    public SquareControl[] all_number_squares;
    public SquareControl number_square;
    public GameObject selected_square;

    public GameObject complete;
    public Text complete_text;
    public Text timer;
    public Text checkTxt;
    public float second = 0;
    public int minute = 0;
    public int hour = 0;
    public int check;

	// Use this for initialization
	void Start ()
    {
        //game started
        check = 1;
        if (PlayerPrefs.GetInt("sudokuContinue", 0) == 1)
        {
            //load previous data
            canChange = new List<List<bool>>();
            solution = new List<List<int>>();
            for (int i = 0; i < 9; i++)
            {
                canChange.Add(PlayerPrefsX.GetBoolArray("SudokuCanChange" + i.ToString()).ToList());
                solution.Add(PlayerPrefsX.GetIntArray("SudokuSolution" + i.ToString()).ToList());
            }
            second = PlayerPrefs.GetFloat("sudokuSecond");
            check = PlayerPrefs.GetInt("sudokuCheck");
            loadUserInput();
        }
        else
        {
            PlayerPrefs.SetInt("sudokuContinue", 1);
            //make new one
            canChange = setCanChange(Difficulty.diff);
            solution = generateSudoku();
            //save to local
            for (int i = 0; i < 9; i++)
            {
                PlayerPrefsX.SetBoolArray("SudokuCanChange" + i.ToString(), canChange[i].ToArray());
                PlayerPrefsX.SetIntArray("SudokuSolution" + i.ToString(), solution[i].ToArray());
            }
        }
        //draw sudoku based on the information
        int squareIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                //Debug.LogAssertion(solution[i][j]);
                all_number_squares[squareIndex].can_change = canChange[i][j];
                //all_number_squares[squareIndex].setValue(solution[i][j]);
                if (!canChange[i][j])
                {
                    all_number_squares[squareIndex].gameObject.GetComponent<Image>().color = new Color32(214, 214, 214, 255);
                    all_number_squares[squareIndex].setValue(solution[i][j]);
                }
                squareIndex++;
            }
            Debug.Log("");
        }
        second += Time.deltaTime;
    }
    
    public void UpdateTimer()
    {
        second += Time.deltaTime;
        PlayerPrefs.SetFloat("sudokuSecond", second);
        timer.text = hour + "h: " + minute + "m: " + (int)second + "s";
        if (second >= 60)
        {
            minute++;
            second = 0;
        }
        else if (minute >= 60)
        {
            hour++;
            minute = 0;
        }
        
        
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateTimer();
        saveUserInput();
        checkTxt.text = check.ToString();
    }

    public void saveUserInput()
    {
        int squareIndex = 0;
        int[] playerInputs = new int[81];
        for(int i = 0; i< 9; i++)
        {
            for(int j = 0; j< 9; j++)
            {
                playerInputs[squareIndex] = all_number_squares[squareIndex].pen_num;
                squareIndex++;
            }
        }
        PlayerPrefsX.SetIntArray("sudokuPlayerInputs", playerInputs);
    }
    public void loadUserInput()
    {
        int squareIndex = 0;
        int[] playerInputs = PlayerPrefsX.GetIntArray("sudokuPlayerInputs");
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                all_number_squares[squareIndex].pen_num = playerInputs[squareIndex];
            }
        }
        
    }

    private List<List<bool>> setCanChange(difficult diff)
    {
        List<List<bool>> sudokuCanChange = new List<List<bool>>();
        for (int i = 0; i < 9; i++)
        {
            List<bool> r = new List<bool>();
            for (int j = 0; j < 9; j++)
            {
                r.Add(false);
            }
            sudokuCanChange.Add(r);
        }
        int count = 0;
        System.Random rand = new System.Random();
        if (diff == difficult.easy)
        {
            count = 25;
        }
        if (diff == difficult.medium)
        {
            count = 35;
        }
        if (diff == difficult.hard)
        {
            count = 50;
        }
        do
        {
            int rowIndex = rand.Next(0, 9);
            int colIndex = rand.Next(0, 9);
            if (sudokuCanChange[rowIndex][colIndex] == false)
            {
                sudokuCanChange[rowIndex][colIndex] = true;
                count--;
            }
        } while (count >= 0);
        return sudokuCanChange;
    }

    static List<List<int>> generateSudoku()
    {
        List<List<int>> Box = new List<List<int>>();
        List<List<int>> row = new List<List<int>>();
        List<List<int>> col = new List<List<int>>();
        int loopCounter = 20;
        for (int i = 0; i < 9; i++)
        {
            Box.Add(makeList());
            row.Add(makeList());
            col.Add(makeList());
        }
        List<List<int>> sudoku = new List<List<int>>();
        //current remain 
        List<int> BoxRemainNum;
        List<int> RowRemainNum;
        List<int> ColRemainNum;
        bool rollback = false;
        for (int r = 0; r < 9; r++)
        {
            //this goes through one line
            do
            {
                if (loopCounter <= 0)
                {
                    return generateSudoku();
                }
                rollback = false;
                List<int> newRow = new List<int>();
                //copy for rollbacks
                List<List<int>> RollbackBox = deepCopy(Box);
                List<List<int>> RollbackRow = deepCopy(row);
                List<List<int>> RollbackCol = deepCopy(col);
                for (int c = 0; c < 9; c++)
                {
                    ColRemainNum = col[c];
                    RowRemainNum = row[r];
                    BoxRemainNum = Box[whichBox(r, c)];
                    int num = pickNumber(ref BoxRemainNum, ref RowRemainNum, ref ColRemainNum, c);
                    if (num == -1)
                    {
                        rollback = true;
                        Box = RollbackBox.ToList();
                        row = RollbackRow.ToList();
                        col = RollbackCol.ToList();
                        loopCounter--;
                        break;
                    }
                    else
                    {
                        newRow.Add(num);
                    }
                }
                //Well generated
                if (!rollback)
                {
                    sudoku.Add(newRow);
                    break;
                }
            } while (rollback);
        }
        return sudoku;
    }
    static int pickNumber(ref List<int> box, ref List<int> row, ref List<int> col, int colIndex)
    {
        List<int> validList = new List<int>();
        for (int i = 1; i <= 9; i++)
        {
            if (row.Contains(i) && col.Contains(i) && box.Contains(i))
            {
                validList.Add(i);
            }
        }
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        if (validList.Count == 0)
        {

        }
        else
        {
            int pick = validList[rand.Next(0, validList.Count)];
            remove(ref box, pick);
            remove(ref row, pick);
            remove(ref col, pick);
            return pick;
        }
        return -1;
    }
    static void remove(ref List<int> list, int num)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == num)
            {
                list.RemoveAt(i);
                return;
            }
        }
    }
    static List<int> deepCopy(List<int> original)
    {
        List<int> newObject = new List<int>();
        foreach (int i in original)
        {
            newObject.Add(i);
        }
        return newObject;
    }
    static List<List<int>> deepCopy(List<List<int>> original)
    {
        List<List<int>> newObject = new List<List<int>>();
        foreach (var one in original)
        {
            newObject.Add(deepCopy(one));
        }
        return newObject;
    }
    static List<int> makeList()
    {
        List<int> list = new List<int>();
        list.Add(9); list.Add(2); list.Add(3); list.Add(4); list.Add(5); list.Add(6); list.Add(7); list.Add(8); list.Add(1);
        return list;
    }
    static int whichBox(int row, int column)
    {
        if (row >= 0 && row <= 2 && column >= 0 && column <= 2)
        {
            return 0;
        }
        if (row >= 0 && row <= 2 && column >= 3 && column <= 5)
        {
            return 1;
        }
        if (row >= 0 && row <= 2 && column >= 6 && column <= 8)
        {
            return 2;
        }
        if (row >= 3 && row <= 5 && column >= 0 && column <= 2)
        {
            return 3;
        }
        if (row >= 3 && row <= 5 && column >= 3 && column <= 5)
        {
            return 4;
        }
        if (row >= 3 && row <= 5 && column >= 6 && column <= 8)
        {
            return 5;
        }
        if (row >= 6 && row <= 8 && column >= 0 && column <= 2)
        {
            return 6;
        }
        if (row >= 6 && row <= 8 && column >= 3 && column <= 5)
        {
            return 7;
        }
        if (row >= 6 && row <= 8 && column >= 6 && column <= 8)
        {
            return 8;
        }
        return -1;
    }

    public void CheckBoard()
    {
        bool solved = true;
        //int check = 1;                  // counting the check button click
        int z = 0;
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (all_number_squares[z].pen_num != solution[x][y] && all_number_squares[z].can_change)
                {
                    all_number_squares[z].gameObject.GetComponent<Image>().color = new Color32(255, 81, 81, 255);
                    /*if(check > 5)
                    {
                        solved = true;
                    }
                    else solved = false;*/
                    solved = false;
                }
                z++;
            }
        }
        if (solved)
        {
            Debug.Log("Solved!");
            complete.SetActive(true);
            complete_text.text = "Successfully Solved\n" + "Time Used: " + hour + "h: " + minute + "m: " + (int)second + "s" + "\nTotal Check: " + check;

            PlayerPrefs.SetInt("sudokuCheck", check);
            PlayerPrefs.SetInt("sudokuContinue", 0);
        }
        else
        {
            Debug.Log("Not Solved");
            check++;
            PlayerPrefs.SetInt("sudokuCheck", check);
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