using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownUpgradesShow : MonoBehaviour
{

    public Text maxpop;
    public Text training;
    public Text forge;
    public Text herb;
    public Text bakery;
    public Text reputation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        maxpop.text = "Maximum Population: " + GlobalControl.Instance.max_population;
        training.text = "Training Multiplier: " + GlobalControl.Instance.full_items_list[69].value + "x";
        forge.text = "Forge Shop: Level " + GlobalControl.Instance.full_items_list[70].value;
        herb.text = "Herb Shop:  Level " + GlobalControl.Instance.full_items_list[71].value;
        bakery.text = "Bakery Shop: Level " + GlobalControl.Instance.full_items_list[72].value;
        reputation.text = GlobalControl.Instance.reputation.ToString() + " Rep";
    }
}
