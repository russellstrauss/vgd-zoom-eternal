using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderTriggerController : MonoBehaviour
{
	private GameObject player;
	private GameObject enemy;
	private SawBladeController sawBladeController;
	
	void Start() {
		enemy = GameObject.FindWithTag("enemy");
		player = GameObject.FindWithTag("Player");
		sawBladeController = FindObjectsOfType<SawBladeController>()[0];
		Debug.Log("BoxColliderTriggerController init");
		Debug.Log("Player=" + player);
	}

	void Update() {
		
	}
	
	void OnTriggerEnter(Collider otherObject) {
		Debug.Log(otherObject.gameObject);
		if (otherObject.gameObject.CompareTag("Player")) sawBladeController.TriggerAttack();
	}
}
