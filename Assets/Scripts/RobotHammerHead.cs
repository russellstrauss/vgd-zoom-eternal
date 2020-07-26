using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHammerHead : MonoBehaviour
{
	float hammerHeadDamage = 15;
	
	void Start() {
		
	}

	void Update() {
		
	}
	
	void OnCollisionEnter(Collision otherObjectCollision) {
		Debug.Log("Hammerhead collision");
		// if (otherObjectCollision.gameObject.GetComponent<EnemyController>() != null) {
		// 	gameObject.GetComponent<EnemyController>().SubtractHealth(hammerHeadDamage);
		// 	Debug.Log("Hammerhead enemy collision");
		// }
		
		// if (otherObjectCollision.gameObject.GetComponent<PlayerController>() != null) {
		// 	gameObject.GetComponent<PlayerController>().SubtractHealth(hammerHeadDamage);
		// }
	}
}
