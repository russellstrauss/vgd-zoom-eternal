using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HeliBotController : MonoBehaviour
{
	GameObject mainCamera;
	OrbitalCameraController cameraController;
	Vector2 movementInput;
	InputMaster controls;
	Vector3 playerStartPosition;
	Vector3 cameraStartPosition;

	// Win state
	GameObject wayPoint;

	// propeller
	bool propellerOn = false;
	Rigidbody propellerRB;
	Rigidbody baseRB;
	GameObject propeller;
	public float propellerRotationSpeed;
	float propellerMaxSpeed = 3000f;
	float propellerTimer = 0.0f;
	float propellerRotationBaseSpeed = 8f; // exponential
	float propellerBaseDamage = .05f;
	float propellerForceTimer = 0;
	float propellerForceThrottle = 3;

	// bot
	bool driving = false;
	Vector3 direction;
	float healthDefault = 1000f;
	public float health = 1000f;
	float botRotationSpeed = 200f;
	[HideInInspector]
	public float botMovementSpeed = 2000f;
	// float botMovementSpeedDefault = 2000f;
	bool grounded = true;
	int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;
	bool upsideDown = false;
	Renderer particleRenderer;
	ParticleSystem[] sparks;
	bool propellerButtonHeld = false;
	EnemyController enemyController;

	// test vars
	int count = 0;

	void Start() {
		Reset();
		mainCamera = GameObject.FindWithTag("MainCamera");
		cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		
		player = GameObject.FindWithTag("Player");
		baseRB = gameObject.GetComponent<Rigidbody>();
		
		propeller = GameObject.Find("Propeller");
		propellerRB = propeller.GetComponent<Rigidbody>();
		
		wayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		
		if (FindObjectsOfType<EnemyController>().Length > 0) enemyController = FindObjectsOfType<EnemyController>()[0];

		sparks = gameObject.GetComponentsInChildren<ParticleSystem>();
		HideWheelSparks();
		
		if (BotSelectorController.selectedBot == "Stellar Propeller") SetPlayer();
		else if (BotSelectorController.selectedEnemyBot == "Stellar Propeller") {
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

	void OnCollisionStay(Collision otherObjectCollision) {

	}

	void OnCollisionEnter(Collision otherObjectCollision) {

		Rigidbody otherRB = otherObjectCollision.gameObject.GetComponent<Rigidbody>();
		if (otherObjectCollision.gameObject.GetComponent<PlayerController>() != null) {
			otherObjectCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(propellerRotationSpeed * propellerBaseDamage);
		}
		if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
			otherObjectCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(propellerRotationSpeed * propellerBaseDamage);
		}
		if (otherRB != null) {
			
			if (propellerForceTimer > propellerForceThrottle) otherRB.AddForce(transform.right * (propellerRotationSpeed + botMovementSpeed) * 2, ForceMode.Impulse);
			propellerRotationSpeed = 0;
			PropellerOff();
		}

		if (otherObjectCollision.gameObject == floor) {
			grounded = true;
		}
	}

	void OnCollisionExit(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == floor) grounded = false;
		else {
			
		}
	}

	void FixedUpdate() {
		propellerForceTimer += Time.deltaTime;
		
		if (player != null && gameObject == player) {
			UpdatePropeller();
			UpdatePlayerMovement();
		}
	}

	void ShowWheelSparks() {
		if (sparks != null) {
			foreach(ParticleSystem s in sparks) {
				s.Play();
			}
		}
	}

	void HideWheelSparks() {
		if (sparks != null) {
			foreach(ParticleSystem s in sparks) {
				s.Stop();
			}
		}
	}

	void UpdatePlayerMovement() {

		upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
		player.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);

		if (driving) {
			direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, direction * 3, Color.red);
			}
			baseRB.AddForce(direction * botMovementSpeed, ForceMode.Impulse);
			ShowWheelSparks();
		}
		else {
			HideWheelSparks();
		}

		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}

	void UpdatePropeller() {
		
		propellerTimer += Time.deltaTime;
		if (propellerRotationSpeed < propellerMaxSpeed && propellerOn) propellerRotationSpeed = (float)Math.Pow(propellerRotationBaseSpeed, propellerTimer);
		else if (!propellerOn) {
			if (propellerRotationSpeed > 0) propellerRotationSpeed -= (propellerTimer * 5);
			// else {
			// 	propellerRotationSpeed = 0;
			// }
		}
		propeller.transform.Rotate(new Vector3(0, propellerRotationSpeed, 0) * Time.deltaTime);
		if (explosion != null) {
			explosion.transform.position = propeller.transform.position;
			explosion.transform.rotation = propeller.transform.rotation;
		}

		if (wayPoint != null) {
			wayPoint.transform.position = player.transform.position;
		}
	}

	void PropellerOn() {
		propellerButtonHeld = true;
		FindObjectOfType<AudioManager>().Play("propeller-on");
		propellerOn = true;
		propellerTimer = 0;
		count++;
	}

	void PropellerButtonRelease() {
		propellerButtonHeld = false;
		PropellerOff();
	}

	void PropellerOff(String offSound = "propeller-off") {

		FindObjectOfType<AudioManager>().Stop("propeller-on");
		if (propellerRotationSpeed > propellerMaxSpeed * .6) FindObjectOfType<AudioManager>().Play(offSound);
		propellerOn = false;
		propellerTimer = 0;
		count++;
		if (propellerButtonHeld) PropellerOn();
	}

	void Drive() {
		driving = true;
	}

	void DriveRelease() {
		driving = false;
	}

	void Reset() {
		health = healthDefault;
	}

	public void SetBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}

	void disableBotControls() {
		if (controls != null) {
			controls.Player.Move.Disable();
			controls.Player.Select.Disable();
			controls.Player.Drive.Disable();
		}
	}
	
	public void DeactivateBot() {
		gameObject.SetActive(false);
	}
	
	public float GetPropellerSpeed() {
		if (gameObject.GetComponent<Helicopter_AI>() != null) return gameObject.GetComponent<Helicopter_AI>().propellerRotationSpeed;
		else return propellerRotationSpeed;
	}
	
	public void SetEnemy() {
		gameObject.AddComponent<EnemyController>();
		gameObject.AddComponent<Helicopter_AI>();
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
	
	void EnablePlayerControls() {
		
		controls = new InputMaster();
		if (controls != null && gameObject.CompareTag("Player")) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			controls.Player.Select.performed += ctx => PropellerOn();
			controls.Player.Select.canceled += ctx => PropellerButtonRelease();
			controls.Player.Drive.performed += ctx => Drive();
			controls.Player.Drive.canceled += ctx => DriveRelease();
			controls.Player.Enable();
		}
	}
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}
