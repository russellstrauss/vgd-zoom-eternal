using System;
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
	
	// propeller
	private Boolean propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	private float propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 5f; // exponential
	
	// bot
	private float healthDefault = 1000f;
	private float health = 1000f;
	private float botRotationSpeed = 200f;
	public float botMovementSpeed = 2000f;
	private float botMovementSpeedDefault = 2000f;
	private Boolean grounded = true;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
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
		propeller = GameObject.Find("Propeller");
		propellerRB = propeller.GetComponent<Rigidbody>();
		baseRB = gameObject.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		if (winText != null) winText.enabled = false;
		if (loseText != null) loseText.enabled = false;
		if (playerHealthLabel != null) {
			playerHealthLabel.text = health.ToString("0");
		}
		gameObject.tag = "Player";
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		
		sparks = gameObject.GetComponentsInChildren<ParticleSystem>();
		hideWheelSparks();
	}
	
	void OnCollisionStay(Collision otherObjectCollision) {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject == enemy) {
			
			float damage = propellerRotationSpeed / 30;
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
		
		updatePropeller();
		updatePlayerMovement();
	}
	
	void showWheelSparks() {
		foreach(ParticleSystem s in sparks) {
			s.Play();
		}
	}
	
	void hideWheelSparks() {
		foreach(ParticleSystem s in sparks) {
			s.Stop();
		}
	}
	
	void updatePlayerMovement() {
		
		upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
		if (movementInput.y > -0.5 && movementInput.y < 0.5 && (movementInput.x < -0.5 || movementInput.x > .5)) gameObject.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);
		if (!upsideDown && grounded && (movementInput.y < -0.5 || movementInput.y > .5)) {
			baseRB.AddForce(transform.forward * botMovementSpeed * movementInput.y, ForceMode.Impulse);
			if (movementInput.y > 1) showWheelSparks();
		}
		else {
			hideWheelSparks();
		}
		// if (upsideDown && propellerOn && propellerRotationSpeed > (propellerMaxSpeed * .2)) {
		// 	// initial blast, then turn off
		// 	baseRB.AddTorque(transform.up * 1000);
		// }
		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}
	
	void updatePropeller() {
		propellerTimer += Time.deltaTime;
		if (propellerRotationSpeed < propellerMaxSpeed && propellerOn) propellerRotationSpeed = (float)Math.Pow(propellerRotationBaseSpeed, propellerTimer);
		else if (!propellerOn) {
			if (propellerRotationSpeed > 0) propellerRotationSpeed -= (propellerTimer * 5);
			else {
				propellerRotationSpeed = 0;
			}
		}
		propeller.transform.Rotate(new Vector3(0, propellerRotationSpeed, 0) * Time.deltaTime);
		if (explosion != null) {
			explosion.transform.position = propeller.transform.position;
			explosion.transform.rotation = propeller.transform.rotation;
		}
		
		if (enemyWayPoint != null) {
			enemyWayPoint.transform.position = gameObject.transform.position;
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
	
	public void setBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}
	
	public void AddHealth(float amount) {
		health += amount;
	}
	
	void TriggerDeathState() {
		loseText.enabled = true;
		Explode();
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