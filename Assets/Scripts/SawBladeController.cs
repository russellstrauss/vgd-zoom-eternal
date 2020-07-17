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
	
	Vector3 displacement = new Vector3(0, 5, 0);
	float sawSpeed = 2.5f;
	Vector3[] bladeStartingPositions;
	int count = 0;
	// create timer for saw
	float sawTimer = 0f;

	void Awake() {
		
	}
	
	void Start() {
		player = GameObject.FindWithTag("Player");
		blades = GameObject.FindGameObjectsWithTag("hazard");
		bladeStartingPositions = new Vector3[blades.Length];
		for (int i = 0; i < blades.Length; i++) {
			bladeStartingPositions[i] = blades[i].transform.position;
			
			Vector3 targetPosition = bladeStartingPositions[i] + displacement;
			// blades[i].transform.position = targetPosition;
			Debug.DrawLine(bladeStartingPositions[i], targetPosition, Color.red);
		}
	}

	void Update() {
		
		sawTimer += Time.deltaTime;

		// Debug.Log(sawTimer);

		for (int i = 0; i < blades.Length; i++) {
			
			blades[i].transform.Rotate(rotation * Time.deltaTime);
			
			Debug.DrawRay(bladeStartingPositions[i], new Vector3(0, .1f, 0), Color.yellow);
			
			
			if (sawTimer < 2) {
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, bladeStartingPositions[i] + displacement, sawSpeed * Time.deltaTime);
				// blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, targetPosition, sawSpeed * Time.deltaTime);
			}
			if(sawTimer > 2) {
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
			}
		}
		count++;
	}
	
	public void TriggerAttack() {
		sawTimer = 0;
		isColliding = true;
		FindObjectOfType<AudioManager>().Play("crash");
	}
	
	public void ExitAttack() {
		isColliding = false;
	}
}