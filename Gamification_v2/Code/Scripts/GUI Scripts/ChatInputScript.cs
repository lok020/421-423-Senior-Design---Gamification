using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatInputScript : MonoBehaviour {

    public GameObject Text;

    private EventSystem _system;
    private PlayerController _player;
    private Text _text;

    void Start()
    {
        //Get refence to event system
        _system = EventSystem.current;
        //Get reference to actual text fields
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Chat Input Text")
            {
                _text = t;
            }
        }
        //Get reference to player
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        //If player is currently typing a message
        if (_player.IsTypingChatMessage)
        {
            //If player has hit Esc, clear the text and lose focus
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GetComponent<InputField>().text = "";
                DeselectTextBox();
            }
            //If player has hit Enter, send the text message via UDP and clear the text
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                _player.SetTextMessage(_text.text);
                GetComponent<InputField>().text = "";
                DeselectTextBox();
            }
        }
        //Else allow the player to snap to it if not selected
        else
        {
            //If player hits "Enter" or "t", start typing
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.T))
            {
                SelectTextBox();
            }
        }
        
        //Handle mouse clicks
        if (Input.GetMouseButton(0))
        {
            //If field is clicked on, select it
            Physics2D.queriesHitTriggers = true;
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, Mathf.Infinity, Physics2D.AllLayers);
            if (hit && hit.collider.name == "Chat Input Field")
            {
                SelectTextBox();
            }
            //Else deselect it
            else
            {
                DeselectTextBox();
            }
        }
    }

    private void SelectTextBox()
    {
        InputField inputField = GetComponent<InputField>();
        _system.SetSelectedGameObject(Text);
        _player.IsTypingChatMessage = true;
        inputField.interactable = true;
        inputField.OnPointerClick(new PointerEventData(_system));
    }

    private void DeselectTextBox()
    {
        InputField inputField = GetComponent<InputField>();
        _system.SetSelectedGameObject(null);
        _player.IsTypingChatMessage = false;
        inputField.interactable = false;
    }
}
