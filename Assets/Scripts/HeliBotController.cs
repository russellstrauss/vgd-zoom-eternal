﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HeliBotController : MonoBehaviour
{
	GameObject mainCamera;
	Vector2 movementInput;
	InputMaster controls;
	private Vector3 playerStartPosition;
	private Vector3 cameraStartPosition;
	
	// Win state
	public TextMeshProUGUI winText;
	public TextMeshProUGUI loseText;
	public TextMeshProUGUI playerHealthLabel;
	private GameObject enemyWayPoint;
	private int explodeCount = 0;
	private TimerCountdownController battleClock;
	
	// propeller
	private Boolean propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	private double propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 6f; // exponential
	
	// bot
	private float healthDefault = 1000f;
	public float health = 1000f;
	private float botRotationSpeed = 200f;
	public float botMovementSpeed = 2000f;
	// private float botMovementSpeedDefault = 2000f;
	private Boolean grounded = true;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	private GameObject player;
	private GameObject enemy;
	private GameObject floor;
	private Boolean upsideDown = false;
	private ParticleSystem[] sparks;
	
	// test vars
	private int count = 0;
	
	void Awake() {
		controls = new InputMaster();
		if (controls != null) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			
			controls.Player.Select.performed += ctx => PropellerOn();
			controls.Player.Select.canceled += ctx => PropellerOff();
		}
	}
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
	
	void Start() {
		Reset();
		gameObject.tag = "Player";
		player = gameObject;
		propeller = GameObject.Find("Propeller");
		propellerRB = propeller.GetComponent<Rigidbody>();
		baseRB = player.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		if (winText != null) winText.enabled = false;
		if (loseText != null) loseText.enabled = false;
		if (playerHealthLabel != null) {
			playerHealthLabel.text = health.ToString("0");
		}
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		battleClock = FindObjectsOfType<TimerCountdownController>()[0];
		
		sparks = player.GetComponentsInChildren<ParticleSystem>();
		HideWheelSparks();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject == enemy) {
			
			float damage = (float)propellerRotationSpeed / 30;
			enemy.GetComponent<EnemyController>().SubtractHealth(damage);
			if (enemy.GetComponent<EnemyController>().health < .1) TriggerWinState();
			if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
				baseRB.AddForce(transform.up * 2000 * movementInput.y, ForceMode.Impulse);
				propellerOn = false;
			}
		}
		
		if (otherObjectCollision.gameObject == floor) grounded = true;
	}
	
	void OnCollisionExit(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject == floor) grounded = false;
	}

	void FixedUpdate() {
		
		// Debug.Log(floor.transform.position.y);
		// Debug.Log(player.transform.position.y);
		
		UpdatePropeller();
		UpdatePlayerMovement();
	}
	
	public void hideAllLabels() {
		Debug.Log("Label hide attempt");
		if (winText != null) winText.enabled = false;
		if (loseText != null) loseText.enabled = false;
	}
	
	void ShowWheelSparks() {
		foreach(ParticleSystem s in sparks) {
			s.Play();
		}
	}
	
	void HideWheelSparks() {
		foreach(ParticleSystem s in sparks) {
			s.Stop();
		}
	}
	
	void UpdatePlayerMovement() {
		
		upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
		if (movementInput.y > -0.5 && movementInput.y < 0.5 && (movementInput.x < -0.5 || movementInput.x > .5)) player.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);
		if (!upsideDown && (movementInput.y < -0.5 || movementInput.y > .5)) {
			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, direction * 3, Color.red);
			}
			baseRB.AddForce(direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);
			if (movementInput.y > 1) ShowWheelSparks();
		}
		else {
			HideWheelSparks();
		}
		// if (upsideDown && propellerOn && propellerRotationSpeed > (propellerMaxSpeed * .2)) {
		// 	// initial blast, then turn off
		// 	baseRB.AddTorque(transform.up * 1000);
		// }
		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}
	
	void UpdatePropeller() {
		propellerTimer += Time.deltaTime;
		if (propellerRotationSpeed < propellerMaxSpeed && propellerOn) propellerRotationSpeed = (float)Math.Pow(propellerRotationBaseSpeed, propellerTimer);
		else if (!propellerOn) {
			if (propellerRotationSpeed > 0) propellerRotationSpeed -= (propellerTimer * 5);
			else {
				propellerRotationSpeed = 0;
			}
		}
		propeller.transform.Rotate(new Vector3(0, (float)propellerRotationSpeed, 0) * Time.deltaTime);
		if (explosion != null) {
			explosion.transform.position = propeller.transform.position;
			explosion.transform.rotation = propeller.transform.rotation;
		}
		
		if (enemyWayPoint != null) {
			enemyWayPoint.transform.position = player.transform.position;
		}
	}
	
	void PropellerOn() {
		propellerOn = true;
		propellerTimer = 0;
		count++;
	}
	
	void PropellerOff() {
		propellerOn = false;
		propellerTimer = 0;
		count++;
	}
	
	void Reset() {
		health = healthDefault;
	}
	
	public void SubtractHealth(float amount) {
		health -= amount;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		if (health < .1) {
			TriggerDeathState();
		}
	}
	
	public void SetBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}
	
	public void AddHealth(float amount) {
		health += amount;
		battleClock.StopTimer();
	}
	
	void TriggerDeathState() {
		Explode();
		winText.text = "YOUR BATTLE BOT HAS BEEN DESTROYED";
		winText.enabled = true;
		endState();
	}
	
	public void TriggerTimeUpLose() {
		winText.text = "TIME UP YOU LOST";
		winText.enabled = true;
		endState();
	}
	
	public void TriggerTimeUpWin() {
		winText.text = "TIME UP YOU WON";
		winText.enabled = true;
		endState();
	}
	
	void endState() {
		Time.timeScale = .1f;
		OrbitalCameraController cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}
	
	void TriggerWinState() {
		winText.enabled = true;
	}
	
	void Explode() {
		if (explodeCount < 10) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		if (controls != null) controls.Player.Disable();
		explodeCount++;
	}
}