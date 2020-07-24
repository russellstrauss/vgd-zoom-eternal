using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeCollider : MonoBehaviour
{
	void Start() {
		
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision otherCollision) {
		Rigidbody rb = otherCollision.gameObject.GetComponent<Rigidbody>();
		Vector3 contactNormal = otherCollision.contacts[0].normal;
		rb.AddForce(new Vector3(0, 1, 0) * 10000f, ForceMode.Impulse);
	}
}
