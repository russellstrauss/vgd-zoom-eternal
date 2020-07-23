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
    public int count = 0;
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
		playerRb = player.GetComponent<Rigidbody>();
        heliBotController = player.GetComponent<HeliBotController>();
    }
    
    // Update is called once per frame
    void Update () 
    {
        // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position, GameObject.FindWithTag("hammer").transform.right, Time.deltaTime * 1);
        // transform.Rotate(new Vector3(0, 0, 1));
        // targetRotation = Quaternion.Euler(0,180,RotateAngle*count+origionZ) * Quaternion.identity;
        // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position + new Vector3(0, 0, 5) , new Vector3(0, 0, 1), 1 );
        count++;

        // Debug.DrawRay(gameObject.transform.position, new Vector3(0, 1, 0), Color.red);
        if (m_hammer_trigger.GetComponent<hammerTriggerController>().EnteredTrigger)
        {
            Debug.Log("start to rotat1");
            // transform.RotateAround(hammer.transform.position , new Vector3(0, 0, 1), Time.deltaTime * 2.0f);
            Vector3 dir = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(hammer.transform.rotation, rotation, Time.deltaTime * 1.0f);

            
            // transform.Rotate(0, 0, 2);
            // transform.Rotate(new Vector3(0, 0, 1));
            // print("GameObject.FindWithTag().transform:"+GameObject.FindWithTag("hammer").transform.rotation);
        }
        else
        {
            Debug.Log("start to rotat2");
            // transform.RotateAround(GameObject.FindWithTag("hammer").transform.position + new Vector3(2.5f, 0, 0) , new Vector3(0, 0, -1), 5 );
            transform.rotation = Quaternion.Lerp(hammer.transform.rotation, originalRotationValue, Time.deltaTime * 1.0f);
            // print("GameObject.FindWithTag().transform::::::"+GameObject.FindWithTag("hammer").transform.rotation);
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
    	
    	// TODO add more healthy to the player
		heliBotController.SubtractHealth(50);

    	// GetComponent<MeshRenderer>().enabled = false;
    	// GetComponent<Collider>().enabled = false;

    	//remove the effect from theplayer
    	yield return new WaitForSeconds(duration);
    	//wait x amount of seconds

    	// remove power up object
    	ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
    	Destroy(clone, duration);
    	// Destroy(gameObject);
    }
}
