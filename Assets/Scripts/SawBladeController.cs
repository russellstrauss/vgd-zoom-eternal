using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class SawBladeController : MonoBehaviour {
	
	private Vector3 rotation = new Vector3(0, 800, 0);
	private float sawOscillationDistance = .01f;
	private Vector3 oscillationDirection;
	private Vector3 oscillation;
	private float randomSeed;
	private GameObject[] blades;
	private GameObject player;
	private BoxCollider triggerCollider;

	void Awake() {
		
	}
	
	void Start() {
		blades = GameObject.FindGameObjectsWithTag("hazard");
		oscillationDirection = new Vector3(0, -1, 0);
		randomSeed = UnityEngine.Random.Range(0, 100);
		player = GameObject.FindWithTag("Player");
		triggerCollider = gameObject.GetComponent<BoxCollider>();
	}

	void Update() {
		
		if (Time.timeScale > 0) {
			// oscillation = Mathf.Sin(Time.time * Time.deltaTime) * oscillationDirection * sawOscillationDistance;
			// transform.position = transform.position + oscillation;
			
			foreach (GameObject blade in blades) {
				
				blade.transform.Rotate(rotation * Time.deltaTime);
			}
		}
	}
	
	public void TriggerAttack() {
		Debug.Log("Sawblade Attack");
	}
	
	void OnTriggerEnter(Collider other) {
		
		Debug.Log(other);
		
		if (other.CompareTag("Player")) {
			TriggerAttack();
		}
		
		if (other == player) {
			TriggerAttack();
		}
	}
}