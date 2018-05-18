using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Xml;

// There are three difficulty define for the sudoku
public enum difficult
{
    easy, medium, hard, fix_easy, fix_medium, fix_hard
}

// A variable to hold what difficulty it is going to be
public static class Difficulty
{
    public static difficult diff;
}

// Possible class to having global variable for Stat scene, or Setting scene
public class sudokuData
{
    public float second = 0;
    public int minute = 0;
    public int hour = 0;
    public int check = 1;
    // Variable for holding Easy level stats
    public int Easy_complete_count = 0;
    public float Easy_complete_sec = 0;

    // Variable for holding Medium level stats
    public int Medium_complete_count = 0;
    public float Medium_complete_sec = 0;

    // Variable for holding Hard level stats
    public int Hard_complete_count = 0;
    public float Hard_complete_sec = 0;

    // Variable for holding All level stats
    public int Total_complete_count = 0;
    public float Total_complete_sec = 0;

    // possible move this into global variable class. A variable to hold the second of completing the board
    public float only_second = 0;

    // Another possible variable to move to global variable class. A variable to hold the difficulty as an int
    public int difficulty_level = 0;
    public int sudokuContinue = 0;

    // There are two list, one holding the change-able square, and the other list is the solution for the whole board
    public List<List<bool>> canChange;
    public List<List<int>> solution;
    // This is the part of loading the user input for local save
    public string[] playerInputs;

    public bool fixedBoardIndexSetup = false;   //false = need to setup, true = no need to setup
    public List<int> fixedBoardIndex_easy = new List<int>();
    public List<int> fixedBoardIndex_medium = new List<int>();
    public List<int> fixedBoardIndex_hard = new List<int>();

    //current index of fixedBoardIndex
    public int fix_easy_index = 0;
    public int fix_medium_index = 0;
    public int fix_hard_index = 0;
}

public static class fixedSudoku
{
    // List for the fix board
    public static List<string> easy = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsEasyXMLprefab.xml",
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
    public static List<string> medium = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsMediumXMLprefab.xml",
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
    public static List<string> hard = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsHardXMLprefab.xml",
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
}

// The main control for the Sudoku flow
public class SudokuControl : MonoBehaviour
{
    
    //private List<List<bool>> canChange;
    //private List<List<int>> solution;

    // Here is the variable for the indiv. squares control
    public SquareControl[] all_number_squares;
    public SquareControl number_square;
    public GameObject selected_square;
    // Here is the variables for checking for the complete and the display text when it's solve
    public GameObject complete;
    public Text complete_text;
    public Text timer;
    public Text checkTxt;

    // When the board is save / the save button is press
    public Text savetxt;
    
    // Variable for holding Easy level stats
    public Text Easy_sec;
    public Text Easy_complete;
    // Variable for holding Medium level stats
    public Text Medium_sec;
    public Text Medium_complete;

    // Variable for holding Hard level stats
    public Text Hard_sec;
    public Text Hard_complete;

    // Variable for holding All level stats
    public Text Total_sec;
    public Text Total_complete;
    private NetworkManager network;
    
