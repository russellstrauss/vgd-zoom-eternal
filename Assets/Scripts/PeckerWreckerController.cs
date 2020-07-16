using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PeckerWreckerController : MonoBehaviour
{
	GameObject mainCamera;
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
	
	// bot
	private Rigidbody baseRB;
	private float healthDefault = 1000f;
	public float health = 1000f;
	private float botRotationSpeed = 200f;
	public float botMovementSpeed = 1500f;
	private int gravityMultiplier = 40000;
	private GameObject explosion;
	private GameObject player;
	private GameObject enemy;
	private GameObject floor;
	private ParticleSystem[] sparks;
	
	// test vars
	private int count = 0;
	EnemyController enemyController;
	
	void Awake() {
		controls = new InputMaster();
		if (controls != null) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			
			//controls.Player.Select.performed += ctx => Strike();
		}
	}
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
	
	void Start() {
		Reset();
		gameObject.tag = "Player";
		player = gameObject;
		baseRB = player.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		if (winText != null) winText.enabled = false;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		if (FindObjectsOfType<TimerCountdownController>().Length > 0) battleClock = FindObjectsOfType<TimerCountdownController>()[0];
		if (FindObjectsOfType<EnemyController>().Length > 0) enemyController = FindObjectsOfType<EnemyController>()[0];
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject == enemy) {
			
			// float damage = (float)propellerRotationSpeed / 30;
			// enemy.GetComponent<EnemyController>().SubtractHealth(damage);
			// if (enemy.GetComponent<EnemyController>().health < .1) TriggerWinState();
		}
	}
	
	void OnCollisionExit(Collision otherObjectCollision) {
		
	}
	
	void FixedUpdate() {
		
		Debug.Log("Update");
		UpdatePlayerMovement();
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
		
		Debug.Log(movementInput);
		if (movementInput.x < -0.75 || movementInput.x > 0.75) player.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);
		if (movementInput.y < -0.5 || movementInput.y > .5) {
			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
			Debug.DrawRay(transform.position, direction * 3, Color.red);
			baseRB.AddForce(-direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);
		}

		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}
	
	void Strike() {
		// Debug.Log(gameObject.Find("Hammer"));
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
		winText.enabled = true;
		EndState("YOUR BATTLE BOT HAS BEEN DESTROYED");
	}
	
	public void TriggerTimeUpLose() {
		winText.enabled = true;
		EndState("TIME UP YOU LOST");
	}
	
	public void TriggerTimeUpWin() {
		winText.enabled = true;
		EndState("TIME UP YOU WON");
	}
	
	void EndState(String message) {
		winText.text = message;
		winText.enabled = true;
		Time.timeScale = .1f;
		OrbitalCameraController cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		cameraController.distance = 10f;
		battleClock.StopTimer();
		// Debug.Log("timeScale=" + Time.timeScale);
	}
	
	void TriggerWinState() {
		winText.enabled = true;
	}
	
	void Explode() {
		// if (explodeCount < 10) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		if (controls != null) disableBotControls();
		explodeCount++;
	}
	
	void disableBotControls() {
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
	}
}