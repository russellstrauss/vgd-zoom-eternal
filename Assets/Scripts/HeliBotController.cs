using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliBotController : MonoBehaviour
{
	private GameObject propeller;
	private float rotation;
	private float timer = 0.0f;
	private float baseSpeed = 120f;
	private float maxSpeed = 3000f;
	private float health = 100f;
	
	void Start() {
		propeller = GameObject.Find("Propeller");
	}
	
	void Update() {
		timer += Time.deltaTime;
		if (rotation < maxSpeed) rotation = (float)Math.Log(timer, 1.1) * baseSpeed;
		Debug.Log(rotation);
		propeller.transform.Rotate(new Vector3(0, rotation, 0) * Time.deltaTime);
	}
}