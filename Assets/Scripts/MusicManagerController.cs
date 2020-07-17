using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerController : MonoBehaviour
{
	void Start() {
		DontDestroyOnLoad(gameObject);
	}

	void Update() {
		
	}
	
	public void Stop() {
		gameObject.GetComponent<AudioSource>().Stop();
	}
}
