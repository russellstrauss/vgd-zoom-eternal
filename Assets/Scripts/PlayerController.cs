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
	int particleSubdivisions = 10;
	float particleMass;
	float particlePivotDistance;
	Vector3 particlesPivot;
	float explosionForce = 250f;
	float explosionRadius = 2f;
	float explosionUpward = 1f;
	public Material explosionParticleMaterial;
	Renderer particleRenderer;
	GameObject player;
	public float health = 1000f;
	public TextMeshProUGUI enemyHealthLabel;
	
	void Start() {
		
		cameraController = GameObject.FindWithTag("MainCamera").GetComponent<OrbitalCameraController>();
		
		particlePivotDistance = particleSize * particleSubdivisions / 2;
		particlesPivot = new Vector3(particlePivotDistance, particlePivotDistance, particlePivotDistance);
		particleRenderer = GetComponent<Renderer>();
		
		cameraController.SetPlayerFocus();
	}

	void Update() {
		
	}
	
	public void SubtractHealth(float amount) {
		health -= amount;
		if (enemyHealthLabel != null) enemyHealthLabel.text = health.ToString("0");
		
		if (health < .1 && !exploded) {
			explode();
		}
	}
	
	void explode() {
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
}