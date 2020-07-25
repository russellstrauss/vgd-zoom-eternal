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
	float healthDefault = 1000f;
	float health = 1000f;
	float botRotationSpeed = 100f;
	float botMovementSpeed = 500f;
	int gravityMultiplier = 10000;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;
	Rigidbody hammer;
	
	EnemyController enemyController;
	
	bool hammering = true;
	
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
		//hammer = GameObject.FindWithTag("hammer").GetComponent<Rigidbody>();

		if (player != null && player == gameObject) SetPlayer();
		else if (enemy != null && enemy == gameObject) SetEnemy();
	}
	
	void HammerOn() {
		Debug.Log("Hammer on " + count);
		// // hammer.AddTorque(gameObject.transform.forward * 1000);
		// if (hammer != null) hammer.AddForce(gameObject.transform.forward * 2000, ForceMode.Impulse);
		// count++;
		
		// if (hammering) {
			
		// 	float speed = 1.0f;
		// 	float singleStep = speed * Time.deltaTime;
		// 	Transform target = hammer.transform;
		// 	Vector3 targetDirection = -(target.position - transform.position).normalized;
		// 	Debug.DrawRay(hammer.transform.position, targetDirection * 3, Color.cyan);
		// 	Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

		// 	// Draw a ray pointing at our target in
		// 	Debug.DrawRay(transform.position, newDirection, Color.white);

		// 	// Calculate a rotation a step closer to the target and applies rotation to this object
		// 	hammer.rotation = Quaternion.LookRotation(newDirection);
		// }
		count++;
	}
	
	void HammerOff() {
		
	}
	
	void Update() {
		if (gameObject == player) UpdatePlayerMovement();
		if (gameObject == enemy) updateAIBehavior();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {}
	void OnCollisionExit(Collision otherObjectCollision) {}
	
	void UpdatePlayerMovement() {
		if (movementInput.x < -.5 || movementInput.x > .5) player.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed * movementInput.x);

		Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
		Debug.DrawRay(transform.position, direction * 3, Color.red);
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
	
	void updateAIBehavior() {
		
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
	}
	
	public void DeactivateBot() {
		if (enemy != gameObject) gameObject.SetActive(false);
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