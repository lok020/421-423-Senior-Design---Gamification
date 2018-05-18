using UnityEngine;
using System.Collections;

public class ContentLock : MonoBehaviour {

    public int Lock;
    public bool OnlyUnlockWhenEqual = false;        //true: only unlock for ==, false: >=

    private NetworkManager _network;

	// Use this for initialization
	void Start () {
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //Disable this object if lock is either less than requirement, or greater than and it should only unlock when equal
	    if((Lock > _network.ContentLock) || (OnlyUnlockWhenEqual && Lock < _network.ContentLock))
        {
            //GetComponent<InstanceSwitch>().enabled = false;
            gameObject.SetActive(false);
        }
	}
}
