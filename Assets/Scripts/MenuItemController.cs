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
    public GameObject loadSlider;
    public Slider slider;
	
	BotSelectorController botSelectorController;
	Scene scene;
	
    void Awake()
    {
        loadSlider.SetActive(false);
    }
 
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

        if (scene.name == "ArenaSelector")
        {
            StartCoroutine(LoadAsynchronously(sceneNameToLoad));
        }
        else if (scene.name == "BotSelector")
        {
            SceneManager.LoadScene("ArenaSelector");
        }
		if (FindObjectOfType<MusicManagerController>() != null) FindObjectOfType<MusicManagerController>().Stop();
	}

    IEnumerator LoadAsynchronously (String sceneNameToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNameToLoad);

        loadSlider.SetActive(true);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;

            yield return null;
        }
    }
}
