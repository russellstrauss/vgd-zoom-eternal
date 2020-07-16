using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardTriggerController : MonoBehaviour
{
	// Application.LoadLevel(Application.loadedLevel)
	[HideInInspector]
	public bool inTrigger = false;
	private SawBladeController sawBladeController;
	BoxCollider hazardTrigger;
	
	int count = 0;
	
	void Start() {
		sawBladeController = gameObject.GetComponentInParent<SawBladeController>();
	}

	void Update() {}
	
	void OnTriggerEnter(Collider other) {
		
		inTrigger = true;
		hazardTrigger = other.GetComponent<BoxCollider>();
		
		if (hazardTrigger != null) {
			Debug.Log("OnTriggerEnter=" + other.name + " x" + count);
			if (sawBladeController != null) sawBladeController.TriggerAttack();
			count++;
		}
	}
	
	void OnTriggerExit(Collider other) {
		inTrigger = false;
		hazardTrigger = other.GetComponent<BoxCollider>();
		
		if (hazardTrigger != null) {
			
			Debug.Log("OnTriggerExit=" + other.name + " x" + count);
			if (sawBladeController != null) sawBladeController.ExitAttack();
			count++;
		}
	}
}
