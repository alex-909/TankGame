using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Move : MonoBehaviour
{
    public bool shouldMove;
    public float moveTime;
    public Vector3 Pos1;
    public Vector3 Pos2;

    private Vector3 targetPosition;
    private Vector3 velocity_move = Vector3.zero;

    public bool shouldRotate;
    public float velocity_rotate;
    public Transform Rot1;
    public Transform Rot2;

    private Transform targetRotation;
    void Start()
    {
        targetPosition = Pos1;
        targetRotation = Rot1;
    }

    void Update()
    {
        if (shouldMove) { Move(); }

        if (shouldRotate) { Rotate(); }
    }
    void Move()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity_move , moveTime);
        if ((transform.position - Pos1).magnitude < 0.02f)
        {
            targetPosition = Pos2;
        }
        else if ((transform.position - Pos2).magnitude < 0.02f) 
        {
            targetPosition = Pos1;
        }
    }
    void Rotate() 
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation.rotation, Time.deltaTime * velocity_rotate);
        if (transform.rotation == Rot1.rotation)
        {
            targetRotation = Rot2;
        }
        else if (transform.rotation == Rot2.rotation)
        {
            targetRotation = Rot1;
        }
    }
}
