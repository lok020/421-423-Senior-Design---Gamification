using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Message : MonoBehaviour {

	//object for the popup message
	public GameObject msgPanel;
	//text for the message
	public Text msgText;
	public string target;//target for the instanceswitch

	// Use this for initialization
	void Start () {
		HideMessage ();
	}

	//when the message is set to active show the object
	void onEnable(){
        ShowMessage ();
	}

	//basically just sets the text and makes the object active
	public void ShowMessage(){
        msgPanel.SetActive (true);
	}

	//simply hide the message
	public void HideMessage(){
		msgPanel.SetActive (false);
	}

	public void gotoInstance(){
        Application.LoadLevel(target);
	}


}
