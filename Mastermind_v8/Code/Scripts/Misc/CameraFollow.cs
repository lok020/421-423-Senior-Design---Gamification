using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    
    public float Distance = 5f;
    public bool Locked;
    
    private Transform Target;
    private Vector3 DefaultPosition;
    private bool Panning = false;
    private bool HitTarget = false;
    private float xDir = 0, yDir = 0, moveSpeed = 0, timeRemaining = 0;
    private float TargetZoom = -1;
    private Camera cam;

    private const float refocusTime = 1.5f;


    void Start()
    {
        DefaultPosition = transform.position;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
        }
        Locked = true;
        timeRemaining = refocusTime;
        HitTarget = true;
        cam = GetComponent<Camera>();
    }

    //Used for camera panning
    void FixedUpdate()
    {
        //If camera should pan in a direction
        if (Panning && !Locked)
        {
            //Debug.Log("Time remaining: " + timeRemaining);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                Vector3 moveVector = new Vector3(xDir, yDir, 0).normalized;
                float sFactor = Time.deltaTime * moveSpeed;
                moveVector.Scale(new Vector3(sFactor, sFactor, 0));
                transform.position += moveVector;
            }
            else
            {
                Panning = false;
            }
        }
    }

    //Used for target tracking
    void Update() { 
        //If camera is locked onto a target
        if(Locked)
        {
            //Hasn't hit target
            if(!HitTarget)
            {
                //If target is null, move to DefaultPosition over course of two seconds or until very close
                if (Target == null)
                {
                    if (timeRemaining > 0 && (Vector3.Distance(DefaultPosition, transform.position) > 1) || (System.Math.Abs(cam.orthographicSize - TargetZoom) > 0.5))
                    {
                        //Move camera into position
                        Vector3 newVector = DefaultPosition - transform.position;
                        newVector.z -= Distance;
                        float sFactor = Time.deltaTime / timeRemaining;
                        newVector.Scale(new Vector3(sFactor, sFactor, sFactor));
                        transform.position += newVector;
                        //Adjust zoom as well
                        if (TargetZoom > 0)
                            cam.orthographicSize += (TargetZoom - cam.orthographicSize) * (Time.deltaTime / timeRemaining);
                        timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        transform.position = new Vector3(DefaultPosition.x, DefaultPosition.y, DefaultPosition.z - Distance);
                        if (TargetZoom > 0)
                            cam.orthographicSize = TargetZoom;
                        HitTarget = true;
                    }
                }
                //Else move to target over course of two seconds or until very close
                else
                {
                    if (timeRemaining > 0 && (Vector3.Distance(DefaultPosition, transform.position) > 1) || (System.Math.Abs(cam.orthographicSize - TargetZoom) > 0.5))
                    {
                        Vector3 newVector = Target.position - transform.position;
                        newVector.z -= Distance;
                        float sFactor = Time.deltaTime / timeRemaining;
                        newVector.Scale(new Vector3(sFactor, sFactor, sFactor));
                        transform.position += newVector;
                        //Adjust zoom as well
                        if(TargetZoom > 0)
                            cam.orthographicSize += (TargetZoom - cam.orthographicSize) * (Time.deltaTime / timeRemaining);
                        timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        transform.position = new Vector3(Target.position.x, Target.position.y, Target.position.z - Distance);
                        if (TargetZoom > 0)
                            cam.orthographicSize = TargetZoom;
                        HitTarget = true;
                    }
                }
            }
            //If Target is not null, track target frame by frame
            else if(Target != null)
            {
                transform.position = new Vector3(Target.position.x, Target.position.y, Target.position.z - Distance);
            }
        }
    }

    public void Pan(float x_dir, float y_dir, float move_Speed, float timeToPan)
    {
        xDir = x_dir;
        yDir = y_dir;
        moveSpeed = move_Speed;
        timeRemaining = timeToPan;
        Panning = true;
    }

    public void SetTarget(Transform t)
    {
        Target = t;
        HitTarget = false;
        timeRemaining = refocusTime;
    }

    public void SetTargetZoom(float zoom)
    {
        TargetZoom = zoom;
        HitTarget = false;
        timeRemaining = refocusTime;
    }
} 
