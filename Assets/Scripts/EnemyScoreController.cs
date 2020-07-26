using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EnemyScoreController : MonoBehaviour
{
	TextMeshProUGUI enemyScoreLabel;
	
	void Start() {
		enemyScoreLabel = gameObject.GetComponent<TextMeshProUGUI>();
	}

	void Update() {
		
	}
	
	public void SetScore(float score) {
		enemyScoreLabel.text = Math.Floor(score).ToString();
	}
}
