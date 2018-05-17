using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWallManager : MonoBehaviour {

    // Sets wall to a faded color
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.5f);
        }
    }
}
