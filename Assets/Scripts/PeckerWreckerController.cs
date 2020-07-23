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
	
	// Win state
	public TextMeshProUGUI winText;
	public TextMeshProUGUI playerHealthLabel;
	GameObject enemyWayPoint;
	int explodeCount = 0;
	TimerCountdownController battleClock;
	
	// bot
	Rigidbody baseRB;
	float healthDefault = 1000f;
	float health = 1000f;
	float botRotationSpeed = 100f;
	float botMovementSpeed = 1000f;
	int gravityMultiplier = 10000;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;
	Rigidbody hammer;
	
	EnemyController enemyController;
	
	bool hammering = true;
	
	int count = 0;
	
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
	
	void HammerOn() {
		Debug.Log("Hammer on " + count);
		// hammer.AddTorque(gameObject.transform.forward * 1000);
		if (hammer != null) hammer.AddForce(gameObject.transform.forward * 2000, ForceMode.Impulse);
		count++;
		
		if (hammering) {
			
			float speed = 1.0f;
			float singleStep = speed * Time.deltaTime;
			Transform target = hammer.transform;
			Vector3 targetDirection = -(target.position - transform.position).normalized;
			Debug.DrawRay(hammer.transform.position, targetDirection * 3, Color.cyan);
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

			// Draw a ray pointing at our target in
			Debug.DrawRay(transform.position, newDirection, Color.white);

			// Calculate a rotation a step closer to the target and applies rotation to this object
			hammer.rotation = Quaternion.LookRotation(newDirection);
		}
	}
	
	void HammerOff() {
		
	}
	
	void Start() {
		Reset();
		mainCamera = GameObject.FindWithTag("MainCamera");
		cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		player = GameObject.FindWithTag("Player");
		baseRB = gameObject.GetComponent<Rigidbody>();
		if (winText != null) winText.enabled = false;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		if (FindObjectsOfType<TimerCountdownController>().Length > 0) battleClock = FindObjectsOfType<TimerCountdownController>()[0];
		if (FindObjectsOfType<EnemyController>().Length > 0) enemyController = FindObjectsOfType<EnemyController>()[0];
		//hammer = GameObject.FindWithTag("hammer").GetComponent<Rigidbody>();
	}
	
	void Update() {
		if (gameObject == player) UpdatePlayerMovement();
		if (gameObject == enemy) updateAIBehavior();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {}
	void OnCollisionExit(Collision otherObjectCollision) {}
	
	public void hideAllLabels() {
		if (winText != null) winText.enabled = false;
	}
	
	void UpdatePlayerMovement() {
		
		if (movementInput.x < -.5 || movementInput.x > .5) player.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed * movementInput.x);

		Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
		Debug.DrawRay(transform.position, direction * 3, Color.red);
		baseRB.AddForce(direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);
		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
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
	}
	
	void TriggerDeathState() {
		Explode();
		EndState();
	}
	
	public void TriggerTimeUpLose() {
		EndState();
	}
	
	public void TriggerTimeUpWin() {
		EndState();
	}
	
	void EndState() {
		Time.timeScale = .1f;
		OrbitalCameraController cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}
	
	void TriggerWinState() {
	}
	
	void Explode() {
		// if (explodeCount < 10) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		if (controls != null && player != null) disableBotControls();
		explodeCount++;
	}
	
	void disableBotControls() {
		Debug.Log("Pecker Wrecker Controls Disabled");
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
	}
	
	void updateAIBehavior() {
		
		Debug.Log("updateAIBehavior");
		
		Debug.DrawRay(transform.position, gameObject.transform.forward * 3, Color.white);
		
		Transform target = player.transform;
		float speed = 1.0f;


		Vector3 targetDirection = (target.position - transform.position).normalized;
		// Debug.DrawRay(gameObject.transform.position, targetDirection * 3, Color.cyan);
		// The step size is equal to speed times frame time.
		float singleStep = speed * Time.deltaTime;

		// Rotate the forward vector towards the target direction by one step
		Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

		// Draw a ray pointing at our target in
		// Debug.DrawRay(transform.position, newDirection, Color.red);

		// Calculate a rotation a step closer to the target and applies rotation to this object
		transform.rotation = Quaternion.LookRotation(newDirection);
		
		// transform.position = Vector3.MoveTowards(transform.position, target.position, 10 * Time.deltaTime);
		
		// baseRB.MovePosition(transform.position + targetDirection *  Time.deltaTime * 100f);
	}
	
	public void SetPlayer() {
		gameObject.tag = "Player";
		player = gameObject;
		EnablePlayerControls();
		cameraController.SetPlayerFocus();
	}
}