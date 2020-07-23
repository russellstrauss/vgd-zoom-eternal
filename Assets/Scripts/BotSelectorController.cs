using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	static public string selectedBot;
	float botRotationSpeed = 20f;
	GameObject[] characterSelections;
	
	void Start() {
		characterSelections = GameObject.FindGameObjectsWithTag("characterSelect");
		HideAllBots();
		
		if (selectedBot != null) {
			
			if (selectedBot == "Stellar Propeller" && FindObjectsOfType<HeliBotController>().Length > 0) FindObjectsOfType<HeliBotController>()[0].SetPlayer();
		}
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
			if (character.name == botName) {
				selectedBot = botName;
				character.SetActive(true);
			}
		}
	}
	
	void HideAllBots() {
		foreach (GameObject character in characterSelections) {
			character.SetActive(false);
		}
	}
}
