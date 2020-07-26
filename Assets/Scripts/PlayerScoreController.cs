using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreController : MonoBehaviour
{
	TextMeshProUGUI playerScoreLabel;
	
	void Start() {
		playerScoreLabel = gameObject.GetComponent<TextMeshProUGUI>();
	}

	void Update() {
		
	}
	
	public void SetScore(float score) {
		playerScoreLabel.text = Math.Floor(score).ToString();
	}
}
