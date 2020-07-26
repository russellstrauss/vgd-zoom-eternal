using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeWallController : MonoBehaviour
{
	
	float spikeDamage = 100;
	float playerDamageTimer = 0f;
	float enemyDamageTimer = 0f;
	float damageThrottle = 1f; // To prevent many health subtractions from fire repeatedly and instantly killing
	
	void Start() {
		
	}

	void Update() {
		playerDamageTimer += Time.deltaTime;
		enemyDamageTimer += Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision otherCollision) {
		Rigidbody rb = otherCollision.gameObject.GetComponent<Rigidbody>();
		Vector3 contactNormal = otherCollision.contacts[0].normal;
		rb.AddForce(contactNormal * 50000f, ForceMode.Impulse);
		
		if (otherCollision.gameObject.GetComponent<PlayerController>() != null && playerDamageTimer > damageThrottle) {
			otherCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(spikeDamage);
			playerDamageTimer = 0f;
		}
		if (otherCollision.gameObject.GetComponent<EnemyController>() != null && enemyDamageTimer > damageThrottle) {
			otherCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(spikeDamage);
			enemyDamageTimer = 0f;
		}
	}
}
