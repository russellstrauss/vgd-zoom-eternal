using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Helicopter_AI : MonoBehaviour
{

	// Win state
	private GameObject wayPoint;
    private Vector3 wayPointPos;
	private int explodeCount = 0;
    private Quaternion _lookRotation;
	private Vector3 _direction;
    private float distance;
	private float trail = 35.0f;

	// propeller
	private bool propellerOn = false;
	private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	private double propellerRotationSpeed;
	private float propellerMaxSpeed = 3000f;
	private float propellerTimer = 0.0f;
	private float propellerRotationBaseSpeed = 8f; // exponential

	// bot
	bool driving = false;
	private float healthDefault = 1000f;
	public float health = 1000f;
	private float botRotationSpeed = 200f;
	public float botMovementSpeed = 1500f;
	// private float botMovementSpeedDefault = 2000f;
	private bool grounded = true;
	private int gravityMultiplier = 40000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	private GameObject player;
	private GameObject enemy;
	private GameObject floor;
    private Transform Target;
	private bool upsideDown = false;
	Renderer particleRenderer;
	private ParticleSystem[] sparks;
	bool propellerButtonHeld = false;

	// test vars
	private int count = 0;


	void Start() {
		Reset();
		player = GameObject.FindWithTag("Player");
		propeller = GameObject.Find("Propeller");
        wayPoint = GameObject.Find("wayPoint");
        if (wayPoint != null && player != null) {
			wayPoint.transform.position = player.transform.position;
		}
		Target = wayPoint.transform;
		propellerRB = propeller.GetComponent<Rigidbody>();
		baseRB = gameObject.GetComponent<Rigidbody>();
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");

		sparks = gameObject.GetComponentsInChildren<ParticleSystem>();
		HideWheelSparks();
	}

	void OnCollisionStay(Collision otherObjectCollision) {

	}

	void OnCollisionEnter(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == player && player != null) {

			float damage = (float)propellerRotationSpeed / 30;
            //add line to subtract player health here
			//enemyController.SubtractHealth(damage);
			//if (enemyController.health < .1) TriggerWinState();
			if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
				PropellerOff("propeller-off-sudden");
				FindObjectOfType<AudioManager>().Stop("propeller-on");
				baseRB.AddForce(transform.up * 3000, ForceMode.Impulse);
				propellerOn = false;
			}
		}

		if (otherObjectCollision.gameObject == floor) grounded = true;
	}

	void OnCollisionExit(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == floor) grounded = false;
	}

	void FixedUpdate() {

		if (player != null) {
			UpdatePropeller();
			UpdateMovement();
		}
	}

	// public void hideAllLabels() {
	// 	if (winText != null) winText.enabled = false;
	// }

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

	void UpdateMovement() {

        if (wayPoint != null) {
			wayPoint.transform.position = player.transform.position;
		}
		if ((propellerTimer < 12 && propellerTimer > 14) || (propellerTimer < 18 && propellerTimer > 20)){
			distance = Vector3.Distance(this.transform.position, wayPoint.transform.position);
	        //we are too close and do not want to be hit
	        wayPointPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
	        // if (distance < trail) {
	        //     //Debug.Log("Should be backing up?");
	        //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, -1 * botMovementSpeed * Time.deltaTime);
	        // } else {
	        //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, botMovementSpeed * Time.deltaTime);
	        // }

	        //Simple behavior, we want to back away while our bot is charging up, and when it is fully charged, charge at the enemy
	          //find the vector pointing from our position to the target
	        _direction = (Target.position - transform.position).normalized;

	        //create the rotation we need to be in to look at the target
	        _lookRotation = Quaternion.LookRotation(_direction);
	        //Debug.Log(_lookRotation);

	        //rotate us over time according to speed until we are in the required rotation
	        //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime *botRotationSpeed);

			upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
			enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, _lookRotation, Time.deltaTime * botRotationSpeed);


			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(_direction, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, _direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, _direction * 3, Color.red);
			}
	        //Debug.Log(botMovementSpeed);
			baseRB.AddForce(_direction * botMovementSpeed / 2, ForceMode.Force);

			baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
		}
		else if (propellerTimer >= 7) {

	        distance = Vector3.Distance(this.transform.position, wayPoint.transform.position);
	        //we are too close and do not want to be hit
	        wayPointPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
	        // if (distance < trail) {
	        //     //Debug.Log("Should be backing up?");
	        //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, -1 * botMovementSpeed * Time.deltaTime);
	        // } else {
	        //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, botMovementSpeed * Time.deltaTime);
	        // }

	        //Simple behavior, we want to back away while our bot is charging up, and when it is fully charged, charge at the enemy
	          //find the vector pointing from our position to the target
	        _direction = (Target.position - transform.position).normalized;

	        //create the rotation we need to be in to look at the target
	        _lookRotation = Quaternion.LookRotation(_direction);
	        //Debug.Log(_lookRotation);

	        //rotate us over time according to speed until we are in the required rotation
	        //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime *botRotationSpeed);

			upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
			enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, _lookRotation, Time.deltaTime * botRotationSpeed);


			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(_direction, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, _direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, _direction * 3, Color.red);
			}
	        //Debug.Log(botMovementSpeed);
			baseRB.AddForce(_direction * botMovementSpeed, ForceMode.Impulse);

			baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
		} else if (propellerTimer < 7 && propellerTimer > 2.5) {
			distance = Vector3.Distance(this.transform.position, wayPoint.transform.position);
		   //we are too close and do not want to be hit
		   wayPointPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
		   // if (distance < trail) {
		   //     //Debug.Log("Should be backing up?");
		   //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, -1 * botMovementSpeed * Time.deltaTime);
		   // } else {
		   //     transform.position = Vector3.MoveTowards(transform.position, wayPointPos, botMovementSpeed * Time.deltaTime);
		   // }

		   //Simple behavior, we want to back away while our bot is charging up, and when it is fully charged, charge at the enemy
			 //find the vector pointing from our position to the target
		   _direction = (Target.position - transform.position).normalized;

		   //create the rotation we need to be in to look at the target
		   _lookRotation = Quaternion.LookRotation(_direction);
		   //Debug.Log(_lookRotation);

		   //rotate us over time according to speed until we are in the required rotation
		   //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime *botRotationSpeed);

		   upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
		   enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, _lookRotation, Time.deltaTime * botRotationSpeed);


		   Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(_direction, new Vector3(0, 1, 0))); // Get forward direction along the ground
		   if (grounded) Debug.DrawRay(transform.position, _direction * 3, Color.green);
		   else {
			   Debug.DrawRay(transform.position, _direction * 3, Color.red);
		   }
		   //Debug.Log(botMovementSpeed);
		   if (distance > trail) {
		   		baseRB.AddForce(_direction * botMovementSpeed, ForceMode.Impulse);
	   		} else {
				baseRB.AddForce(-_direction * botMovementSpeed, ForceMode.Impulse);
			}

		   baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
		}
	}

	void UpdatePropeller() {
        if (!propellerOn && propellerTimer > 3) {
            PropellerOn();
        }
		propellerTimer += Time.deltaTime;
        // Debug.Log(propellerTimer);
		if (propellerRotationSpeed < propellerMaxSpeed && propellerOn) propellerRotationSpeed = (float)Math.Pow(propellerRotationBaseSpeed, propellerTimer);
		else if (!propellerOn) {
			if (propellerRotationSpeed > 0) propellerRotationSpeed -= (propellerTimer * 5);
		}
		propeller.transform.Rotate(new Vector3(0, (float)propellerRotationSpeed, 0) * Time.deltaTime);
		if (explosion != null) {
			explosion.transform.position = propeller.transform.position;
			explosion.transform.rotation = propeller.transform.rotation;
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

		// Debug.Log("propellerButtonHeld=" + propellerButtonHeld);

		if (propellerButtonHeld) PropellerOn();
	}

	void Reset() {
		health = healthDefault;
	}

	// public void SubtractHealth(float amount) {
	// 	health -= amount;
	// 	if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
	// 	if (health < .1) {
	// 		TriggerDeathState();
	// 	}
	// }

	// public void SetBotSpeed(float newSpeed) {
	// 	botMovementSpeed = newSpeed;
	// }

	public void AddHealth(float amount) {
		health += amount;
	}

	// void TriggerDeathState() {
	// 	Explode();
	// 	if (winText != null) {
	// 		winText.text = "YOUR BATTLE BOT HAS BEEN DESTROYED";
	// 		winText.enabled = true;
	// 	}
	// 	EndState();
	// }

	// public void TriggerTimeUpLose() {
	// 	if (winText != null) {
	// 		winText.text = "TIME UP YOU LOST";
	// 		winText.enabled = true;
	// 	}
	// 	EndState();
	// }

	// public void TriggerTimeUpWin() {
	// 	if (winText != null) {
	// 		winText.text = "TIME UP YOU WON";
	// 		winText.enabled = true;
	// 	}
	// 	EndState();
	// }
    //
	// void EndState() {
	// 	Time.timeScale = .1f;
	// 	OrbitalCameraController cameraController = mainCamera.GetComponent<OrbitalCameraController>();
	// 	cameraController.distance = 10f;
	// 	battleClock.StopTimer();
	// }

	// void TriggerWinState() {
	// 	if (winText != null) winText.enabled = true;
	// }

	void Explode() {
		if (explodeCount < 1) explosion = Instantiate(explosionEffect, propeller.transform.position, transform.rotation);
		// if (controls != null) disableBotControls();
		explodeCount++;
	}

	// void disableBotControls() {
	// 	controls.Player.Move.Disable();
	// 	controls.Player.Select.Disable();
	// }
}
