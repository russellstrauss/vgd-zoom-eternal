using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class SawBladeController : MonoBehaviour {
	
	public bool isColliding = false;
	
	Vector3 rotation = new Vector3(0, 800, 0);
	float sawOscillationDistance = .01f;
	Vector3 oscillationDirection;
	Vector3 oscillation;
	float randomSeed;
	GameObject[] blades;
	GameObject player;
	[HideInInspector]
	
	Vector3 displacement = new Vector3(0, 15, 0);
	float sawSpeed = 2.5f;
	Vector3[] bladeStartingPositions;
	int count = 0;
	

	void Awake() {
		
	}
	
	void Start() {
		blades = GameObject.FindGameObjectsWithTag("hazard");
		bladeStartingPositions = new Vector3[blades.Length];
		oscillationDirection = new Vector3(0, -1, 0);
		randomSeed = UnityEngine.Random.Range(0, 100);
		player = GameObject.FindWithTag("Player");
		
		
		
	}

	void Update() {
		
		// float step =  sawSpeed * Time.deltaTime; // calculate distance to move
		// blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, target.position, step);

		// if (Vector3.Distance(transform.position, target.position) < 0.001f)
		// {
		// 	// Swap the position of the cylinder.
		// 	target.position *= -1.0f;
		// }
		
		if (count == 0) {
			for (int i = 0; i < blades.Length; i++) {
				bladeStartingPositions[i] = blades[i].transform.position;
			}
		}
		

		for (int i = 0; i < blades.Length; i++) {
			
			// Gizmos.color = Color.yellow;
			// Gizmos.DrawSphere(transform.position, 1);
			
			
			Vector3 targetPosition = bladeStartingPositions[i] + displacement;
			Debug.DrawRay(bladeStartingPositions[i], displacement, Color.green);
			Debug.DrawLine(bladeStartingPositions[i], targetPosition, Color.red);
			
			blades[i].transform.Rotate(rotation * Time.deltaTime);
			// Debug.Log(isColliding);
			// blades[i].transform.position = targetPosition;
			
			// blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, player.transform.position, sawSpeed * Time.deltaTime);
			blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
			
			if (isColliding) {
				// blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, targetPosition, sawSpeed * Time.deltaTime);
			}
			else {
			}
		}
		count++;
	}
	
	public void TriggerAttack() {
		isColliding = true;
		FindObjectOfType<AudioManager>().Play("crash");
	}
	
	public void ExitAttack() {
		isColliding = false;
	}
}