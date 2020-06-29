﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference https://www.youtube.com/watch?v=CLSiRf_OrBk
// https://answers.unity.com/questions/1638885/how-to-destroy-a-clone-of-an-effect-attached-to-an.html
public class PowerUpController : MonoBehaviour
{
	public GameObject pickupEffect;
	// public float multiplier = 1.4f;
	public float duration = 2;
	// public Vector3 RotateAmount= new Vector3(0.0f, 1.0f, 0.0f);
	public float spinForce = 200.0f;


	void Start()
	{
		// var rb = GetComponent<Rigidbody>();
        // rb.angularVelocity = Random.insideUnitSphere;
	}
	void Update()
	{
		// transform.Rotate(RotateAmount * Time.deltaTime);
		transform.Rotate(0, spinForce * Time.deltaTime, 0);
		// transform.rotate(Vector3.right, rotateSpeed * Time.deltaTime, Space.World);
	}

    void OnTriggerEnter(Collider other)
    {
    	if (other.CompareTag("Player")){
    		// Pickup(other);
    		StartCoroutine(Pickup(other));
			// FindObjectOfType<AudioManager>().Play("ting");
			FindObjectOfType<AudioManager>().Play("crash");
			
    	}


    }
    IEnumerator Pickup(Collider player)
    {
    	Debug.Log("Power up is being picked");
    	GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
    	
    	// TODO add more healthy to the player

    	GetComponent<MeshRenderer>().enabled = false;
    	GetComponent<Collider>().enabled = false;

    	//remove the effect from theplayer
    	yield return new WaitForSeconds(duration);
    	//wait x amount of seconds

    	// remove power up object
    	ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
    	Destroy(clone, particle.duration);
    	Destroy(gameObject);
    }
}
