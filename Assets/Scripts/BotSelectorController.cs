using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	
	float botRotationSpeed = 12.5f;
	GameObject[] characterSelections;
	
	void Start() {
		characterSelections = GameObject.FindGameObjectsWithTag("characterSelect");
	}

	void Update() {
		SpinCharacterSelection();
	}
	
	void SpinCharacterSelection() {
		
		foreach (GameObject character in characterSelections) {
			character.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed);
		}
	}
}
