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

	void Awake() {
		
	}
	
	void Start() {
		gameObject.tag = "saw";
		oscillationDirection = new Vector3(0, -1, 0);
		randomSeed = UnityEngine.Random.Range(0, 100);
		
	}

	void Update() {
		
		if (Time.timeScale > 0) {
			oscillation = Mathf.Sin(Time.time) * oscillationDirection * sawOscillationDistance;
			transform.Rotate(rotation * Time.deltaTime);
			transform.position = transform.position + oscillation;
		}
	}
}