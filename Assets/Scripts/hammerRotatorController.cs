using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerRotatorController : MonoBehaviour
{
    // Start is called before the first frame update
    private float origionZ;
    private Quaternion targetRotation;
    public int count = 0;
    // Use this for initialization
    public Vector3 RotateSpeed;
    public float speed = 0.1f;
    public Quaternion originalRotationValue;
    private GameObject m_cube;
    void Start () {
        // origionZ = transform.rotation.z;
        originalRotationValue = transform.rotation;
        m_cube = GameObject.FindWithTag("hammer_trigger");
    }
    
    // Update is called once per frame
    void Update () 
    {
        // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position, GameObject.FindWithTag("hammer").transform.right, Time.deltaTime * 1);
        // transform.Rotate(new Vector3(0, 0, 1));
        // targetRotation = Quaternion.Euler(0,180,RotateAngle*count+origionZ) * Quaternion.identity;
        // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position + new Vector3(0, 0, 5) , new Vector3(0, 0, 1), 1 );
        // transform.Rotate (Vector3.up * 50 * Time.deltaTime, Space.Self);
        // currentTime += Time.deltaTime;
        // if (currentTime < 7f)
        // {
        //     transform.localEulerAngles += (RotateSpeed * Time.deltaTime * speed);
        // }
        count++;
        
        if (m_cube.GetComponent<hammerTriggerController>().EnteredTrigger)
        {
            transform.RotateAround(GameObject.FindWithTag("hammer").transform.position , new Vector3(0, 0, 1), 1 );
            // transform.Rotate(0, 0, 2);
            // transform.Rotate(new Vector3(0, 0, 1));
            // print("GameObject.FindWithTag().transform:"+GameObject.FindWithTag("hammer").transform.rotation);
        }
        else
        {
            // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position + new Vector3(2.5f, 0, 0) , new Vector3(0, 0, -1), 5 );
            transform.rotation = Quaternion.Slerp(GameObject.FindWithTag("hammer").transform.rotation, originalRotationValue, Time.time * 1.0f);
            // print("GameObject.FindWithTag().transform::::::"+GameObject.FindWithTag("hammer").transform.rotation);
        }

    }
}
