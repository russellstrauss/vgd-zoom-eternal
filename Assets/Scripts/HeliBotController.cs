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
	private string winTextDefault;
	private string loseTextDefault;
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
	public float health = 1000f;
	private float botRotationSpeed = 100f;
	private float botMovementSpeed = 25f;
	private Boolean grounded = false;
	private int contactPoints = 0;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	private GameObject enemy;
	
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
		if (winText != null) {
			winTextDefault = winText.text;
			winText.text = "";
		}
		if (loseText != null) {
			loseTextDefault = loseText.text;
			loseText.text = "";
		}
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString();
		gameObject.tag = "Player";
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
	}
	
	void Update() {
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
	
	void OnCollisionStay(Collision collision) {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject == enemy) {
			
			float damage = propellerRotationSpeed / 40;
			enemy.GetComponent<EnemyController>().SubtractHealth(damage);
			if (enemy.GetComponent<EnemyController>().health < .1) TriggerWinState();
			if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
				baseRB.AddForce(-transform.forward * 2000 * movementInput.y, ForceMode.Impulse);
				propellerOn = false;
			}
		}
	}

	void FixedUpdate() {
	
		if (!(movementInput.y < -0.5 || movementInput.y > .5)) gameObject.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);
		if ((movementInput.y < -0.5 || movementInput.y > .5)) {
			baseRB.AddForce(transform.forward * 2000 * movementInput.y, ForceMode.Impulse);
		}
		baseRB.AddForce(-transform.up * gravityMultiplier, ForceMode.Force);
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
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString();
		if (health < .1) {
			TriggerDeathState();
		}
	}
	
	public void AddHealth(float amount) {
		health += amount;
	}
	
	void TriggerDeathState() {
		loseText.text = loseTextDefault;
		Explode();
	}
	
	void TriggerWinState() {
		winText.text = winTextDefault;
	}
	
	void Explode() {
		if (explodeCount < 10) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		if (controls != null) controls.Player.Disable();
		explodeCount++;
	}
}