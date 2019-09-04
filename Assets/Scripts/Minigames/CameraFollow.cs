using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Transform player;
    [SerializeField]
    private bool isFollowingAlsoX = false;

    public void SetPlayer(Transform t)
    {
        player = t;
    }


    void LateUpdate() { 
        
        Transform target = player;
        if (target == null)
            return;
        Vector3 desiredPosition;
        Vector3 smoothedPosition;

        if (!isFollowingAlsoX)
        {
            desiredPosition = new Vector3(target.position.x, target.position.y, 0) + offset;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
        else
        {
            desiredPosition = new Vector3(target.position.x, target.position.y, 0) + offset;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
        transform.position = smoothedPosition;

    }

}
