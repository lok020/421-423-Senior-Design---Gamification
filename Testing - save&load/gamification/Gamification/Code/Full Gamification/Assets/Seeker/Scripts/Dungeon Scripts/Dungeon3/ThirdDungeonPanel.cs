using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdDungeonPanel : MonoBehaviour {


    public int answer = 0;
    public GameObject the_player;
    public GameObject thing;
    public Text text_box;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        the_player.GetComponent<PlayerMovement>().can_move = false;
    }

    public void SetObject(GameObject this_object)
    {
        thing = this_object;
    }

    public void SetText(string dialogue)
    {
        text_box.text = dialogue;
    }

    public void Answered()
    {
        if (thing.tag == "PlantTrap")
        {
            switch (answer)
            {
                case 1:
                    thing.GetComponent<Vines2>().AnswerYes();
                    break;
                case 2:
                    thing.GetComponent<Vines2>().AnswerNo();
                    break;
                default:
                    break;
            }
            answer = 0;
            the_player.GetComponent<PlayerMovement>().can_move = true;
            gameObject.SetActive(false);
        }

        else if (thing.tag == "Exit")
        {
            switch (answer)
            {
                case 1:
                    thing.GetComponent<ThirdDungeon>().Leave();
                    break;
                case 2:
                    thing.GetComponent<ThirdDungeon>().Stay();
                    break;
                default:
                    break;
            }
            answer = 0;
            the_player.GetComponent<PlayerMovement>().can_move = true;
            gameObject.SetActive(false);
        }
    }

    public void yes()
    {
        answer = 1;
        Answered();
    }

    public void no()
    {
        answer = 2;
        Answered();
    }
}
