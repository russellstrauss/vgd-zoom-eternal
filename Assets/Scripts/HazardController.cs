﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
	public GameObject pickupEffect;
	// public float multiplier = 1.4f;
	public float duration = 2;

    void OnTriggerEnter(Collider other)
    {
    	if (other.CompareTag("Player")){
    		// Pickup(other);
    		StartCoroutine(Pickup(other));
    	}


    }
    IEnumerator Pickup(Collider player)
    {
    	Debug.Log("Hazard is being picked");
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
