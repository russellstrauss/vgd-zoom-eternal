using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWheel : MonoBehaviour
{
    public WheelCollider targetWheel;
    // private Vector3 wheelPosition = new Vector3();
    // private Quaternion wheelRotation = new Quaternion();
    void Update()
    {
        // targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        // transform.position = wheelPosition;
        // transform.rotation = wheelRotation * Quaternion.Euler(90, 0, 0);;
    }
}
