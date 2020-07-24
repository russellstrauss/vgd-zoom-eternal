using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeWallController : MonoBehaviour
{
	
	GameObject player;
	
	void Start() {
		
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision otherCollision) {
		Rigidbody rb = otherCollision.gameObject.GetComponent<Rigidbody>();
		Vector3 contactNormal = otherCollision.contacts[0].normal;
		rb.AddForce(contactNormal * 50000f, ForceMode.Impulse);
		
		if (otherCollision.gameObject.CompareTag("Player") || otherCollision.gameObject.CompareTag("enemy")) {
			
			// if (otherCollision.gameObject.GetComponent<HeliBotController>() != null) otherCollision.gameObject.GetComponent<HeliBotController>().SubtractHealth(100);
			// if (otherCollision.gameObject.GetComponent<EnemyController>() != null) otherCollision.gameObject.GetComponent<>().SubtractHealth(100);
			
		}
	}
}
