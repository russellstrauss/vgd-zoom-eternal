using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour {
	
	private int collisionCount;
	
	void Start() {
		collisionCount = 0;
		gameObject.tag = "enemy";
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		
		if (collision.gameObject.CompareTag("Player")) {
			
			collisionCount++;
			//Vector3 contactPoint = collision.contacts[0].normal;
			//rb.AddForce(contactPoint * 500);
			FindObjectOfType<AudioManager>().Play("crash");
			
			if (collisionCount > 4) gameObject.SetActive(false);
		}
	}
}