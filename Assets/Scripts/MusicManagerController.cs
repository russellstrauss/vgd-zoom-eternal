using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerController : MonoBehaviour
{
	public Sound[] musicList;
	
	void Awake() {
		
		foreach(Sound s in musicList) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}
	
	void Start() {
		DontDestroyOnLoad(gameObject);
		
		if (musicList.Length > 0) PlayRandomSong();
	}

	void Update() {
		
	}
	
	public void Stop() {
		gameObject.GetComponent<AudioSource>().Stop();
	}
	
	void PlayRandomSong() {
		System.Random random = new System.Random();
		musicList[random.Next(0, musicList.Length - 1)].source.Play();
	}
}
