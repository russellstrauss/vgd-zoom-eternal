using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeliBotController : MonoBehaviour
{
	GameObject mainCamera;
	Vector2 movementInput;
	InputMaster controls;
	private Vector3 playerStartPosition;
	private Vector3 cameraStartPosition;
	// propeller
	private Boolean propellerOn = false;
	private Rigidbody rb;
	private GameObject propeller;
	private float propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 4f; // exponential
	// bot
	private float health = 100f;
	private float botRotationSpeed = 100f;
	private float botMovementSpeed = 25f;
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
		rb = propeller.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
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
	}

	void OnCollisionEnter(Collision collision)
	{
		// ContactPoint contact = collision.contacts[0];
		// Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		// Vector3 pos = contact.point;
		
		Vector3 contactPoint = collision.contacts[0].normal;
		// rb.AddForce(contactPoint * 500);
		// FindObjectOfType<AudioManager>().Play("crash");
			
			
		if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Rigidbody>() != null) {
			
			Rigidbody gameObjectRB = collision.gameObject.GetComponent<Rigidbody>();
			gameObjectRB.AddForce(contactPoint * 5000, ForceMode.Impulse);
		}
	}

	void FixedUpdate() {
		
		// Vector3 cameraForward = mainCamera.transform.forward.normalized;
		// Vector3 cameraRight = mainCamera.transform.right.normalized;
		// cameraForward.y = 0;
		// cameraRight.y = 0;
		
		// Vector3 movement = cameraForward * movementInput.y + cameraRight * movementInput.x;
		gameObject.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);
		if (movementInput.y < -0.5 || movementInput.y > .5) transform.position += transform.forward * Time.deltaTime * botMovementSpeed * (movementInput.y / 2);
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
		health = 100f;
	}
}