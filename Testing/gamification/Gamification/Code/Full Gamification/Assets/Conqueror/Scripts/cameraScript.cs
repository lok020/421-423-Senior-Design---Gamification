using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {
    public GameObject target;
    private Vector3 previousPosition;
    private Vector3 targetPosition;
    public float moveSpeed;

    public Vector3 minCamPosition;
    public Vector3 maxCamPosition;
    private bool startChange = false;

	// Use this for initialization
	void Start () {
		if (target.transform.position.x <= minCamPosition.x)
        {
            targetPosition.x = minCamPosition.x;
            startChange = true;
        }

        if (target.transform.position.y <= minCamPosition.y)
        {
            targetPosition.y = minCamPosition.y;
            startChange = true;
        }

        if (target.transform.position.x >= minCamPosition.x)
        {
            targetPosition.x = minCamPosition.x;
            startChange = true;
        }

        if (target.transform.position.y >= minCamPosition.y)
        {
            targetPosition.y = minCamPosition.y;
            startChange = true;
        }

        if (!startChange)
        {
            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        }
        previousPosition = targetPosition;
    }
	
	// Update is called once per frame
	void Update () {
        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        if (targetPosition.x <= minCamPosition.x || targetPosition.x >= maxCamPosition.x)
        {
            targetPosition.x = previousPosition.x;
        }
        if (targetPosition.y <= minCamPosition.y || targetPosition.y >= maxCamPosition.y)
        {
            targetPosition.y = previousPosition.y;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        previousPosition = targetPosition;
    }
}
