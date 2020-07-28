using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	static public string selectedBot;
	float botRotationSpeed = 20f;
	GameObject[] characterSelections;
	static public string selectedEnemyBot = string.Empty;
    bool enemySet = false;
	
	HeliBotController heliBotController;
	PeckerWreckerController peckerWreckerController;
	
	void Start() {
		characterSelections = GameObject.FindGameObjectsWithTag("characterSelect");
		HideAllBotIcons();
		
		System.Random random = new System.Random();
		int value = random.Next(0, 2);
		// Debug.Log(value);
		// Debug.Log(selectedEnemyBot);
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
	
	void HideAllBotIcons() {
		foreach (GameObject character in characterSelections) {
			character.SetActive(false);
		}
	}
}
