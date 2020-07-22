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
	private AudioSource music;
	private float musicDefaultVolume;
	
	// Hide Win/Lose state GUI's
	private GameObject player;
	private HeliBotController heliBotController;
    
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
		if (player) heliBotController = player.GetComponent<HeliBotController>();
		music = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
		musicDefaultVolume = music.volume;
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
		if (heliBotController) heliBotController.hideAllLabels();
		if (music) music.volume = musicDefaultVolume / 8;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
		music.volume = musicDefaultVolume;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}
