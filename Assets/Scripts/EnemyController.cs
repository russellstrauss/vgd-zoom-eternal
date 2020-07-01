using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour {
	
	private int collisionCount;
	public float particleSize = .2f;
	public int particleSubdivisions = 5;
	public float particleMass;
	float particlePivotDistance;
	Vector3 particlesPivot;
	public float explosionForce;
	public float explosionRadius;
	public float explosionUpward;
	public Material explosionParticleMaterial;
	private Renderer particleRenderer;
	private GameObject player;
	public float health = 1000f;
	public TextMeshProUGUI enemyHealthLabel;
	private int gravityMultiplier = 10000;
	Rigidbody enemyRB;
	
	void Start() {
		collisionCount = 0;
		gameObject.tag = "enemy";
		
		particlePivotDistance = particleSize * particleSubdivisions / 2;
		particlesPivot = new Vector3(particlePivotDistance, particlePivotDistance, particlePivotDistance);
		
		particleRenderer = GetComponent<Renderer>();
		player = GameObject.FindWithTag("Player");
		Debug.Log("Health " + health.ToString());
		if (enemyHealthLabel != null) enemyHealthLabel.text = health.ToString();
		enemyRB = gameObject.GetComponent<Rigidbody>();
	}

	void Update() {
		if (enemyRB != null) enemyRB.AddForce(-transform.up * gravityMultiplier, ForceMode.Force);
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		
		if (collision.gameObject == player) {
			
			player.GetComponent<HeliBotController>().SubtractHealth(10);
			
			collisionCount++;
			FindObjectOfType<AudioManager>().Play("crash");
			
		}
	}
	
	public void SubtractHealth(float amount) {
		health -= amount;
		if (enemyHealthLabel != null) enemyHealthLabel.text = health.ToString();
		
		if (health < .1) {
			explode();
		}
	}
	
	private void explode() {
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
	}
	
	private void createParticle(int x, int y, int z) {
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