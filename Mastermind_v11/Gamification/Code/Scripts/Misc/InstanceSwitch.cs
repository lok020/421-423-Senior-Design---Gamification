using UnityEngine;
using System.Collections;

public class InstanceSwitch : MonoBehaviour {
    public SceneManager sceneManager;
	public string instanceName;
	public GameObject message;
    public GameObject player;

    public int SpawnPointID = 0;

	//when the player collides with the portal start message
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
            /*
            //Debug.Log ("Switching Instances");
            //Notify the database of this scene change
            var db = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
            db.UpdatePlayerStat("CurrentScene", instanceName);
            db.UpdatePlayerStat("CurrentSceneSpawn", SpawnPointID.ToString());
            if (sceneManager != null)
            {
                db.UpdatePlayerStat("CurrentSceneManager", sceneManager.name);
            }
            else
            {
                db.UpdatePlayerStat("CurrentSceneManager", "");
            }
            //Update the player's layer
            var player = other.GetComponent<PlayerController>();
            var sprite = other.GetComponent<SpriteRenderer>();
            sprite.sortingLayerName = player.DefaultSortingLayer;
            sprite.sortingOrder = player.DefaultSortingOrder;
            player.NumberOfSpritesBehind = 0;
            */
        }
	}


}
