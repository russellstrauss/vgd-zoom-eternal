using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
	PlayerController playerController;
	EnemyController enemyController;
	PauseMenu pauseMenuController;
	GameObject player;
	bool started = false;
	
	IEnumerator battleClockTimer;
	
	InputMaster controls;
	Vector2 movementInput;
	Scene scene;
	
	void Start() {
		
		if (enemyController != null) enemyController = GameObject.FindWithTag("enemy").GetComponent<EnemyController>();
		player = GameObject.FindWithTag("Player");
		if (player != null) playerController = player.GetComponent<PlayerController>();
		
		if (pauseMenuController) {
			
			pauseMenuController = GameObject.FindWithTag("GameManager").GetComponent<PauseMenu>();
			pauseMenuController.PauseGame();
			pauseMenuController.pauseMenu.SetActive(false);
		}
		
		scene = SceneManager.GetActiveScene();
		
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
			if (!started) StartTimer();
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
			if (!started) StartTimer();
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
		
		// if (playerController.health < enemyController.health) {
		// 	playerController.TriggerTimeUpLose();
		// }
		// else {
		// 	playerController.TriggerTimeUpWin();
		// }
	}
	
	void StartCountdown() {
		Debug.Log("StartCountdown");
		FindObjectOfType<MusicManagerController>().StopMainMenuMusic();
		countdownInitialized = true;
		FindObjectOfType<AudioManager>().Play("short-beep");
	}
	
	public void StartTimer() {
		FindObjectOfType<MusicManagerController>().PlayRandomSong();
		if (pauseMenuController != null) pauseMenuController.ResumeGame();
		battleClockInitialized = true;
		started = true;
	}
	
	public void StopTimer() {
		battleClockInitialized = false;
		StopCoroutine("DecreaseBattleTime");
	}
}
