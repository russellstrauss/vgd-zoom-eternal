using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	
	OrbitalCameraController cameraController;
	bool exploded = false;
	float particleSize = .2f;
	int particleSubdivisions = 5;
	float particleMass;
	float particlePivotDistance;
	Vector3 particlesPivot;
	float explosionForce = 100f;
	float explosionRadius = 2f;
	float explosionUpward = .5f;
	public Material explosionParticleMaterial;
	Renderer particleRenderer;
	GameObject player;
	public float health = 1000f;
	public TextMeshProUGUI winText;
	public TextMeshProUGUI playerHealthLabel;
	public TextMeshProUGUI enemyHealthLabel;
	TimerCountdownController battleClock;
	
	void Start() {
		
		cameraController = GameObject.FindWithTag("MainCamera").GetComponent<OrbitalCameraController>();
		
		SetParticles();
		cameraController.SetPlayerFocus();
		
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		if (winText != null) winText.enabled = false;
		if (FindObjectsOfType<TimerCountdownController>().Length > 0) battleClock = FindObjectsOfType<TimerCountdownController>()[0];
	}

	void Update() {
		
	}
	
	void Explode() {
		gameObject.SetActive(false);
			
		for (int x = 0; x < particleSubdivisions; x++) {
			for (int y = 0; y < particleSubdivisions; y++) {
				for (int z = 0; z < particleSubdivisions; z++) {
					createParticle(x, y, z);
				}
			}
		}
		
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider collision in colliders) {
			
			Rigidbody rb = collision.GetComponent<Rigidbody>();
			if (rb != null) {
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
			}
		}
		exploded = true;
	}
	
	void createParticle(int x, int y, int z) {
		GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
		if (particleRenderer != null){
			particleRenderer.material = explosionParticleMaterial;
		}
		
		particle.transform.position = transform.position + new Vector3(particleSize * x, particleSize * y, particleSize * z) - particlesPivot;
		particle.transform.localScale = new Vector3(particleSize, particleSize, particleSize);
		
		Rigidbody particleRB = particle.AddComponent<Rigidbody>();
		particleRB.mass = particleSize;
	}
	
	public void AddHealth(float amount) {
		health += amount;
	}
	
	public void SubtractHealth(float amount) {
		health -= amount;
		if (playerHealthLabel != null) playerHealthLabel.text = health.ToString("0");
		if (health < .1) {
			Explode();
			TriggerDeathState();
		}
	}
	
	void SetParticles() {
		particlePivotDistance = particleSize * particleSubdivisions / 2;
		particlesPivot = new Vector3(particlePivotDistance, particlePivotDistance, particlePivotDistance);
		particleRenderer = GetComponent<Renderer>();
	}
	
	void TriggerWinState() {
		if (winText != null) winText.enabled = true;
	}
	
	public void hideAllLabels() {
		if (winText != null) winText.enabled = false;
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
		// Time.timeScale = .1f;
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject != GameObject.FindWithTag("Floor")) {
			FindObjectOfType<AudioManager>().PlayRandomCrashShort();
		}
	}
}