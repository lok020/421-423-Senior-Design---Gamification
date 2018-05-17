using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour {

    public GameObject health;
    public GameObject stamina;
    public GameObject dexterity;
    public GameObject insight;


	// Use this for initialization
	void Start () {
        health.GetComponent<Text>().text = "Health:\n" + GlobalControl.Instance.hp.ToString();
        stamina.GetComponent<Text>().text = "Stamina:\n" + GlobalControl.Instance.stam.ToString();
        dexterity.GetComponent<Text>().text = "Dexterity:\n" + GlobalControl.Instance.dex.ToString();
        insight.GetComponent<Text>().text = "Insight:\n" + GlobalControl.Instance.ins.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
