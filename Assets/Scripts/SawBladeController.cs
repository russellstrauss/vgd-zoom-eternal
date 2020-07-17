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
	
	Vector3 displacement = new Vector3(0, 4f, 0);
	float sawSpeed = 5f;
	Vector3[] bladeStartingPositions;
	float sawTimer = 0f;
	float sawActiveTime = 8f;
	
	// Test vars
	int count = 0;
	
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

		for (int i = 0; i < blades.Length; i++) {
			
			blades[i].transform.Rotate(rotation * Time.deltaTime);
			
			Debug.DrawRay(bladeStartingPositions[i], displacement, Color.green);
			Debug.DrawRay(bladeStartingPositions[i], new Vector3(0, .2f, 0), Color.red);
			Debug.DrawRay(bladeStartingPositions[i] + displacement, new Vector3(0, .2f, 0), Color.red);
			
			if (sawTimer < sawActiveTime / 2) {
				Vector3 targetPosition = bladeStartingPositions[i] + displacement;
				// Doesn't move up when timer is reset my player collision. Why?
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, targetPosition, sawSpeed * Time.deltaTime);
				// Debug.Log("saw moving up, sawTimer=" + sawTimer + ", count=" + count);
				count++;
			}
			else if (sawTimer > sawActiveTime / 2) {
				count = 0;
				blades[i].transform.position = Vector3.MoveTowards(blades[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
			}
			if (sawTimer > sawActiveTime) sawTimer = 0;
		}
	}
	
	public void TriggerAttack() {
		sawTimer = 0;
		count = 0;
		isColliding = true;
		FindObjectOfType<AudioManager>().Play("crash");
	}
	
	public void ExitAttack() {
		isColliding = false;
	}
}