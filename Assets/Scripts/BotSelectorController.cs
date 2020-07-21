using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	
	float botRotationSpeed = 12.5f;
	GameObject[] characterSelections;
	
	void Start() {
		characterSelections = GameObject.FindGameObjectsWithTag("characterSelect");
		HideAllBots();
	}

	void Update() {
		SpinCharacterSelection();
	}
	
	void SpinCharacterSelection() {
		
		foreach (GameObject character in characterSelections) {
			character.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * botRotationSpeed);
		}
	}
	
	public void ShowBot(string botName) {
		foreach (GameObject character in characterSelections) {
			character.SetActive(false);
			if (character.name == botName) character.SetActive(true);
		}
	}
	
	void HideAllBots() {
		foreach (GameObject character in characterSelections) {
			character.SetActive(false);
		}
	}
}
