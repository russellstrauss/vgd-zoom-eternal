using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeCollider : MonoBehaviour
{
	float sawDamage = 250;
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
		rb.AddForce(new Vector3(0, 1, 0) * 10000f, ForceMode.Impulse);
		if (otherCollision.gameObject.GetComponent<PlayerController>() != null && playerDamageTimer > damageThrottle) {
			otherCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(sawDamage);
			playerDamageTimer = 0f;
		}
		if (otherCollision.gameObject.GetComponent<EnemyController>() != null && enemyDamageTimer > damageThrottle) {
			otherCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(sawDamage);
			enemyDamageTimer = 0f;
		}
	}
}
