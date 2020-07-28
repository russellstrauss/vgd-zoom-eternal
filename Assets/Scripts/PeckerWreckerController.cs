using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PeckerWreckerController : MonoBehaviour
{
	GameObject mainCamera;
	OrbitalCameraController cameraController;
	Vector2 movementInput;
	InputMaster controls;
	Vector3 playerStartPosition;
	Vector3 cameraStartPosition;
	
	GameObject enemyWayPoint;
	
	// bot
	Rigidbody baseRB;
	float botRotationSpeed = 100f;
	float botMovementSpeed = 500f;
	int gravityMultiplier = 10000;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;
	
	GameObject robotHammer;
	GameObject RobotHammerPivot;
	float hammerSpeed = 2000;
	float currentRotation = 0;
	
	EnemyController enemyController;
	
	bool hammering = false;
	
	void Start() {
		mainCamera = GameObject.FindWithTag("MainCamera");
		cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		
		player = GameObject.FindWithTag("Player");
		baseRB = gameObject.GetComponent<Rigidbody>();
		
		enemy = GameObject.FindWithTag("enemy");
		enemyWayPoint = GameObject.Find("wayPoint");
		floor = GameObject.FindWithTag("Floor");
		enemyController = FindObjectOfType<EnemyController>();
		RobotHammerPivot = gameObject.GetComponentInChildren<RobotHammerPivot>().gameObject;
		robotHammer = gameObject.GetComponentInChildren<RobotHammer>().gameObject;
		InitBot();
	}
	public void InitBot() {
		Debug.Log("Pecker Wrecker InitBot()");
		if (BotSelectorController.selectedBot == "Stellar Propeller") SetEnemy(); // testing remove
		
		if (BotSelectorController.selectedBot == "Pecker Wrecker") SetPlayer();
		else if (BotSelectorController.selectedEnemyBot == "Pecker Wrecker") {
			SetEnemy();
		}
		else { // If manually selected in scene
			if (player != null && player == gameObject) SetPlayer();
			else if (enemy != null && enemy == gameObject) SetEnemy();
			else {
				DeactivateBot();
			}
		}
	}
	
	void HammerOn() {
		hammering = true;
	}
	
	void HammerOff() {
		hammering = false;
	}
	
	
	void _hammering() {
		
		float hammerRotationLimit = 90f;
		
		if (currentRotation < hammerRotationLimit) {
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, hammerSpeed * Time.deltaTime);
			currentRotation += hammerSpeed * Time.deltaTime;
		}
	}
	
	void _unhammering() {
		
		if (currentRotation > 0) {	
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, -hammerSpeed * Time.deltaTime);
			currentRotation -= hammerSpeed * Time.deltaTime;
		}
	}
	
	void Update() {
		if (gameObject == player) UpdatePlayerMovement();
		
		// Spin the object around the target at 20 degrees/second.
		if (hammering) _hammering();
		else _unhammering();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {
		// float hammerHeadDamage = 250f;
		float hammerHeadDamage = 6f;
		if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
			otherObjectCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(hammerHeadDamage);
		}
		
		if (otherObjectCollision.gameObject.GetComponent<PlayerController>() != null) {
			otherObjectCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(hammerHeadDamage);
		}
	}
	void OnCollisionExit(Collision otherObjectCollision) {}
	
	void UpdatePlayerMovement() {
		if (movementInput.x < -.5 || movementInput.x > .5) player.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed * movementInput.x);

		Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
		// Debug.DrawRay(transform.position, direction * 3, Color.red);
		baseRB.AddForce(direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);
		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}
	
	public void SetBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}
	
	void disableBotControls() {
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
	}
	
	public void SetEnemy() {
		Debug.Log("PeckerWrecker SetEnemy()");
		gameObject.AddComponent<EnemyController>();
		gameObject.AddComponent<PeckerWreckerAi>();
		gameObject.tag = "enemy";
		enemy = gameObject;
	}
	
	public void SetPlayer() {
		gameObject.tag = "Player";
		gameObject.AddComponent<PlayerController>();
		player = gameObject;
		EnablePlayerControls();
		if (cameraController != null) cameraController.SetPlayerFocus();
	}
	
	public void DeactivateBot() {
		gameObject.SetActive(false);
	}
	
	void EnablePlayerControls() {
		controls = new InputMaster();
		if (controls != null) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			controls.Player.Select.performed += ctx => HammerOn();
			controls.Player.Select.canceled += ctx => HammerOff();
			controls.Player.Enable();
		}
	}
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}