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
	private string winTextDefault;
	
	// propeller
	private Boolean propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	private float propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 4f; // exponential
	
	// bot
	private float health = 100f;
	private float botRotationSpeed = 100f;
	private float botMovementSpeed = 25f;
	private Boolean grounded = false;
	private int contactPoints = 0;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	
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
		winTextDefault = winText.text;
		winText.text = "";
		gameObject.tag = "Player";
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
		// Debug.Log("propellerRotationSpeed: " + propellerRotationSpeed + " propellerTimer: " + propellerTimer);
		if (explosion != null) {
			explosion.transform.position = propeller.transform.position;
			explosion.transform.rotation = propeller.transform.rotation;
		}
	}
	
	void OnCollisionStay(Collision collision) {
		
	}
	
	void OnCollisionEnter(Collision otherObject) {
		
		//if (propellerRotationSpeed > propellerMaxSpeed / 2) Instantiate(sparkEffect, otherObject.transform.position, transform.rotation);
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
		
		// UpdateHealth();
	}
	
	void PropellerOff() {
		propellerOn = false;
		propellerTimer = 0;
		count++;
	}
	
	void Reset() {
		health = 100f;
	}
	
	void UpdateHealth() {
		health -= 50;
		if (health < .1) {
			TriggerDeathState();
		}
	}
	
	void TriggerDeathState() {
		winText.text = winTextDefault;
		Explode();
	}
	
	void Explode() {
		explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		controls.Player.Disable();
	}
}