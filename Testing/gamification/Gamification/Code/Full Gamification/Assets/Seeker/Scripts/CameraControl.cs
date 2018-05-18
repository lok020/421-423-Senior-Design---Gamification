using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public bool in_dungeon;
    public bool pause = false;
    public GameObject target;
    private Vector3 previous_position;
    private Vector3 target_position;
    public float move_speed;

    public Vector3 min_cam_position;
    public Vector3 max_cam_position;

    private bool start_change = false;

    // Use this for initialization
    void Start()
    {
        if (in_dungeon)
        {
            max_cam_position.x = GlobalControl.Instance.dungeon.columns - 8.2f;
            max_cam_position.y = GlobalControl.Instance.dungeon.rows - 3.5f;
        }

        if (target.transform.position.x <= min_cam_position.x)
        {
            target_position.x = min_cam_position.x;
            start_change = true;
        }

        if (target.transform.position.y <= min_cam_position.y)
        {
            target_position.y = min_cam_position.y;
            start_change = true;
        }

        if (target.transform.position.x >= max_cam_position.x)
        {
            target_position.x = max_cam_position.x;
            start_change = true;
        }

        if (target.transform.position.y >= max_cam_position.y)
        {
            target_position.y = max_cam_position.y;
            start_change = true;
        }

        if (!start_change)
        {
            target_position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        }

        previous_position = target_position;
    }

    // Update is called once per frame
    void Update () {
        if (pause)
        {
            return;
        }

        target_position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        if (target_position.x <= min_cam_position.x || target_position.x >= max_cam_position.x)
        {
            target_position.x = previous_position.x;
        }
        if (target_position.y <= min_cam_position.y || target_position.y >= max_cam_position.y)
        {
            target_position.y = previous_position.y;
        }

        transform.position = Vector3.Lerp(transform.position, target_position, move_speed * Time.deltaTime);
        previous_position = target_position;
	}
}
