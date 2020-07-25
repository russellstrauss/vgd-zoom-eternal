using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeWallController : MonoBehaviour
{
	
	GameObject player;
	int spikeDamage = 100;
	float playerCollisionTimer = 0;
	float enemyCollisionTimer = 0;
	
	void Start() {
		
	}

	void Update() {
		playerCollisionTimer += Time.deltaTime;
		enemyCollisionTimer += Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision otherCollision) {
		Rigidbody rb = otherCollision.gameObject.GetComponent<Rigidbody>();
		Vector3 contactNormal = otherCollision.contacts[0].normal;
		rb.AddForce(contactNormal * 50000f, ForceMode.Impulse);
		
		if (otherCollision.gameObject.CompareTag("Player") || otherCollision.gameObject.CompareTag("enemy")) {
			otherCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(spikeDamage);
			otherCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(spikeDamage);
		}
	}
}
