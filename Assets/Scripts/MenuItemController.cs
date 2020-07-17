using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemController : MonoBehaviour
{
	
	GameObject hoverIcon;
	
	void Start() {
		
		Image[] uiSprites = gameObject.GetComponentsInChildren<Image>();
         
       foreach(Image icon in uiSprites)
       {
           if (icon.gameObject.transform.parent != null && icon != gameObject) {
               hoverIcon = icon.gameObject;
           }
       }
	}
	
	void Update() {
		
	}
	
	public void mouseIn() {
		// Debug.Log("mouseIn " + gameObject);
		// hoverIcon.enabled = true;
		hoverIcon.SetActive(true);
		Debug.Log(hoverIcon);
	}
	
	public void mouseOut() {
		// Debug.Log("mouseOut " + gameObject);
		// hoverIcon.enabled = false;
		hoverIcon.SetActive(false);
		Debug.Log(hoverIcon);
	}
	
	public void menuClick() {
		Debug.Log("menuClick " + gameObject);
	}
}
