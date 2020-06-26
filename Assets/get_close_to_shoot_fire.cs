using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class get_close_to_shoot_fire : MonoBehaviour
{
    private GameObject wayPoint;
    private Vector3 wayPointPos;
    //This will be the AI's speed. Adjust as necessary.
    private float speed = 6.0f;
    private Transform Target;
    private float RotationSpeed = 3.0f;

     //values for internal use
     private Quaternion _lookRotation;
     private Vector3 _direction;
     private float distance;
     private float trail = 8.0f;
    void Start ()
    {
      //At the start of the game, the zombies will find the gameobject called wayPoint.
      wayPoint = GameObject.Find("wayPoint");
      Target = wayPoint.transform;
    }

    void Update ()
    {

    distance = Vector3.Distance(this.transform.position, wayPoint.transform.position);
    //we are too close and do not want to be hit
     wayPointPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
    if (distance < trail) {
        //Debug.Log("Should be backing up?");
        transform.position = Vector3.MoveTowards(transform.position, wayPointPos, -1 * speed * Time.deltaTime);
    } else {
        transform.position = Vector3.MoveTowards(transform.position, wayPointPos, speed * Time.deltaTime);
    }

      //wayPointRotation = new Quaternion(wayPoint.transform.rotation.x, 0, wayPoint.transform.rotation.z, 0);
      //find the vector pointing from our position to the target
      _direction = (Target.position - transform.position).normalized;

    //create the rotation we need to be in to look at the target
    _lookRotation = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, -90, 0);;
    Debug.Log(_lookRotation);

    //rotate us over time according to speed until we are in the required rotation
    transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);

    }
}
