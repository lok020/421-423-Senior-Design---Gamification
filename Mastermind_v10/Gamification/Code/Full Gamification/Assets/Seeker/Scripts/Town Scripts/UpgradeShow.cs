using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShow : MonoBehaviour {

    public Text health;
    public Text stamina;
    public Text dexterity;
    public Text insight;
    public Text reputation;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        health.text = "Health: " + GlobalControl.Instance.hp.ToString();
        stamina.text = "Stamina: " + GlobalControl.Instance.stam.ToString();
        dexterity.text = "Dexterity: " + GlobalControl.Instance.dex.ToString();
        insight.text = "Insight: " + GlobalControl.Instance.ins.ToString();
        reputation.text = GlobalControl.Instance.reputation.ToString() + " Rep";
    }
}
