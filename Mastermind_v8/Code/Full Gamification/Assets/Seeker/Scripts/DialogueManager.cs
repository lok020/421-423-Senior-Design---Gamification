using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour {

    public GameObject block;
    private bool continuous;
    public float delay;
    public bool immediate_disable;
    public bool continue_disable;

    public GameObject text_box;

    public Text the_text;

    public TextAsset text_file;
    public string[] text_lines;

    public int start_at_line;
    public int current_line;
    public int end_at_line;

    public bool is_active;

    public PlayerMovement player;

    private float pause;

    public bool script_changer;
    public UnityEvent action_at_end;

    public bool script_holder;
    public UnityEvent hold_action;


    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerMovement>();

        block.SetActive(true);

        if (immediate_disable)
        {
            player.can_move = false;
        }

        if (text_file != null)
        {
            text_lines = (text_file.text.Split('\n'));
        }

        if (start_at_line == 0)
        {
            start_at_line = current_line;
        }

        if (is_active)
        {
            EnableTextBox();
        }

        if (delay >= 0)
        {
            Invoke("EnableTextBox", delay);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!is_active)
        {
            return;
        }
        

        is_active = true;

        /*if (player != null)
        {
            player.can_move = false;
        }*/

        if (text_lines[current_line][0] == '*')
        {
            if (script_holder)
            {
                hold_action.Invoke();
            }
            text_box.SetActive(false);


            current_line++;

            pause = float.Parse(text_lines[current_line]);


            current_line++;
        }

        if (pause > 0)
        {
            pause -= Time.deltaTime;
        }

        else
        {
            text_box.SetActive(true);
            the_text.text = text_lines[current_line];

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                current_line++;
            }


            if (current_line > end_at_line)
            {
                if (script_changer)
                {
                    action_at_end.Invoke();
                }
                current_line = start_at_line;
                DisableTextBox();
                return;
            }
        }
	}

    public void EnableTextBox()
    {
        if (player != null)
        {
            player.can_move = false;
        }
        text_box.SetActive(true);
        is_active = true;
    }

    public void DisableTextBox()
    {
        if (player != null && !continue_disable)
        {
            player.can_move = true;
        }
        text_box.SetActive(false);
        is_active = false;
        block.SetActive(false);
    }

    public void Reset(TextAsset new_text)
    {
        text_lines = (new_text.text.Split('\n'));
        end_at_line = text_lines.Length - 1;
    }

    public void Again()
    {
        Start();
    }

    public void IncreaseProgress()
    {
        GlobalControl.Instance.QuestUpdate();
    }
}
