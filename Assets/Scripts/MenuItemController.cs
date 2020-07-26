using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuItemController : MonoBehaviour
{
	Image activeBG;
	public GameObject hoverIcon;
	public GameObject background;
	public string sceneNameToLoad;
	
	BotSelectorController botSelectorController;
	Scene scene;
	
	void Start() {
		
		scene = SceneManager.GetActiveScene();

		if (hoverIcon != null) hoverIcon.SetActive(false);
        
		if (scene.name == "ArenaSelector") {
			background.SetActive(false);
			activeBG = GameObject.Find("ActiveBG").GetComponent<Image>();
		}
		else if (scene.name == "BotSelector") {
			botSelectorController = FindObjectOfType<BotSelectorController>();
		}
		
	}
	
	void Update() {
		
	}
	
	public void mouseIn() {
		if (hoverIcon != null) hoverIcon.SetActive(true);
		if (background != null) activeBG.sprite = background.GetComponent<Image>().sprite;
		
		if (botSelectorController != null) {
			botSelectorController.ShowBot(gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
		}
	}
	
	public void mouseOut() {
		if (hoverIcon != null) hoverIcon.SetActive(false);
	}
	
	public void menuClick() {
		
		if (scene.name == "ArenaSelector") SceneManager.LoadScene(sceneNameToLoad);
		else if (scene.name == "BotSelector") SceneManager.LoadScene("ArenaSelector");
		else {
			
			if (FindObjectOfType<MusicManagerController>() != null) FindObjectOfType<MusicManagerController>().Stop();
		}
	}
}
