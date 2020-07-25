using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class SawBladeController : MonoBehaviour {
	
	[HideInInspector]
	public bool isColliding = false;
	
	Vector3 rotation = new Vector3(0, 800, 0);
	GameObject[] blades;
	GameObject player;
	Rigidbody[] sawRigidbodies;
	
	Vector3 displacement = new Vector3(0, 4f, 0);
	float sawSpeed = 5f;
	Vector3[] bladeStartingPositions;
	float sawTimer = 0f;
	float sawActiveTime = 8f;
	
	// Test vars
	int count = 0;
	
	void Start() {
		player = GameObject.FindWithTag("Player");
		blades = GameObject.FindGameObjectsWithTag("hazard");
		sawRigidbodies = new Rigidbody[blades.Length];
		
		bladeStartingPositions = new Vector3[blades.Length];
		for (int i = 0; i < blades.Length; i++) {
			
			sawRigidbodies[i] = blades[i].GetComponent<Rigidbody>();
			
			bladeStartingPositions[i] = blades[i].transform.position;
			
			Vector3 targetPosition = bladeStartingPositions[i] + displacement;
		}
	}

	void Update() {
		
		sawTimer += Time.deltaTime;

		for (int i = 0; i < blades.Length; i++) {
			
			blades[i].transform.Rotate(rotation * Time.deltaTime);
			
			if (sawTimer < sawActiveTime / 2) {
				Vector3 targetPosition = bladeStartingPositions[i] + displacement;
				// Doesn't move up when timer is reset my player collision. Why?
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, targetPosition, sawSpeed * Time.deltaTime);
				count++;
			}
			else if (sawTimer > sawActiveTime / 2) {
				count = 0;
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
			}
			if (sawTimer > sawActiveTime) sawTimer = 0;
		}
	}
	
	public void TriggerAttack(Collider otherCollision) {
		sawTimer = 0;
		count = 0;
		isColliding = true;
	}
	
	public void ExitAttack() {
		isColliding = false;
	}
}