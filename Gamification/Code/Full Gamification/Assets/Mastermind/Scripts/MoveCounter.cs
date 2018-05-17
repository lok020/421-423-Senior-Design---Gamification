using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCounter : MonoBehaviour {

    public Text Moves;
    private int move;

	// Use this for initialization
	void Start () {
		
	}

    void updateMoves()
    {

        //Moves.text = move;
    }
	
	// Update is called once per frame
	void Update () {
        updateMoves();
	}
}
