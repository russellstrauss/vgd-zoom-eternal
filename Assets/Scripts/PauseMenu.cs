using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	InputMaster controls;
	Vector2 movementInput;
    public GameObject pauseMenu;
    public static bool isPaused;
    
	void Awake() {
		controls = new InputMaster();
		if (controls != null) {
			controls.Player.Exit.performed += ctx => setState();
			controls.Player.Move.performed+= ctx => selectMenu();
			controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
			controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
		}
	}
	
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
		// Debug.Log(movementInput);
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
	
	void selectMenu() 
	{
		
	}

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
	
	void OnEnable()	{ if (controls != null) controls.Player.Enable(); }
	void OnDisable() { if (controls != null) controls.Player.Disable(); }
}
