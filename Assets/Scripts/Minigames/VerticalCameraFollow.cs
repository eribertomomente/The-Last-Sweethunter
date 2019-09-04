using System;
using System.Collections;
using UnityEngine;

public class VerticalCameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Transform player;
    [SerializeField]
    private bool isFollowingAlsoX = false;
    private bool endCinematic;

    public float arrivalPointCamera;

    public void SetPlayer(Transform t)
    {
        player = t;
    }

    private void Start()
    {
        endCinematic = false;
        StartCoroutine(MoveCamera());
    }
  
    private IEnumerator MoveCamera()
    {
        while(transform.position.y > arrivalPointCamera && endCinematic == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                transform.position.Set(transform.position.x, arrivalPointCamera, 0);
                endCinematic = true;
            }
            else
            {
                transform.Translate(Vector3.down * Time.deltaTime * 7);
            }

            yield return null;
        }

        endCinematic = true;

    }

    void LateUpdate() {

        if (endCinematic)
        {
            Transform target = player;
            if (target == null)
                return;
            Vector3 desiredPosition;
            Vector3 smoothedPosition;

            if (!isFollowingAlsoX)
            {
                desiredPosition = new Vector3(0, target.position.y, 0) + offset;
                smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            }
            else
            {
                desiredPosition = new Vector3(0, target.position.y, 0) + offset;
                smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            }
            transform.position = smoothedPosition;
        }

    }

}
