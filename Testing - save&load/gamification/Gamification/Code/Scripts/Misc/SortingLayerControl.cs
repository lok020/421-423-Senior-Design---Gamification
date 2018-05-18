using UnityEngine;
using System.Collections;

public class SortingLayerControl : MonoBehaviour {

    private int _sortingOrder;
    private int _defaultSortingOrder;
    private string _sortingLayer;
    private string _defaultSortingLayer;
    private GameObject player = null;

	// Use this for initialization
	void Start () {
        _sortingLayer = GetComponent<SpriteRenderer>().sortingLayerName;
        _sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
	}

    void Update()
    {
        if (player == null) return;
        if(player.GetComponent<PlayerController>().NumberOfSpritesBehind == 0)
        {
            player.GetComponent<SpriteRenderer>().sortingLayerName = player.GetComponent<PlayerController>().DefaultSortingLayer;
            player.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<PlayerController>().DefaultSortingOrder;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
            other.GetComponent<PlayerController>().NumberOfSpritesBehind++;
            other.GetComponent<SpriteRenderer>().sortingLayerName = _sortingLayer;
            if (other.GetComponent<SpriteRenderer>().sortingOrder >= _sortingOrder - 1)
                other.GetComponent<SpriteRenderer>().sortingOrder = _sortingOrder - 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().NumberOfSpritesBehind--;
        }
    }
}
