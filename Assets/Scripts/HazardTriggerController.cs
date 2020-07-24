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
	
	void OnTriggerEnter(Collider otherCollision) {
		
		inTrigger = true;
		hazardTrigger = otherCollision.GetComponent<BoxCollider>();
		
		if (sawBladeController != null && hazardTrigger != null) {
			sawBladeController.TriggerAttack(otherCollision);
		}
	}
	
	void OnTriggerExit(Collider otherCollision) {
		inTrigger = false;
		hazardTrigger = otherCollision.GetComponent<BoxCollider>();
		
		if (hazardTrigger != null) {
			
			if (sawBladeController != null) {
				sawBladeController.ExitAttack();
			}
			count++;
		}
	}
}
