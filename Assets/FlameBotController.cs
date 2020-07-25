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
	OrbitalCameraController cameraController;
	Vector2 movementInput;
	InputMaster controls;
	private Vector3 playerStartPosition;
	private Vector3 cameraStartPosition;

	// Win state
	public TextMeshProUGUI winText;
	public TextMeshProUGUI playerHealthLabel;
	private GameObject enemyWayPoint;
	private TimerCountdownController battleClock;

	// propeller
	private bool fireOn = false;
	private Rigidbody baseRB;
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
	private int gravityMultiplier = 4000;
	public GameObject explosionEffect;
	public GameObject sparkEffect;
	private GameObject explosion;
	private GameObject player;
	private GameObject enemy;
	private GameObject floor;
	private bool upsideDown = false;
	Renderer particleRenderer;

	EnemyController enemyController;
	PlayerController playerController;

	void Start() {
		Reset();
		player = GameObject.FindWithTag("Player");
		mainCamera = GameObject.FindWithTag("MainCamera");
		cameraController = mainCamera.GetComponent<OrbitalCameraController>();
		baseRB = gameObject.GetComponent<Rigidbody>();
		if (winText != null) winText.enabled = false;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		enemyWayPoint = GameObject.Find("wayPoint");
		enemy = GameObject.FindWithTag("enemy");
		floor = GameObject.FindWithTag("Floor");
		battleClock = FindObjectOfType<TimerCountdownController>();
		enemyController = FindObjectOfType<EnemyController>();
		playerController = FindObjectOfType<PlayerController>();
		
		if (player != null && player == gameObject) SetPlayer();
		else if (enemy != null && enemy == gameObject) SetEnemy();
	}

	void OnCollisionStay(Collision otherObjectCollision) {

	}

	void OnCollisionEnter(Collision otherObjectCollision) {

		if (otherObjectCollision.gameObject == enemy && enemy != null && enemyController != null) {

			float damage = 5;
			enemyController.SubtractHealth(damage);
			if (enemyController.health < .1) TriggerWinState();
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

	void UpdatePlayerMovement() {

		upsideDown = Vector3.Dot(transform.up, Vector3.down) > 0;
		player.transform.Rotate(new Vector3(0, botRotationSpeed * movementInput.x, 0) * Time.deltaTime);

		if (driving) {
			//Debug.Log("Driving forward?");
			Vector3 direction =  Vector3.Normalize(Vector3.ProjectOnPlane(transform.right, new Vector3(0, 1, 0))); // Get forward direction along the ground
			baseRB.AddForce(direction * botMovementSpeed, ForceMode.Impulse);
		}

		baseRB.AddForce(new Vector3(0, -1, 0) * gravityMultiplier, ForceMode.Force);
	}

    void UpdateFlame() {
        if (fireOn) {
			if (GameObject.Find("Flamethrower(Clone)") != null) {
				//already exists, lets update location
				GameObject fire = GameObject.Find("Flamethrower(Clone)");
				// Debug.Log("Game object exists");
				fire.transform.position =  transform.position+(transform.right*2)+transform.up*2;
				fire.transform.rotation =  player.transform.rotation * Quaternion.Euler(0, 90, 0);
				// if (playerController != null && Time.timeScale == 1 && player != gameObject) playerController.SubtractHealth(enemyDamageRatio);
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
			// if (playerController != null && Time.timeScale == 1 && player != gameObject) playerController.SubtractHealth(enemyDamageRatio);
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
	
	public void SetPlayer()
	{
		gameObject.tag = "Player";
		gameObject.AddComponent<PlayerController>();
		player = gameObject;
		EnablePlayerControls();
		Debug.Log("Init flame bot here");
	}
	
	public void SetEnemy()
	{
		gameObject.AddComponent<EnemyController>();
		gameObject.tag = "enemy";
		enemy = gameObject;
	}
	
	void EnablePlayerControls() {
		
		controls = new InputMaster();

		if (controls != null && gameObject.CompareTag("Player")) {
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
			controls.Player.Select.performed += ctx => FireOn();
			controls.Player.Select.canceled += ctx => StopCurrent();
			controls.Player.Drive.performed += ctx => Drive();
			controls.Player.Drive.canceled += ctx => DriveRelease();
			controls.Player.Enable();
		}
	}
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}
}
