﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdownController : MonoBehaviour
{
	TextMeshProUGUI countdownClock;
	int countdownSecondsRemaining = 3;
	bool countdownDecreasing = false;
	bool countdownInitialized = false;
	TextMeshProUGUI battleClock;
	float battleSecondsRemaining = 180f;
	bool battleClockDecreasing = false;
	bool battleClockInitialized = false;
	AudioSource music;
	HeliBotController playerController;
	EnemyController enemyController;
	PauseMenu pauseMenuController;
	GameObject player;
	
	IEnumerator battleClockTimer;
	
	InputMaster controls;
	Vector2 movementInput;
	
	void Start() {
		
		if (enemyController != null) enemyController = GameObject.FindWithTag("enemy").GetComponent<EnemyController>();
		player = GameObject.FindWithTag("Player");
		if (player != null) playerController = player.GetComponent<HeliBotController>();
		
		pauseMenuController = GameObject.FindWithTag("GameManager").GetComponent<PauseMenu>();
		pauseMenuController.PauseGame();
		pauseMenuController.pauseMenu.SetActive(false);
		battleClock = gameObject.transform.Find("Game Timer").GetComponent<TextMeshProUGUI>();
		countdownClock = gameObject.transform.Find("Match Start Countdown").GetComponent<TextMeshProUGUI>();
		countdownClock.text = countdownSecondsRemaining.ToString("0");
		battleClockTimer = DecreaseBattleTime();
		StartCountdown();
	}

	void Update() {
		
		if (battleClockInitialized && !battleClockDecreasing && battleSecondsRemaining > 0 && Time.timeScale == 1) {
			StartCoroutine("DecreaseBattleTime");
		}
		
		if (countdownInitialized && !countdownDecreasing && countdownSecondsRemaining > 0) {
			StartCoroutine(DecreaseCountdown());
		}
		else if (countdownSecondsRemaining < 1 && !battleClockInitialized) {
			StartTimer();
		}
	}
	
	IEnumerator DecreaseCountdown() {
		countdownDecreasing = true;
		yield return new WaitForSecondsRealtime(1);
		countdownSecondsRemaining -= 1;
		String time = TimeSpan.FromSeconds(countdownSecondsRemaining).ToString("%s");
		if (countdownClock && countdownSecondsRemaining > 0) {
			countdownClock.text = time;
			FindObjectOfType<AudioManager>().Play("short-beep");
		}
		else if (countdownSecondsRemaining == 0) {
			countdownClock.fontSize = 200;
			countdownClock.text = "FIGHT!";
			FindObjectOfType<AudioManager>().Play("buzzer");
			yield return new WaitForSecondsRealtime(1);
			countdownClock.enabled = false;
			StartTimer();
		}
		countdownDecreasing = false;
	}
	
	IEnumerator DecreaseBattleTime() {
		
		battleClockDecreasing = true;
		yield return new WaitForSecondsRealtime(1);
		battleSecondsRemaining -= 1;
		String time = TimeSpan.FromSeconds(battleSecondsRemaining).ToString(@"m\:ss");
		if (battleClock) battleClock.text = time;
		battleClockDecreasing = false;
		
		if (battleSecondsRemaining < 1) TimeUp();
	}
	
	void TimeUp() {
		
		if (playerController.health < enemyController.health) {
			playerController.TriggerTimeUpLose();
		}
		else {
			playerController.TriggerTimeUpWin();
		}
	}
	
	void StartCountdown() {
		countdownInitialized = true;
		FindObjectOfType<AudioManager>().Play("short-beep");
	}
	
	public void StartTimer() {
		pauseMenuController.ResumeGame();
		battleClockInitialized = true;
	}
	
	public void StopTimer() {
		battleClockInitialized = false;
		StopCoroutine("DecreaseBattleTime");
	}
}
