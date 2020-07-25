using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class SpikeController : MonoBehaviour {
	
	[HideInInspector]
	public bool isColliding = false;
	
	Vector3 oscillationDirection;
	Vector3 oscillation;
	float randomSeed;
	GameObject[] spikesCone;
	GameObject player;
	Rigidbody[] sawRigidbodies;
	
	Vector3 displacement = new Vector3(0, 1.2f, 0);
	float sawSpeed = 5f;
	Vector3[] bladeStartingPositions;
	float sawTimer = 0f;
	float sawActiveTime = 8f;
	private GameObject m_spike_trigger;
	
	// Test vars
	int count = 0;
	
	void Start() {
		player = GameObject.FindWithTag("Player");
		spikesCone = GameObject.FindGameObjectsWithTag("spikeCone");
		m_spike_trigger = GameObject.FindWithTag("spikeTrigger");
		sawRigidbodies = new Rigidbody[spikesCone.Length];
		
		bladeStartingPositions = new Vector3[spikesCone.Length];
		for (int i = 0; i < spikesCone.Length; i++) {
			
			sawRigidbodies[i] = spikesCone[i].GetComponent<Rigidbody>();
			
			bladeStartingPositions[i] = spikesCone[i].transform.position;
			
			Vector3 targetPosition = bladeStartingPositions[i] + displacement;
			// spikesCone[i].transform.position = targetPosition;
			Debug.DrawLine(bladeStartingPositions[i], targetPosition, Color.red);
		}
	}

	void Update() {
		if (m_spike_trigger.GetComponent<spikeTriggerController>().EnteredTrigger)
		{
			sawTimer += Time.deltaTime;
			for (int i = 0; i < spikesCone.Length; i++) {
				Debug.DrawRay(bladeStartingPositions[i], displacement, Color.green);
				Debug.DrawRay(bladeStartingPositions[i], new Vector3(0, .2f, 0), Color.red);
				Debug.DrawRay(bladeStartingPositions[i] + displacement, new Vector3(0, .2f, 0), Color.red);
				
				if (sawTimer < sawActiveTime / 2) {
					Vector3 targetPosition = bladeStartingPositions[i] + displacement;
					// Doesn't move up when timer is reset my player collision. Why?
					spikesCone[i].transform.position = Vector3.MoveTowards(spikesCone[i].transform.position, targetPosition, sawSpeed * Time.deltaTime);
					// Debug.Log("saw moving up, sawTimer=" + sawTimer + ", count=" + count);
					count++;
				}
				else if (sawTimer > sawActiveTime / 2) {
					count = 0;
					spikesCone[i].transform.position = Vector3.MoveTowards(spikesCone[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
				}
				if (sawTimer > sawActiveTime) sawTimer = 0;
			}
		}else{
			for (int i = 0; i < spikesCone.Length; i++) {
				spikesCone[i].transform.position = Vector3.MoveTowards(spikesCone[i].transform.position, bladeStartingPositions[i], sawSpeed * Time.deltaTime);
			}
		}


	}
	
	public void TriggerAttack(Collider otherCollision) {
		sawTimer = 0;
		count = 0;
		isColliding = true;
		// FindObjectOfType<AudioManager>().Play("crash");
	}
	
	public void ExitAttack() {
		isColliding = false;
	}
}