using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	
	float botRotationSpeed = 25f;
	
	void Start() {
		
	}

	void Update() {
		SpinCharacterSelection();
	}
	
	void SpinCharacterSelection() {
		GameObject.FindWithTag("characterSelect").transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed);
	}
}
