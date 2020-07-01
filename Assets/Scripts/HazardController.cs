using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
	public GameObject pickupEffect;
	// public float multiplier = 1.4f;
	public float duration = 2;
	private GameObject player;
	private Rigidbody playerRb;
	private HeliBotController heliBotController;
	
	void Start()
	{
		// var rb = GetComponent<Rigidbody>();
        // rb.angularVelocity = Random.insideUnitSphere;
		player = GameObject.FindWithTag("Player");
		playerRb = player.GetComponent<Rigidbody>();
		heliBotController = player.GetComponent<HeliBotController>();
	}
	
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.CompareTag("playerCollider")) {
			Vector3 contactNormal = collision.contacts[0].normal;
			
			playerRb.AddForce(new Vector3(0, 1, 0) * 10000, ForceMode.Impulse);
			FindObjectOfType<AudioManager>().Play("crash");
		}
	}
	
	
	
    void OnTriggerEnter(Collider other)
    {

    	if (other.gameObject.CompareTag("playerCollider")){
    		// Pickup(other);
    		StartCoroutine(Pickup(other));
			GetComponent<TimeStop>().StopTime(0.05f, 10, 0.1f);
		}
    }
    IEnumerator Pickup(Collider other)
    {
    	// Debug.Log("Hazard is being picked");
    	GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
    	
    	// TODO add more healthy to the player
		heliBotController.SubtractHealth(100);

    	// GetComponent<MeshRenderer>().enabled = false;
    	// GetComponent<Collider>().enabled = false;

    	//remove the effect from theplayer
    	yield return new WaitForSeconds(duration);
    	//wait x amount of seconds

    	// remove power up object
    	ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
    	Destroy(clone, particle.duration);
    	// Destroy(gameObject);
    }
}
