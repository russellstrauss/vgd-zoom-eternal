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
		
		if (collision.collider.CompareTag("playerCollider")) {
			Vector3 contactNormal = collision.contacts[0].normal;
			
			playerRb.AddForce(new Vector3(0, 1, 0) * 10000, ForceMode.Impulse);
			FindObjectOfType<AudioManager>().Play("crash");
			// StartCoroutine(Pickup());
		}
	}
	
	
	// Why does this method call itself? Method does not take a parameter, yet calls itself with a parameter? Commenting during merge since it seems like a mistake
    
	// IEnumerator Pickup()
    // {
    // 	Debug.Log("Hazard is being picked");

    // 	if (other.gameObject.CompareTag("playerCollider")){
    // 		// Pickup(other);
    // 		StartCoroutine(Pickup(other));
	// 		GetComponent<TimeStop>().StopTime(0.05f, 10, 0.1f);
	// 	}
    // }
	
	// Old method pre-merge; delete/change as needed
    IEnumerator Pickup(Collider other)
    {
    	GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
    	
    	// TODO add more healthy to the player
		FindObjectOfType<PlayerController>().SubtractHealth(100);

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
