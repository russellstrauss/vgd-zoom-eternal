using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorController : MonoBehaviour
{
	static public string selectedBot;
	float botRotationSpeed = 20f;
	GameObject[] characterSelections;
	static public string selectedEnemyBot;
    bool enemySet = false;
	
	HeliBotController heliBotController;
	PeckerWreckerController peckerWreckerController;
	
	void Start() {
		characterSelections = GameObject.FindGameObjectsWithTag("characterSelect");
		HideAllBotIcons();
		
		if (selectedBot != null) {
			
			try {
				
				while (!enemySet) {
                    
					// Select random bot to be enemy
                    System.Random random = new System.Random();
                    int value = random.Next(0, 2);
                    if (value == 0 && selectedBot != "Pecker Wrecker") {
                        selectedEnemyBot = "Pecker Wrecker";
                        FindObjectOfType<HeliBotController>().SetEnemy();
                        enemySet = true;
                    }
                    else if (value == 1 && selectedBot != "Stellar Propeller") {
                        selectedEnemyBot = "Stellar Propeller";
                        FindObjectOfType<PeckerWreckerController>().SetEnemy();
                        enemySet = true;
                    }
                    else if (value == 2 && selectedBot != "Hot Bot") {
                        selectedEnemyBot = "Hot Bot";
                        FindObjectOfType<DigitalRuby.PyroParticles.FlameBotController>().SetEnemy();
                        enemySet = true;
                    }
                    Debug.Log("Match bots selected. Player: "  + selectedBot + " Enemy: " + selectedEnemyBot);
                }
			}
			catch (System.Exception error) {
				Debug.Log("Error setting enemy: " + error);
			}
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
	
	void HideAllBotIcons() {
		foreach (GameObject character in characterSelections) {
			character.SetActive(false);
		}
	}
}
