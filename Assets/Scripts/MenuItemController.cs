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
		
		background.SetActive(false);
		// if (gameObject.GetComponentInChildren<TextMeshProUGUI>().text != "Rage in the Cage") hoverIcon.SetActive(false);
		hoverIcon.SetActive(false);
		activeBG = GameObject.Find("ActiveBG").GetComponent<Image>();
	}
	
	void Update() {
		
	}
	
	public void mouseIn() {
		hoverIcon.SetActive(true);
		activeBG.sprite = background.GetComponent<Image>().sprite;
	}
	
	public void mouseOut() {
		hoverIcon.SetActive(false);
	}
	
	public void menuClick() {
		Debug.Log("menuClick " + gameObject);
		SceneManager.LoadScene(sceneNameToLoad);
		FindObjectOfType<MusicManagerController>().Stop();
	}
}
