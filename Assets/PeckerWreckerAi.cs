using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeckerWreckerAi : MonoBehaviour
{
	Vector3 playerStartPosition;

    private GameObject wayPoint;
    private Vector3 wayPointPos;
    private Quaternion _lookRotation;
	private Vector3 _direction;
    private float distance;
	private float trail = 25.0f;
	// bot
	Rigidbody baseRB;
	float botRotationSpeed = 100f;
	float botMovementSpeed = 500f;
	int gravityMultiplier = 10000;
	GameObject explosion;
	GameObject player;
	GameObject enemy;
	GameObject floor;

	GameObject robotHammer;
	GameObject robotHammerHead;
	GameObject RobotHammerPivot;
	Rigidbody robotHammerHeadRB;
	float hammerSpeed = 2000;
	float currentRotation = 0;
    private float timer = 0.0f;
    private float hammerTimer = 0.0f;
    private Transform Target;
    private bool grounded = true;

	EnemyController enemyController;

	bool hammering = false;

	void Start() {

		player = GameObject.FindWithTag("Player");
		baseRB = gameObject.GetComponent<Rigidbody>();

		enemy = GameObject.FindWithTag("enemy");
		wayPoint = GameObject.Find("wayPoint");
		floor = GameObject.FindWithTag("Floor");
		enemyController = FindObjectOfType<EnemyController>();
		RobotHammerPivot = gameObject.GetComponentInChildren<RobotHammerPivot>().gameObject;
		robotHammer = gameObject.GetComponentInChildren<RobotHammer>().gameObject;
        if (wayPoint != null && player != null) {
			wayPoint.transform.position = player.transform.position;
		}
		Target = wayPoint.transform;
		// robotHammerHead = gameObject.GetComponentInChildren<RobotHammerHead>().gameObject;
		// robotHammerHeadRB = robotHammerHead.GetComponent<Rigidbody>();

		// if (BotSelectorController.selectedBot == "Pecker Wrecker") SetPlayer();
		// else if (BotSelectorController.selectedEnemyBot == "Pecker Wrecker") {
		// 	SetEnemy();
		// }
		// else {
		// 	if (player != null && player == gameObject) SetPlayer();
		// 	else if (enemy != null && enemy == gameObject) SetEnemy();
		// 	else {
		// 		DeactivateBot();
		// 	}
		// }
	}

	void HammerOn() {
		hammering = true;
        Debug.Log("hammering");
	}

	void HammerOff() {
		hammering = false;
	}


	void _hammering() {

		float hammerRotationLimit = 90f;

		if (currentRotation < hammerRotationLimit) {
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, hammerSpeed * Time.deltaTime);
			currentRotation += hammerSpeed * Time.deltaTime;
		}
	}

	void _unhammering() {

		if (currentRotation > 0) {
			robotHammer.transform.RotateAround(RobotHammerPivot.transform.position, transform.right, -hammerSpeed * Time.deltaTime);
			currentRotation -= hammerSpeed * Time.deltaTime;
		}
	}

	void Update() {
		UpdatePlayerMovement();
        timer += Time.deltaTime;
        if (hammering || hammerTimer < 0){
            hammerTimer += Time.deltaTime;
            if (hammerTimer > 0.5f) {
                HammerOff();
                hammerTimer = -0.5f;
                Debug.Log("turning hammer off");
            }
        }

		// Spin the object around the target at 20 degrees/second.
		if (hammering) _hammering();
		else _unhammering();
	}

	void OnCollisionStay(Collision otherObjectCollision) {}
	void OnCollisionEnter(Collision otherObjectCollision) {
		// float hammerHeadDamage = 250f;
		float hammerHeadDamage = 1f;
		if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
			otherObjectCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(hammerHeadDamage);
		}

		if (otherObjectCollision.gameObject.GetComponent<PlayerController>() != null) {
			otherObjectCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(hammerHeadDamage);
		}
        if (otherObjectCollision.gameObject == floor) grounded = true;
	}
	void OnCollisionExit(Collision otherObjectCollision) {
        if (otherObjectCollision.gameObject == floor) grounded = false;
    }

	void UpdatePlayerMovement() {
        if (wayPoint != null) {
			wayPoint.transform.position = player.transform.position;
		}
        if (timer > 25) {
            timer = 0;
        }
		if ((timer < 12 && timer > 14) || (timer < 18 && timer > 21)){
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
            if (distance < 6 && !hammering && hammerTimer >= 0) {
                HammerOn();
            }
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
		else if (timer >= 3) {

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
	        //Debug.Log(distance);
            if (distance < 6 && !hammering && hammerTimer >= 0) {
                HammerOn();
                timer = 1;
            }
	        //rotate us over time according to speed until we are in the required rotation
	        //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime *botRotationSpeed);

			enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, _lookRotation, Time.deltaTime * botRotationSpeed);


			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(_direction, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, _direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, _direction * 3, Color.red);
			}
	        //Debug.Log(botMovementSpeed);
			baseRB.AddForce(_direction * botMovementSpeed, ForceMode.Impulse);

			baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
		} else if (timer < 3 && timer >= 0.3f) {
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
           if (distance < 6 && !hammering && hammerTimer >= 0) {
               HammerOn();
           }
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
		// if (movementInput.x < -.5 || movementInput.x > .5) player.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed * movementInput.x);
        //
		// Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0))); // Get forward direction along the ground
		// // Debug.DrawRay(transform.position, direction * 3, Color.red);
		// baseRB.AddForce(direction * botMovementSpeed * movementInput.y, ForceMode.Impulse);
		// baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}

	public void SetBotSpeed(float newSpeed) {
		botMovementSpeed = newSpeed;
	}

	// public void SetEnemy() {
	// 	gameObject.AddComponent<EnemyController>();
	// 	gameObject.tag = "enemy";
	// 	enemy = gameObject;
	// }
    //
	// public void SetPlayer() {
	// 	gameObject.tag = "Player";
	// 	gameObject.AddComponent<PlayerController>();
	// 	player = gameObject;
	// 	EnablePlayerControls();
	// 	cameraController.SetPlayerFocus();
	// }

	public void DeactivateBot() {
		gameObject.SetActive(false);
	}

}
