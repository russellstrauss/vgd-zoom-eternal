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
	
	void Start() {
		
		Scene scene = SceneManager.GetActiveScene();

		if (hoverIcon != null) hoverIcon.SetActive(false);
        
		if (scene.name == "ArenaSelector") {
			background.SetActive(false);
			activeBG = GameObject.Find("ActiveBG").GetComponent<Image>();
		}
		
	}
	
	void Update() {
		
	}
	
	public void mouseIn() {
		if (hoverIcon != null) hoverIcon.SetActive(true);
		if (background != null) activeBG.sprite = background.GetComponent<Image>().sprite;
	}
	
	public void mouseOut() {
		if (hoverIcon != null) hoverIcon.SetActive(false);
	}
	
	public void menuClick() {
		SceneManager.LoadScene(sceneNameToLoad);
		if (FindObjectOfType<MusicManagerController>() != null) FindObjectOfType<MusicManagerController>().Stop();
	}
}
