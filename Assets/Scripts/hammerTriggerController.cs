using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerTriggerController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool EnteredTrigger = false;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "playerCollider")
        {
            EnteredTrigger = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "playerCollider")
        {
            EnteredTrigger = false;
        }
    }
     
}
