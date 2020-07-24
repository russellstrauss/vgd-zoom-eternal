using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeCollider : MonoBehaviour
{
	
	int sawBladeDamage = 250;
	int throwbackForce = 10000;
	
	void Start() {
		
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision otherCollision) {
		Rigidbody rb = otherCollision.gameObject.GetComponent<Rigidbody>();
		Vector3 contactNormal = otherCollision.contacts[0].normal;
		rb.AddForce(new Vector3(0, 1, 0) * throwbackForce, ForceMode.Impulse);
		
		if (otherCollision.gameObject.GetComponent<PlayerController>() != null) otherCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(sawBladeDamage);
		if (otherCollision.gameObject.GetComponent<EnemyController>() != null) otherCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(sawBladeDamage);
	}
}
