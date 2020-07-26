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
	Rigidbody hammer;
	GameObject RobotHammerPivot;
	GameObject robotHammer;
	float currentRotation = 0;
	
	EnemyController enemyController;
	
	bool hammering = false;
	
	int count = 0;
	
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

		if (BotSelectorController.selectedBot == "Pecker Wrecker") SetPlayer();
		else if (BotSelectorController.selectedEnemyBot == "Pecker Wrecker") {
			SetEnemy();
		}
		else {
			if (player != null && player == gameObject) SetPlayer();
			else if (enemy != null && enemy == gameObject) SetEnemy();
			else {
				DeactivateBot();
			}
		}
	}
	
	void HammerOn() {
		Debug.Log("Hammer on " + count);
		hammering = true;
		count++;
	}
	
	void HammerOff() {
		hammering = false;
	}
	
	void _hammering() {
		Debug.DrawRay(RobotHammerPivot.transform.position, new Vector3(0, 3, 0), Color.blue);
		Debug.DrawRay(RobotHammerPivot.transform.position, transform.right, Color.green);
		if (currentRotation < (2000 * Time.deltaTime * 12)) {
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, 2000 * Time.deltaTime);
			currentRotation += 2000 * Time.deltaTime;
		}
	}
	
	void _unhammering() {
		if (currentRotation > 0) {
			
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, -2000 * Time.deltaTime);
			currentRotation -= 2000 * Time.deltaTime;
		}
		// if (robotHammer.transform.rotation.x > Math.PI / 6) robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, -1000 * Time.deltaTime);
		
	}
	
	void Update() {
		if (gameObject == player) UpdatePlayerMovement();
		
		// Spin the object around the target at 20 degrees/second.
		if (hammering) _hammering();
		else _unhammering();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {}
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
		gameObject.AddComponent<EnemyController>();
		gameObject.tag = "enemy";
		enemy = gameObject;
	}
	
	public void SetPlayer() {
		gameObject.tag = "Player";
		gameObject.AddComponent<PlayerController>();
		player = gameObject;
		EnablePlayerControls();
		cameraController.SetPlayerFocus();
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