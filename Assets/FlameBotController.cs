using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
namespace DigitalRuby.PyroParticles {
public class FlameBotController : MonoBehaviour
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

	// propeller
	private bool fireOn = false;
	// private Rigidbody propellerRB;
	private Rigidbody baseRB;
	private GameObject propeller;
	// private double propellerRotationSpeed;
	// private float propellerMaxSpeed = 3000f;
	// private float propellerTimer = 0.0f;
	// private float propellerRotationBaseSpeed = 8f; // exponential
    private Quaternion _lookRotation;
	private Vector3 _direction;
	private float distance;
	private float trail = 8.0f;
	public GameObject[] Prefabs;
	public UnityEngine.UI.Text CurrentItemText;

	private GameObject currentPrefabObject;
	private FireBaseScript currentPrefabScript;
	private int currentPrefabIndex;
	private Quaternion originalRotation;

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
	// bool propellerButtonHeld = false;

	// test vars
	private int count = 0;
	EnemyController enemyController;

	void Awake() {
		controls = new InputMaster();

		if (controls != null && gameObject.CompareTag("Player")) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

			controls.Player.Select.performed += ctx => FireOn();
			controls.Player.Select.canceled += ctx => StopCurrent();

			controls.Player.Drive.performed += ctx => Drive();
			controls.Player.Drive.canceled += ctx => DriveRelease();
		}
	}

	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }

	void Start() {
		Reset();
		player = GameObject.FindWithTag("Player");
		// propeller = GameObject.Find("Propeller");
		// propellerRB = propeller.GetComponent<Rigidbody>();
		baseRB = gameObject.GetComponent<Rigidbody>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		if (winText != null) winText.enabled = false;
		if (playerHealthLabel != null) {
			playerHealthLabel.text = health.ToString("0");
		}
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		if (FindObjectsOfType<TimerCountdownController>().Length > 0) battleClock = FindObjectsOfType<TimerCountdownController>()[0];
		if (FindObjectsOfType<EnemyController>().Length > 0) enemyController = FindObjectsOfType<EnemyController>()[0];

		sparks = player.GetComponentsInChildren<ParticleSystem>();
		HideWheelSparks();
	}

	void OnCollisionStay(Collision otherObjectCollision) {

	}

	void OnCollisionEnter(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == enemy && enemy != null && enemyController != null) {

			float damage = 5;
			enemyController.SubtractHealth(damage);
			if (enemyController.health < .1) TriggerWinState();
			// if (propellerOn && propellerRotationSpeed > propellerMaxSpeed * .9f) {
			// 	PropellerOff("propeller-off-sudden");
			// 	FindObjectOfType<AudioManager>().Stop("propeller-on");
			// 	baseRB.AddForce(transform.up * 2000 * movementInput.y, ForceMode.Impulse);
			// 	propellerOn = false;
			// }
		}

		if (otherObjectCollision.gameObject == floor) grounded = true;
	}

	void OnCollisionExit(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == floor) grounded = false;
	}

	void FixedUpdate() {

		if (player != null) {
			UpdateFlame();
			UpdatePlayerMovement();
		}
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
			//Debug.Log("Driving forward?");
			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.right, new Vector3(0, 1, 0))); // Get forward direction along the ground
			if (grounded) Debug.DrawRay(transform.position, direction * 3, Color.green);
			else {
				Debug.DrawRay(transform.position, direction * 3, Color.red);
			}
			baseRB.AddForce(direction * botMovementSpeed, ForceMode.Impulse);
		}
		else {
			HideWheelSparks();
		}

		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}

	// void UpdatePropeller() {
	// 	propellerTimer += Time.deltaTime;
	// 	if (propellerRotationSpeed < propellerMaxSpeed && propellerOn) propellerRotationSpeed = (float)Math.Pow(propellerRotationBaseSpeed, propellerTimer);
	// 	else if (!propellerOn) {
	// 		if (propellerRotationSpeed > 0) propellerRotationSpeed -= (propellerTimer * 5);
	// 		else {
	// 			propellerRotationSpeed = 0;
	// 		}
	// 	}
	// 	propeller.transform.Rotate(new Vector3(0, (float)propellerRotationSpeed, 0) * Time.deltaTime);
	// 	if (explosion != null) {
	// 		explosion.transform.position = propeller.transform.position;
	// 		explosion.transform.rotation = propeller.transform.rotation;
	// 	}
    //
	// 	if (enemyWayPoint != null) {
	// 		enemyWayPoint.transform.position = player.transform.position;
	// 	}
	// }
    //
	// void PropellerOn() {
	// 	propellerButtonHeld = true;
	// 	FindObjectOfType<AudioManager>().Play("propeller-on");
	// 	propellerOn = true;
	// 	propellerTimer = 0;
	// 	count++;
	// }
    //
	// void PropellerButtonRelease() {
	// 	propellerButtonHeld = false;
	// 	PropellerOff();
	// }
    //
	// void PropellerOff(String offSound = "propeller-off") {
    //
	// 	FindObjectOfType<AudioManager>().Stop("propeller-on");
	// 	if (propellerRotationSpeed > propellerMaxSpeed * .6) FindObjectOfType<AudioManager>().Play(offSound);
	// 	propellerOn = false;
	// 	propellerTimer = 0;
	// 	count++;
    //
	// 	// Debug.Log("propellerButtonHeld=" + propellerButtonHeld);
    //
	// 	if (propellerButtonHeld) PropellerOn();
	// }
    void UpdateFlame() {
        if (fireOn) {
        if (GameObject.Find("Flamethrower(Clone)") != null) {
            //already exists, lets update location
            GameObject fire = GameObject.Find("Flamethrower(Clone)");
            // Debug.Log("Game object exists");
            fire.transform.position =  transform.position+(transform.right*2)+transform.up*2;
            fire.transform.rotation =  player.transform.rotation * Quaternion.Euler(0, 90, 0);
			//if (heliBotController != null && Time.timeScale == 1) heliBotController.SubtractHealth(enemyDamageRatio);
            //Debug.Log("at pt" + fire.transform.position);
        } else {
            //does not exist so lets make flame
            //Debug.Log("Making flame");
            BeginEffect();
        }
    }
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
		OrbitalCameraController cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}

	void TriggerWinState() {
		if (winText != null) winText.enabled = true;
	}

	void Explode() {
		if (explodeCount < 1) explosion = Instantiate(explosionEffect, player.transform.position, transform.rotation);
		if (controls != null) disableBotControls();
		explodeCount++;
	}

	void disableBotControls() {
		controls.Player.Move.Disable();
		controls.Player.Select.Disable();
	}

    void FireOn() {
        fireOn = true;
        if (GameObject.Find("Flamethrower(Clone)") != null) {
            //already exists, lets update location
            GameObject fire = GameObject.Find("Flamethrower(Clone)");
            // Debug.Log("Game object exists");
            fire.transform.position =  transform.position+(transform.right*2)+transform.up*2;
            fire.transform.rotation =  player.transform.rotation * Quaternion.Euler(0, 90, 0);
			//if (heliBotController != null && Time.timeScale == 1) heliBotController.SubtractHealth(enemyDamageRatio);
            //Debug.Log("at pt" + fire.transform.position);
        } else {
            //does not exist so lets make flame
            //Debug.Log("Making flame");
            BeginEffect();
        }
    }

    private void BeginEffect()
    {
        fireOn = true;
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        currentPrefabObject = GameObject.Instantiate(Prefabs[currentPrefabIndex]);
        currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();

        if (currentPrefabScript == null)
        {
            // temporary effect, like a fireball
            currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
            if (currentPrefabScript.IsProjectile)
            {
                // set the start point near the player
                rotation = _lookRotation;
                pos = transform.position + forward + right + up;
            }
            else
            {
                // set the start point in front of the player a ways
                pos = transform.position + (forwardY * 4.0f);
            }
        }
        else
        {
            // set the start point in front of the player a ways, rotated the same way as the player
            pos = transform.position + (forwardY * 2.0f);
            rotation = _lookRotation;
            pos.y = 0.5f;
        }

        FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
        // if (projectileScript != null)
        // {
        //     // make sure we don't collide with other fire layers
        //     projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
        // }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = _lookRotation;
    }
    public void StartCurrent()
    {
        StopCurrent();
        BeginEffect();
    }

    private void StopCurrent()
    {
        // if we are running a constant effect like wall of fire, stop it now
        fireOn = false;
        if (currentPrefabScript != null && currentPrefabScript.Duration > 10000)
        {
            currentPrefabScript.Stop();
        }
        currentPrefabObject = null;
        currentPrefabScript = null;
    }

}
}
