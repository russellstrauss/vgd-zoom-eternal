﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	
	OrbitalCameraController cameraController;
	bool exploded = false;
	float particleSize = .2f;
	int particleSubdivisions = 10;
	float particleMass;
	float particlePivotDistance;
	Vector3 particlesPivot;
	float explosionForce = .002f;
	float explosionRadius = .5f;
	float explosionUpward = .5f;
	public Material explosionParticleMaterial;
	Renderer particleRenderer;
	GameObject player;
	float health = 1000f;
	public TextMeshProUGUI endStateText;
	TimerCountdownController battleClock;
	PlayerScoreController playerScoreController;
	GameObject endState;
	float soundThrottle = .1f;
	float soundThrottleTimer = 0;
	
	int count = 0;
	
	void Start() {
		
		cameraController = FindObjectOfType<OrbitalCameraController>();
		endState = GameObject.FindWithTag("endState");
		endStateText = endState.GetComponentInChildren<TextMeshProUGUI>();
		endState.SetActive(false);
		SetParticles();
		playerScoreController = FindObjectOfType<PlayerScoreController>();

		battleClock = FindObjectOfType<TimerCountdownController>();
		
		count++;
	}

	void Update() {
		soundThrottleTimer += Time.deltaTime;
		
	}
	
	void Explode() {
		gameObject.SetActive(false);
		
		if (!exploded) {
			
			for (int x = 0; x < particleSubdivisions; x++) {
				for (int y = 0; y < particleSubdivisions; y++) {
					for (int z = 0; z < particleSubdivisions; z++) {
						CreateParticle(x, y, z);
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
	}
	
	void CreateParticle(int x, int y, int z) {
		GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
		particle.tag = "noSound";
		if (particleRenderer != null) {
			particleRenderer.material = explosionParticleMaterial;
		}
		
		particle.transform.position = transform.position + new Vector3(particleSize * x, particleSize * y, particleSize * z) - particlesPivot;
		particle.transform.localScale = new Vector3(particleSize, particleSize, particleSize);
		
		Rigidbody particleRB = particle.AddComponent<Rigidbody>();
		particleRB.mass = 1;
	}
	
	public void AddHealth(float amount) {
		health += amount;
	}
	
	public void SubtractHealth(float amount) {
		health -= amount;
		if (health < 0) health = 0;
		playerScoreController.SetScore(health);
		if (health < .1) {
			TriggerDeathState();
		}
	}
	
	void SetParticles() {
		particlePivotDistance = particleSize * particleSubdivisions / 2;
		particlesPivot = new Vector3(particlePivotDistance, particlePivotDistance, particlePivotDistance);
		particleRenderer = GetComponent<Renderer>();
	}
	
	public void TriggerWinState() {
		if (endStateText != null) endStateText.text = "YOU WON THE MATCH!";
		EndState();
	}
	
	public void hideAllLabels() {
		if (endStateText != null) endStateText.enabled = false;
	}
	
	void TriggerDeathState() {
		Explode();
		if (endStateText != null) endStateText.text = "YOU COULDN'T HANDLE THE HEAT!";
		EndState();
	}
	
	public void TriggerTimeUpLose() {
		if (endStateText != null) endStateText.text = "TIME UP YOU LOST!";
		EndState();
	}

	public void TriggerTimeUpWin() {
		if (endStateText != null) endStateText.text = "TIME UP YOU WON!";
		EndState();
	}

	void EndState() {
		
		if (endState != null) endState.SetActive(true);
		if (endStateText != null) endStateText.enabled = true;
		cameraController.distance = 10f;
		battleClock.StopTimer();
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		float bumpDamage = .25f;
		if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
			otherObjectCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(bumpDamage); // get rb to deal damage based on speed
		}
		
		if (!otherObjectCollision.gameObject.CompareTag("Floor") && !otherObjectCollision.gameObject.CompareTag("barrier") && !otherObjectCollision.gameObject.CompareTag("noSound")) {
			if (soundThrottleTimer > soundThrottle) {
				FindObjectOfType<AudioManager>().PlayRandomCrashMed();
				soundThrottleTimer = 0;
			}
		}
	}
	
	void SetExplosionForces() {
		
		if (BotSelectorController.selectedBot == "Pecker Wrecker") {
			explosionForce = .00001f;
			explosionRadius = .5f;
		}
		if (BotSelectorController.selectedBot == "Stellar Propeller") {
			explosionForce = .002f;
			explosionRadius = .5f;
		}
		if (BotSelectorController.selectedBot == "Hot Bot") {
			
		}
	}
}