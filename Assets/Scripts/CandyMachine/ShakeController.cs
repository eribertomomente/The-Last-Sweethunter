using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsCMCollider))]
public class ShakeController : MonoBehaviour
{

    public float ShakeDetectionThreshold;
    public float MinShakeInterval;

    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;

    private PhysicsCMCollider physicsController;

     void Start()
    {
        sqrShakeDetectionThreshold = Mathf.Pow(ShakeDetectionThreshold, 2);
        physicsController = GetComponent<PhysicsCMCollider>();
    }

     void Update()
    {
        if(Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold && Time.unscaledTime >= timeSinceLastShake + MinShakeInterval)
        {
            physicsController.ShakeRigidBodies(Input.acceleration);
            timeSinceLastShake = Time.unscaledTime;
        }
    }
}
