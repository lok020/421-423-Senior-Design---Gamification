using System;
using UnityEngine;
using UnityEngine.UI;

public class VirtualPlayer : MonoBehaviour {

    public int Id;
    public int Level;
    public int SpriteId;
    public string Name;

    public int MoveSpeed = 8;       //Keep this the same as the Player Controller!
    
    private Vector3 _move, _target;
    private Animator _anim;
    private DialogController _dialog;
    private int Direction = 0;           //What direction the player is facing
    private int FramesStill = 0;
    
    private float _timeSinceUpdate = 0;
    private float _hideAfterInactiveSeconds = 15;   //Hide inactive players after 15 seconds

    //Unity functions
    public void Start()
    {
        _anim = GetComponent<Animator>();
        _dialog = GetComponent<DialogController>();
    }

    public void Update()
    {
        //Update message timer
        _timeSinceUpdate += Time.deltaTime;
        //If player is inactive, hide them
        if(_timeSinceUpdate > _hideAfterInactiveSeconds)
        {
            GetComponentInChildren<Canvas>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        //Get vector to target location
        float moveX = (_target.x - transform.position.x);
        float moveY = (_target.y - transform.position.y);
        float moveDistance = MoveSpeed * Time.deltaTime;
        //If more than one "step" away, move towards it
        if (moveDistance < Vector2.Distance(transform.position, _target))
        {
            transform.position += _move.normalized * MoveSpeed * Time.deltaTime;
            _move = new Vector3(moveX, moveY, transform.position.z);
        }
        //Else do not move towards it
        else
        {
            transform.position = _target;
            _move = Vector3.zero;
        }

        //Update frames still counter (helps determine if player actually stopped)
        if (_move.magnitude == 0) FramesStill++;
        else FramesStill = 0;

        //Update animation
        UpdateAnimation();
    }

    //Updates animation
    void UpdateAnimation()
    {
        //Get direction
        float x = _move.x;
        float y = _move.y;
        if (x < 0 && Math.Abs(x) >= Math.Abs(y))        //NW, W or SW
        {
            Direction = 1;  //West
        }
        else if (x > 0 && Math.Abs(x) >= Math.Abs(y))   //NE, E or SE
        {
            Direction = 3;  //East
        }
        else if (y > 0 && Math.Abs(y) >= Math.Abs(x))   //N
        {
            Direction = 2;  //North
        }
        else if (y < 0)                                 //S
        {
            Direction = 0;  //South
        }
        //Update animation
        _anim.SetInteger("Direction", Direction);
        _anim.SetBool("Moving", (FramesStill < 10));
        //Debug.Log("Moving: " + (FramesStill < 10));
    }

    public void InitializeStats(int id, int level, int spriteId, string name)
    {
        //Initialize variables
        Id = id;
        Level = level;
        SpriteId = spriteId;
        SetUsername(name);
    }

    public void UpdateStats(int level, int spriteId, string name)
    {
        //Update timers and unhide player
        _timeSinceUpdate = 0;
        GetComponentInChildren<Canvas>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        if (Level != level)
        {
            //Update level
            Level = level;
            UpdatePlayerLabel();
            //TO DO: Add fireworks effect
        }
        if(SpriteId != spriteId)
        {
            //Update player sprite
            SpriteId = spriteId;
            //TO DO: Change player sprite
            //TO DO: Add visual effect for changing sprite
        }
        if(Name != name)
        {
            //Update player name
            Name = name;
            UpdatePlayerLabel();
        }
    }

    //Sets movement target
    public void Move(float targetX, float targetY)
    {
        //Update timers and unhide player
        _timeSinceUpdate = 0;
        GetComponentInChildren<Canvas>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        //If distance is way too far away (over 10 units) snap to it directly
        if (Vector2.Distance(transform.position, new Vector2(targetX, targetY)) > 10)
            MoveDirectlyTo(targetX, targetY);
        //Else move towards it like normal
        else
            _target = new Vector3(targetX, targetY, transform.position.z);
    }

    //Moves transform directly
    public void MoveDirectlyTo(float x, float y)
    {
        //Update timers and unhide player
        _timeSinceUpdate = 0;
        GetComponentInChildren<Canvas>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        _move = Vector3.zero;
        transform.position = new Vector3(x, y, transform.position.z);
        _target = transform.position;
    }

    //Set player's username
    private void SetUsername(string newName)
    {
        //Update name
        Name = newName;
        UpdatePlayerLabel();
    }

    //Update player's label (call this if the username or level change)
    private void UpdatePlayerLabel()
    {
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Player Label")
            {
                t.text = Name + " (" + Level + ")";
            }
        }
    }

    public void UpdateText(string text)
    {
        //Update timers and unhide player
        _timeSinceUpdate = 0;
        GetComponentInChildren<Canvas>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        //If this text is already being displayed, ignore it (we can receive the same text message multiple times)
        if (text == _dialog.GetText()) return;
        //Otherwise set the text
        _dialog.Speak(text, 5.0f);
    }
}
