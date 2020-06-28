﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	
	private const string Tag = "Pick Up";
	public float speed;
	public TextMeshProUGUI countText;
	public TextMeshProUGUI winText;
	private string winTextDefault;
	public float jumpForce; 
	private Vector3 playerStartPosition;
	private Vector3 cameraStartPosition;
	private Rigidbody rb;
	private int count;
	private int jumpCount = 0;
	private GameObject[] gems;
	Vector2 movementInput;
	InputMaster controls;
	private GameObject mainCamera;
	private ParticleSystem particles;
	
	void Awake() {
		controls = new InputMaster();
		if (controls != null) {
			// controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			// controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			controls.Player.Select.performed += ctx => Jump();
		}
	}
	
	void Start()
	{
		particles = gameObject.GetComponent<ParticleSystem>();
		gems = GameObject.FindGameObjectsWithTag("Pick Up");
		mainCamera = GameObject.FindWithTag("MainCamera");
		rb = GetComponent<Rigidbody>();
		count = 0;
		SetCountText();
		playerStartPosition = rb.position;
		cameraStartPosition = mainCamera.transform.position;
		winTextDefault = winText.text;
		winText.text = "";
		gameObject.tag = "Player";
	}
	
	void Jump() {
		if (jumpCount < 2) {
		// 	rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
		// 	FindObjectOfType<AudioManager>().Play("Ting");
			jumpCount++;
		}
	}
	
	void OnEnable() {
		if (controls != null) controls.Player.Enable();
	}
	
	void OnDisable() {
		if (controls != null) controls.Player.Disable();
	}

	void FixedUpdate() {
		
		float fallingVelocity = Physics.gravity.y * (rb.mass * rb.mass) / 10;
		
		Vector3 cameraForward = mainCamera.transform.forward.normalized;
		Vector3 cameraRight = mainCamera.transform.right.normalized;
		cameraForward.y = 0;
		cameraRight.y = 0;
		
		Vector3 movement = cameraForward * movementInput.y + cameraRight * movementInput.x;
		movement.y = fallingVelocity;
		
		if (movement.magnitude < 0) movement = Vector3.zero;
		
		rb.AddForce(movement * speed);
		
		if (transform.position.y < -350) {
			ResetState();
		}
	}

	void OnTriggerEnter(Collider collision) 
	{
		if (collision.gameObject.CompareTag("Pick Up"))
		{
			collision.gameObject.SetActive(false);
			count = count + 1;
			SetCountText();
			FindObjectOfType<AudioManager>().Play("PickUpSound");
		}
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.CompareTag("Floor"))
		{
			jumpCount = 0;
			Vector3 contactPoint = collision.contacts[0].normal;
			rb.AddForce(contactPoint * rb.velocity.magnitude * 5);
		}
		
		if (collision.gameObject.CompareTag("saw")) {
			Vector3 contactPoint = collision.contacts[0].normal;
			rb.AddForce(contactPoint * 3500);
			FindObjectOfType<AudioManager>().Play("crash");
		}
		
		if (collision.gameObject.CompareTag("enemy")) {
			Vector3 contactPoint = collision.contacts[0].normal;
			rb.AddForce(contactPoint * 2500);
		}
	}

	void SetCountText()
	{

		// if (count > gems.Length - 1) {
		// 	winText.text = winTextDefault;
		// }
	}
	
	void ResetState() {
		
		
		winText.text = winTextDefault;
		
		// transform.position = playerStartPosition;
		// count = 0;
		// SetCountText();
		// rb.velocity = Vector3.zero;
		// rb.angularVelocity = Vector3.zero;
		// mainCamera.transform.position = cameraStartPosition;
		// FindObjectOfType<AudioManager>().Play("Death");
		// winText.text = "";
		
		// foreach (var gem in gems)
		// {
		// 	gem.gameObject.SetActive(true);
		// }
	}
}