    // Use this for initialization
    void Start ()
    {
        /*
        fix_board_easy = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsEasyXMLprefab.xml", 
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
        fix_board_medium = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsMediumXMLprefab.xml",
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
        fix_board_hard = ReadXML.parseXmlFile(Application.dataPath + @"/Mastermind/Scripts/Assets/Resources/levelsHardXMLprefab.xml",
             "//ArrayOfLevel/Level/ArenaHorizontalLines/Line");
        */

        for(int i = 0; i< 3; i++)
        {
            Debug.Log(fixedSudoku.easy[i]+"======");
        }

        if(player.Sudoku.fixedBoardIndexSetup == false)
        {
            for (int i = 0; i < fixedSudoku.easy.Count; i++)
                player.Sudoku.fixedBoardIndex_easy.Add(i);

            for (int i = 0; i < fixedSudoku.medium.Count; i++)
                player.Sudoku.fixedBoardIndex_medium.Add(i);

            for (int i = 0; i < fixedSudoku.hard.Count; i++)
                player.Sudoku.fixedBoardIndex_hard.Add(i);

            //Shuffle the index lists
            Shuffle(player.Sudoku.fixedBoardIndex_easy);
            Shuffle(player.Sudoku.fixedBoardIndex_medium);
            Shuffle(player.Sudoku.fixedBoardIndex_hard);
        }
        //network = NetworkManager.Instance;
        //game started
        if (player.Sudoku.sudokuContinue == 1)
        {
            loadUserInput();
        }
        //new game start
        else
        {
            //Fix Mode
            //make fix sudoku from xml nodes

            Debug.Log("diffmode = " + Difficulty.diff.ToString());
            if(difficult.fix_easy == Difficulty.diff)
            {
                if(fixedSudoku.easy.Count >= player.Sudoku.fix_easy_index)
                    generateFixSudoku(fixedSudoku.easy[player.Sudoku.fix_easy_index]);
            }
            else if(difficult.fix_medium == Difficulty.diff)
            {
                if (fixedSudoku.medium.Count >= player.Sudoku.fix_medium_index)
                    generateFixSudoku(fixedSudoku.medium[player.Sudoku.fix_medium_index]);
            }
            else if (difficult.fix_hard == Difficulty.diff)
            {
                if (fixedSudoku.hard.Count >= player.Sudoku.fix_hard_index)
                    generateFixSudoku(fixedSudoku.hard[player.Sudoku.fix_hard_index]);
            }
            //Random Mode
            else
            {
                //make new one
                player.Sudoku.canChange = setCanChange(Difficulty.diff);
                player.Sudoku.solution = generateSudoku();
                player.Sudoku.check = 1;
                player.Sudoku.second = 0;
                player.Sudoku.minute = 0;
                player.Sudoku.hour = 0;
            }
        }
        //draw sudoku based on the information
        int squareIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                all_number_squares[squareIndex].can_change = player.Sudoku.canChange[i][j];
                if (!player.Sudoku.canChange[i][j])
                {
                    all_number_squares[squareIndex].gameObject.GetComponent<Image>().color = new Color32(214, 214, 214, 255);
                    all_number_squares[squareIndex].setValue(player.Sudoku.solution[i][j]);
                }
                squareIndex++;
            }
            Debug.Log("");
        }
        // This is a library function use to add 1 for every second pass
        player.Sudoku.second += Time.deltaTime;
        saveUserInput();
        //PlayerPrefs.SetInt("sudokuContinue", 0);
    }
    
    // A function formate of updating and converting the second into minutes and hours form
    public void UpdateTimer()
    {
        player.Sudoku.second += Time.deltaTime;
        timer.text = player.Sudoku.hour + "h: " + player.Sudoku.minute + "m: " + (int)player.Sudoku.second + "s";
        player.Sudoku.only_second += Time.deltaTime;
        if (player.Sudoku.second >= 60)
        {
            player.Sudoku.minute++;
            player.Sudoku.second = 0;
        }
        else if (player.Sudoku.minute >= 60)
        {
            player.Sudoku.hour++;
            player.Sudoku.minute = 0;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Easy_complete.text = player.Sudoku.Easy_complete_count.ToString();
        Medium_complete.text = player.Sudoku.Medium_complete_count.ToString();
        Hard_complete.text = player.Sudoku.Hard_complete_count.ToString();
        Total_complete.text = player.Sudoku.Total_complete_count.ToString();

        UpdateTimer();

        //Easy_sec.text = only_hour + "h: " + only_minute + "m: " + (int)Easy_complete_sec + "s";
        Easy_sec.text = Convert.ToInt32(player.Sudoku.Easy_complete_sec).ToString() + "s";
        Medium_sec.text = Convert.ToInt32(player.Sudoku.Medium_complete_sec).ToString() + "s";
        Hard_sec.text = Convert.ToInt32(player.Sudoku.Hard_complete_sec).ToString() + "s";
        Total_sec.text = Convert.ToInt32(player.Sudoku.Total_complete_sec).ToString() + "s";

        checkTxt.text = player.Sudoku.check.ToString();
    }

    // A attempt when server save is not implement for saving the user inputs locally
    public void saveUserInput()
    {
        int squareIndex = 0;
        string[] playerInputs = new string[81];
        for(int i = 0; i< 9; i++)
        {
            for(int j = 0; j< 9; j++)
            {
                playerInputs[squareIndex] = all_number_squares[squareIndex].pen_box.text;
                squareIndex++;
            }
        }
        player.Sudoku.playerInputs = playerInputs;
        player.Sudoku.sudokuContinue = 1;
        //PlayerPrefsX.SetStringArray("sudokuPlayerInputs", playerInputs);
        //PlayerPrefs.SetInt("sudokuContinue", 1);
        savetxt.text = "Saved!";
    }

    // This is the part of loading the user input for local save
    public void loadUserInput()
    {
        int squareIndex = 0;
        string[] playerInputs = player.Sudoku.playerInputs;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if(player.Sudoku.canChange[i][j] && playerInputs[squareIndex] != "")
                    all_number_squares[squareIndex].setValue(Convert.ToInt32(playerInputs[squareIndex]));
                squareIndex++;
            }
        }
        
    }

    // This is the setter for the change-able square base on the difficulty player choose
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

        // The following code is for setting the input space base on what level player choose
        int count = 0;
        System.Random rand = new System.Random();
        // Easy will require less input space
        if (diff == difficult.easy)
        {
            count = 35;
            player.Sudoku.difficulty_level = 1;
        }
        // Medium will increase by 10
        if (diff == difficult.medium)
        {
            count = 45;
            player.Sudoku.difficulty_level = 2;
        }
        // Hard will increase by another 10
        if (diff == difficult.hard)
        {
            count = 55;
            player.Sudoku.difficulty_level = 3;
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
    
    // This is the function of generating the Sudoku board
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
    /// <summary>
    /// It directly insert fixed Sudoku to global variables
    /// </summary>
    /// <param name="value_string"></param>
    static void generateFixSudoku(string value_string)
    {
        Debug.Log(value_string); 

        //value_string: 123456789/TFTFTTTFTFT*
        string[] lines = value_string.Split('*');
        List<List<int>> result_solution = new List<List<int>>();
        List<List<bool>> result_canChange = new List<List<bool>>();

        for(int r = 0; r < 9; r++)
        {
            string[] valuesDisplay = lines[r].Split('/');
            string values = valuesDisplay[0];
            string display = valuesDisplay[1];

            List<int> value_row = new List<int>();
            List<bool> canChange_row = new List<bool>();
            //loop column in a row
            foreach (char c in values)
            {
                value_row.Add(c-'0');
            }
            foreach (char c in display)
            {
                if (c == 'T' || c == 't')
                {
                    canChange_row.Add(true);
                }
                else
                {
                    canChange_row.Add(false);
                }
            }
            result_solution.Add(value_row);
            result_canChange.Add(canChange_row);
        }
        player.Sudoku.solution = result_solution;
        player.Sudoku.canChange = result_canChange;
    }
    // pick number
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

    // remove
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

    // copy
    static List<int> deepCopy(List<int> original)
    {
        List<int> newObject = new List<int>();
        foreach (int i in original)
        {
            newObject.Add(i);
        }
        return newObject;
    }

    // copy2
    static List<List<int>> deepCopy(List<List<int>> original)
    {
        List<List<int>> newObject = new List<List<int>>();
        foreach (var one in original)
        {
            newObject.Add(deepCopy(one));
        }
        return newObject;
    }

    // making the list
    static List<int> makeList()
    {
        List<int> list = new List<int>();
        list.Add(9); list.Add(2); list.Add(3); list.Add(4); list.Add(5); list.Add(6); list.Add(7); list.Add(8); list.Add(1);
        return list;
    }

    // decide which box is it
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

    // A function to call when the check button is clicked
    public void CheckBoard()
    {
        bool solved = true;
        //int check = 1;                  // counting the check button click
        int z = 0;
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (all_number_squares[z].pen_num != player.Sudoku.solution[x][y] && all_number_squares[z].can_change)
                {
                    all_number_squares[z].gameObject.GetComponent<Image>().color = new Color32(255, 81, 81, 255);
                    /*if(check > 5)
                    {
                        solved = true;
                    }
                    else solved = false;*/
                    solved = false;
                }
                else if (all_number_squares[z].pen_num == player.Sudoku.solution[x][y] && all_number_squares[z].can_change)
                {
                    all_number_squares[z].gameObject.GetComponent<Image>().color = new Color(166, 254, 0);
                }
                z++;
            }
        }
        // If the board is solved, display text and store the stats
        if (solved)
        {
            Debug.Log("Solved!");
            complete.SetActive(true);
            complete_text.text = "Successfully Solved\n" + "Time Used: " + player.Sudoku.hour + "h: " + player.Sudoku.minute + "m: " + (int)player.Sudoku.second + "s" + 
                "\nTotal Check: " + player.Sudoku.check;
            player.Sudoku.check = 1;
            player.Sudoku.sudokuContinue = 0;
            //PlayerPrefs.SetInt("sudokuCheck", player.Sudoku.check);
            //PlayerPrefs.SetInt("sudokuContinue", 0);

            if (player.Sudoku.difficulty_level == 1)
            {
                player.Sudoku.Easy_complete_count += 1;
                player.Sudoku.Easy_complete_sec += player.Sudoku.only_second;
                //PlayerPrefs.SetInt("SudokuCompleteEasy", player.Sudoku.Easy_complete_count + 1);
                //PlayerPrefs.SetFloat("SudokuSecEasy", player.Sudoku.Easy_complete_sec + player.Sudoku.only_second);

                /*
                // saving data into server
                network.QueueMessage((new List<string>() { "MASTERMIND_EASYTOTALTIMEPLAYED", "TotalTimePlayed", (player.Sudoku.Easy_complete_count++).ToString() })); //does easy_complete_count need to be incremented?
                network.QueueMessage((new List<string>() { "MASTERMIND_EASYTOTALTIME", "TotalTime", player.Sudoku.only_second.ToString() }));
                */

                // Adding active coin for the plater who finish the board
                player.Incre.coin.active += 30;
            }
            else if (player.Sudoku.difficulty_level == 2)
            {
                player.Sudoku.Medium_complete_count += 1;
                player.Sudoku.Medium_complete_sec += player.Sudoku.only_second;

                //PlayerPrefs.SetInt("SudokuCompleteMedium", player.Sudoku.Medium_complete_count + 1);
                //PlayerPrefs.SetFloat("SudokuSecMedium", Medium_complete_sec + only_second);

                /*
                // saving data into server
                network.QueueMessage((new List<string>() { "MASTERMIND_MEDIUMTOTALTIMEPLAYED", "TotalTimePlayed", (player.Sudoku.Medium_complete_count++).ToString() }));
                network.QueueMessage((new List<string>() { "MASTERMIND_MEDIUMTOTALTIME", "TotalTime", player.Sudoku.only_second.ToString() }));
                */

                // Adding active coin for the plater who finish the board
                player.Incre.coin.active += 70;
            }
            else if (player.Sudoku.difficulty_level == 3)
            {
                player.Sudoku.Hard_complete_count += 1;
                player.Sudoku.Hard_complete_sec += player.Sudoku.only_second;
                
                /*
                // saving data into server
                network.QueueMessage((new List<string>() { "MASTERMIND_HARDTOTALTIMEPLAYED", "TotalTimePlayed", (player.Sudoku.Hard_complete_count++).ToString() }));
                network.QueueMessage((new List<string>() { "MASTERMIND_HARDTOTALTIME", "TotalTime", player.Sudoku.only_second.ToString() }));
                */
                // Adding active coin for the plater who finish the board
                player.Incre.coin.active += 160;
            }

            else if(difficult.fix_easy == Difficulty.diff)
            {
                player.Sudoku.fix_easy_index++;
            }
            else if (difficult.fix_medium == Difficulty.diff)
            {
                player.Sudoku.fix_medium_index++;
            }
            else if (difficult.fix_hard == Difficulty.diff)
            {
                player.Sudoku.fix_hard_index++;
            }
            player.Sudoku.Total_complete_count += 1;
            player.Sudoku.Total_complete_sec += player.Sudoku.only_second;
        }
        else
        {
            Debug.Log("Not Solved");
            player.Sudoku.check++;
            //PlayerPrefs.SetInt("sudokuCheck", check);
        }
    }

    // when square is select, it will have a different color
    public void SelectSquare(GameObject ss)
    {
        if (selected_square != null)
        {
            selected_square.GetComponent<Image>().color = new Color(255, 255, 255);
        }
        selected_square = ss;
        selected_square.GetComponent<Image>().color = new Color(0, 255, 255);
    }

    // changing to the pen
    public void PenChange(int num)
    {
        if (selected_square != null)
        {
            selected_square.GetComponent<SquareControl>().PenValueChange(num);
            savetxt.text = "Not Saved.";
        }
    }

    // changing to the pencil
    public void PencilChange(int num)
    {
        if (selected_square != null)
        {

            selected_square.GetComponent<SquareControl>().PencilValueChange(num);
        }
    }

    public static void Shuffle<T>(IList<T> list)
    {
        System.Random rand = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
