using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using TMPro;

public class SawBotController : MonoBehaviour
{
	GameObject mainCamera;
	Vector2 movementInput;
	InputMaster controls;
	Vector3 playerStartPosition;
	Vector3 cameraStartPosition;
	
	// Win state
	public TextMeshProUGUI playerHealthLabel;
	GameObject enemyWayPoint;
	int explodeCount = 0;
	TimerCountdownController battleClock;
	
	// bot
	Rigidbody baseRB;
	float healthDefault = 1000f;
	float health = 1000f;
	float botRotationSpeed = 200f;
	float botMovementSpeed = 300f;
	int gravityMultiplier = 10000;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;
	Rigidbody hammer;
	
	EnemyController enemyController;
	
	void Awake() {
		controls = new InputMaster();
		if (controls != null && gameObject.CompareTag("Player")) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			
			controls.Player.Select.performed += ctx => HammerOn();
			controls.Player.Select.canceled += ctx => HammerOff();
		}
	}
	
	void HammerOn() {
		// hammer.AddTorque(-gameObject.transform.forward * 1000);
	}
	
	void HammerOff() {
		
	}
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
	
	void Start() {
		Reset();
		if (gameObject.CompareTag("Player")) player = gameObject;
		else {
			player = GameObject.FindWithTag("Player");
		}
		baseRB = gameObject.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		battleClock = FindObjectOfType<TimerCountdownController>();
		enemyController = FindObjectOfType<EnemyController>();
	}
	
	void Update() {
		if (gameObject == player) UpdatePlayerMovement();
		// if (gameObject == enemy) updateAIBehavior();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {}
	void OnCollisionExit(Collision otherObjectCollision) {}
	
	void UpdatePlayerMovement() {
		
		player.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);

		Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
		
		// Debug.DrawRay(transform.position, direction * 3, Color.red);
		baseRB.AddForce(-direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);

		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}
	
	void Reset() {
		health = healthDefault;
	}
	
	public void SetBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}
	
	void disableBotControls() {
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
	}
}
