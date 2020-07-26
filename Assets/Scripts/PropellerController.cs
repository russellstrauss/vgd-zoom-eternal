﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerController : MonoBehaviour
{	
	GameObject enemy;
	Rigidbody enemyRB;
	float propellerBaseDamage = .001f;
	float particleTimer = 0;
	float particleThrottle = 1f; // how long until trying to shoot a new one
	HeliBotController heliBotController;
	
	void Start() {
		enemy = GameObject.FindWithTag("enemy");
		if (enemy != null) enemyRB = enemy.GetComponent<Rigidbody>();
		heliBotController = FindObjectOfType<HeliBotController>();
	}

	void Update() {
		particleTimer += Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		
		if (otherObjectCollision.gameObject.GetComponent<HeliBotController>() == null) {
			
			Rigidbody otherRB = otherObjectCollision.gameObject.GetComponent<Rigidbody>();
			if (otherRB != null) {
				
				Vector3 contactNormal = otherObjectCollision.contacts[0].normal;
				otherRB.AddForce(contactNormal * (float)heliBotController.propellerRotationSpeed * 100000f * heliBotController.botMovementSpeed, ForceMode.Impulse);
			}
			if (otherObjectCollision.gameObject.GetComponent<PlayerController>() != null) {
				otherObjectCollision.gameObject.GetComponent<PlayerController>().SubtractHealth(propellerBaseDamage * heliBotController.GetPropellerSpeed());
			}
			if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
				otherObjectCollision.gameObject.GetComponent<EnemyController>().SubtractHealth(propellerBaseDamage * heliBotController.GetPropellerSpeed());
			}
			
			if (particleTimer > particleThrottle) {
				
				LaunchShrapnel(otherObjectCollision.gameObject.transform.position);
				particleTimer = 0;
			}
		}
	}
	
	private void CreateParticle(int x, int y, int z, int subdivisions) {
		GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
		particle.tag = "noSound";
		float particleSize = .2f;
		float particlePivotDistance = particleSize * subdivisions / 2;
		Vector3 particlesPivot = new Vector3(particlePivotDistance, particlePivotDistance, particlePivotDistance);
		particle.transform.position = transform.position + new Vector3(particleSize * x, particleSize * y, particleSize * z) - particlesPivot;
		particle.transform.localScale = new Vector3(particleSize, particleSize, particleSize);
		
		Rigidbody particleRB = particle.AddComponent<Rigidbody>();
		particleRB.mass = particleSize;
	}
	
	private void LaunchShrapnel(Vector3 position) {
		
		int particleSubdivisions = 1;
		for (int x = 0; x < particleSubdivisions; x++) {
			for (int y = 0; y < particleSubdivisions; y++) {
				for (int z = 0; z < particleSubdivisions; z++) {
					CreateParticle(x, y, z, particleSubdivisions);
				}
			}
		}
		
		float explosionForce = 250f;
		float explosionRadius = 2f;
		float explosionUpward = 1f;
		
		Vector3 explosionPos = position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider collision in colliders) {
			
			Rigidbody rb = collision.GetComponent<Rigidbody>();
			if (rb != null) {
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
			}
		}
	}
}