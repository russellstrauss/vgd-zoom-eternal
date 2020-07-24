using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManagerController : MonoBehaviour
{
	public Sound[] musicList;
	public Sound selectedMusic;
	
	void Awake() {
		
		foreach(Sound s in musicList) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.defaultVolume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}
	
	void Start() {
		DontDestroyOnLoad(gameObject);
	}

	void Update() {
		
	}
	
	public void Stop() {
		gameObject.GetComponent<AudioSource>().Stop();
	}
	
	public void PlayRandomSong() {
		System.Random random = new System.Random();
		selectedMusic = musicList[random.Next(0, musicList.Length - 1)];
		selectedMusic.source.Play();
	}
	
	public void SetVolume(float volume) {
		if (selectedMusic != null && selectedMusic.source != null) selectedMusic.source.volume = volume;
	}
	
	public void LowerVolume() {
		SetVolume(selectedMusic.defaultVolume / 8);
	}
	
	public void ResetVolume() {
		SetVolume(selectedMusic.defaultVolume);
	}
}
