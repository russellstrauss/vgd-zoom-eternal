using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerController : MonoBehaviour
{	
	private GameObject enemy;
	private Rigidbody enemyRB;
	
	void Start() {
		enemy = GameObject.FindWithTag("enemy");
		enemyRB = enemy.GetComponent<Rigidbody>();
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		ContactPoint contact = otherObjectCollision.contacts[0];
		enemyRB.AddForce(contact.normal * 100000, ForceMode.Impulse);
	}
}