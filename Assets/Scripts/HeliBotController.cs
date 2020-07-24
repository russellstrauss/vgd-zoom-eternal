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
	public TextMeshProUGUI winText;
	public TextMeshProUGUI playerHealthLabel;
	private GameObject enemyWayPoint;
	private int explodeCount = 0;
	private TimerCountdownController battleClock;

	// propeller
	private bool propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	public double propellerRotationSpeed;
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

	// test vars
	private int count = 0;
	EnemyController enemyController;

	void Start() {
		Reset();
		mainCamera = GameObject.FindWithTag("MainCamera");
		cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		
		player = GameObject.FindWithTag("Player");
		baseRB = gameObject.GetComponent<Rigidbody>();
		
		propeller = GameObject.Find("Propeller");
		propellerRB = propeller.GetComponent<Rigidbody>();
		
		if (winText != null) winText.enabled = false;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		if (FindObjectsOfType<TimerCountdownController>().Length > 0) battleClock = FindObjectsOfType<TimerCountdownController>()[0];
		if (FindObjectsOfType<EnemyController>().Length > 0) enemyController = FindObjectsOfType<EnemyController>()[0];

		sparks = gameObject.GetComponentsInChildren<ParticleSystem>();
		HideWheelSparks();
		
		if (player != null && player == gameObject) SetPlayer();
		else if (enemy != null && enemy == gameObject) SetEnemy();
		else gameObject.SetActive(false);
	}

	void OnCollisionStay(Collision otherObjectCollision) {

	}

	void OnCollisionEnter(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == enemy && enemy != null && enemyController != null) {

			float damage = (float)propellerRotationSpeed / 30;
			enemyController.SubtractHealth(damage);
			if (enemyController.health < .1) TriggerWinState();
			if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
				PropellerOff("propeller-off-sudden");
				FindObjectOfType<AudioManager>().Stop("propeller-on");
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

		if (player != null && gameObject == player) {
			UpdatePropeller();
			UpdatePlayerMovement();
		}
		if (enemy != null && gameObject == enemy) updateAIBehavior();
	}

	public void hideAllLabels() {
		if (winText != null) winText.enabled = false;
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
	
	void updateAIBehavior() {
		Debug.Log("updateAIBehavior on HeliBot");
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
		if (winText != null) {
			winText.text = "YOUR BATTLE BOT HAS BEEN DESTROYED";
			winText.enabled = true;
		}
		EndState();
	}

	public void TriggerTimeUpLose() {
		if (winText != null) {
			winText.text = "TIME UP YOU LOST";
			winText.enabled = true;
		}
		EndState();
	}

	public void TriggerTimeUpWin() {
		if (winText != null) {
			winText.text = "TIME UP YOU WON";
			winText.enabled = true;
		}
		EndState();
	}

	void EndState() {
		Time.timeScale = .1f;
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}

	void TriggerWinState() {
		if (winText != null) winText.enabled = true;
	}

	void Explode() {
		if (explodeCount < 1) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		if (controls != null) disableBotControls();
		explodeCount++;
	}

	void disableBotControls() {
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
		controls.Player.Drive.Disable();
	}
	
	public void SetEnemy() {
		gameObject.tag = "enemy";
		enemy = gameObject;
	}
	
	public void SetPlayer() {
		Debug.Log("SetPlayer HeliBot");
		gameObject.tag = "Player";
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
