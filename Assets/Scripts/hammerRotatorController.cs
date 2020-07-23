using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerRotatorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pickupEffect;
	// public float multiplier = 1.4f;
	public float duration = 2;
	private GameObject player;
	private Rigidbody playerRb;
	private HeliBotController heliBotController;
    private Quaternion targetRotation;
    // Use this for initialization
    public float speed = 0.1f;
    public Quaternion originalRotationValue;
    private GameObject m_hammer_trigger;
    private GameObject hammer;
    public Transform target;

    void Start () {
        originalRotationValue = transform.rotation;
        m_hammer_trigger = GameObject.FindWithTag("hammer_trigger");
        hammer = GameObject.FindWithTag("hammer");
        player = GameObject.FindWithTag("Player");
		if (player != null) {
			playerRb = player.GetComponent<Rigidbody>();
			heliBotController = player.GetComponent<HeliBotController>();
		}
    }
    
    // Update is called once per frame
    void Update () 
    {

        if (m_hammer_trigger.GetComponent<hammerTriggerController>().EnteredTrigger)
        {
            Debug.Log("start to rotat1");
			target = m_hammer_trigger.transform;
            Vector3 dir = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(hammer.transform.rotation, rotation, Time.deltaTime * 1.0f);
        }
        else
        {
            Debug.Log("start to rotat2");
            transform.rotation = Quaternion.Lerp(hammer.transform.rotation, originalRotationValue, Time.deltaTime * 1.0f);
        }

    }
    void OnCollisionEnter(Collision collision){
		
		if (collision.collider.CompareTag("playerCollider")) {
            Debug.Log("start to rotat3");
			Vector3 contactNormal = collision.contacts[0].normal;
			playerRb.AddForce(new Vector3(0, 1, 0) * 10000, ForceMode.Impulse);
			FindObjectOfType<AudioManager>().Play("crash");
			StartCoroutine(Damage());
		}
	}

    IEnumerator Damage()
    {
        Debug.Log("start to rotat4");
    	GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
		
		heliBotController.SubtractHealth(250);

    	//remove the effect from theplayer
    	yield return new WaitForSeconds(duration);
    	//wait x amount of seconds

    	// remove power up object
    	ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
    	Destroy(clone, duration);
    }
}
