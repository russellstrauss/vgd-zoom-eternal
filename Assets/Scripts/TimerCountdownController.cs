using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdownController : MonoBehaviour
{
	private TextMeshProUGUI countdownClock;
	private int countdownSecondsRemaining = 3;
	private Boolean countdownDecreasing = false;
	private Boolean countdownInitialized = false;
	private TextMeshProUGUI battleClock;
	private float battleSecondsRemaining = 180f;
	private Boolean battleClockDecreasing = false;
	private Boolean battleClockInitialized = false;
	private AudioSource music;
	private HeliBotController playerController;
	private EnemyController enemyController;
	private PauseMenu pauseMenu;
	
	InputMaster controls;
	Vector2 movementInput;
	
	void Start() {
		
		music = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
		music.Play();
		music.Pause();
		
		enemyController = GameObject.FindWithTag("enemy").GetComponent<EnemyController>();
		playerController = GameObject.FindWithTag("Player").GetComponent<HeliBotController>();
		
		pauseMenu = GameObject.FindWithTag("GameManager").GetComponent<PauseMenu>();
		pauseMenu.PauseGame();
		battleClock = gameObject.transform.Find("Game Timer").GetComponent<TextMeshProUGUI>();
		countdownClock = gameObject.transform.Find("Match Start Countdown").GetComponent<TextMeshProUGUI>();
		countdownClock.text = countdownSecondsRemaining.ToString("0");
		StartCountdown();
	}

	void Update() {
		
		if (battleClockInitialized && !battleClockDecreasing && battleSecondsRemaining > 0 && Time.timeScale == 1) {
			StartCoroutine(DecreaseBattleTime());
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
		if (battleClockInitialized) {
			
			battleClockDecreasing = true;
			yield return new WaitForSecondsRealtime(1);
			battleSecondsRemaining -= 1;
			String time = TimeSpan.FromSeconds(battleSecondsRemaining).ToString(@"m\:ss");
			if (battleClock) battleClock.text = time;
			battleClockDecreasing = false;
			
			if (battleSecondsRemaining < 1) TimeUp();
		}
	}
	
	void TimeUp() {
		
		if (playerController.health < enemyController.health) {
			playerController.TriggerTimeUpLose();
		}
		else {
			playerController.TriggerTimeUpWin();
		}
	}
	
	private void StartCountdown() {
		countdownInitialized = true;
		FindObjectOfType<AudioManager>().Play("short-beep");
	}
	
	public void StartTimer() {
		pauseMenu.ResumeGame();
		battleClockInitialized = true;
		music.UnPause();
	}
	
	public void StopTimer() {
		battleClockInitialized = false;
		StopCoroutine(DecreaseBattleTime());
		Debug.Log("timer stopped");
	}
}
