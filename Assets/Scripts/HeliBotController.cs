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
	private Vector3 playerStartPosition;
	private Vector3 cameraStartPosition;

	// Win state
	private GameObject wayPoint;

	// propeller
	private bool propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	public float propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 8f; // exponential

	// bot
	bool driving = false;
	private float healthDefault = 1000f;
	public float health = 1000f;
	private float botRotationSpeed = 200f;
	public float botMovementSpeed = 2000f;
	// private float botMovementSpeedDefault = 2000f;
	private bool grounded = true;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	private GameObject player;
	private GameObject enemy;
	private GameObject floor;
	private bool upsideDown = false;
	Renderer particleRenderer;
	private ParticleSystem[] sparks;
	bool propellerButtonHeld = false;
	EnemyController enemyController;

	// test vars
	private int count = 0;

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

		if (otherObjectCollision.gameObject == enemy && enemy != null && enemyController != null) {

			float damage = propellerRotationSpeed / 30;
			enemyController.SubtractHealth(damage);
			// if (enemyController.health < .1) TriggerWinState();
			if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
				PropellerOff("propeller-off-sudden");
				FindObjectOfType<AudioManager>().Stop("propeller-on");
				baseRB.AddForce(transform.up * 2000 * movementInput.y, ForceMode.Impulse);
				propellerOn = false;
			}
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
			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
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
		
		Debug.Log(propellerRotationSpeed);
		
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
