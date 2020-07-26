using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
	InputMaster controls;
	Vector2 movementInput;
    public GameObject pauseMenu;
    public static bool isPaused;
	public GameObject[] menuItems;
	AudioSource music;
	
	// Hide Win/Lose state GUI's
	GameObject player;
	PlayerController playerController;
	HeliBotController heliBotController;
    
	void Awake() {
		controls = new InputMaster();
		if (controls != null) {
			controls.Player.Exit.performed += ctx => setState();
			controls.Player.Move.performed+= ctx => selectMenuItem();
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
		}
	}
	
    void Start()
    {
        pauseMenu.SetActive(false);
		player = GameObject.FindWithTag("Player");
		if (player) playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
		
    }
	
	void setState() 
	{
		if (isPaused)
		{
			ResumeGame();
		} else
		{
			PauseGame();
		}
	}
	
	void selectMenuItem() 
	{
		
	}

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
		if (playerController) playerController.hideAllLabels();
		FindObjectOfType<MusicManagerController>().LowerVolume();
		// FindObjectOfType<AudioMananger>().StopAllSounds();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
		FindObjectOfType<MusicManagerController>().ResetVolume();
    }

    public void GoToMenu()
    {
		FindObjectOfType<MusicManagerController>().StopAllMusic();
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
		FindObjectOfType<MusicManagerController>().PlayMainMenuMusic();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}
