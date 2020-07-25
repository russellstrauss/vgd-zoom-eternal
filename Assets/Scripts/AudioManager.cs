using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;
	public Sound[] crashSoundsShort;
	public Sound[] crashSoundsMed;
	public Sound[] crashSoundsLong;
	float crashSoundVolume = .25f;
	public static AudioManager instance;
	
	void Awake()
	{
		if (instance == null) instance = this;
		else { // retain AudioManager on scene change
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		
		sounds = sounds.Concat(crashSoundsShort).ToArray();
		sounds = sounds.Concat(crashSoundsMed).ToArray();
		sounds = sounds.Concat(crashSoundsLong).ToArray();
		
		foreach(Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
		
		SetCrashVolume();
	}
	
	public void Play(string name) {
		Sound s = Array.Find(sounds, sounds => sounds.name == name);
		if (s == null) {
			Debug.LogWarning("Sound " + name + " not found.");
		}
		else if (s.source == null) {
			// Debug.LogWarning("Sound source for " + name + " not found. Check sources in Audio Manager.");
		}
		else {
			s.source.Play();
		}
	}
	
	public void Stop(string name) {
		Sound s = Array.Find(sounds, sounds => sounds.name == name);
		if (s == null) {
			Debug.LogWarning("Sound " + name + " not found.");
		}
		else {
			s.source.Stop();
		}
	}
	
	public void PlayRandomCrashShort() {
		System.Random random = new System.Random();
		crashSoundsShort[random.Next(0, crashSoundsShort.Length)].source.Play();
	}
	
	public void PlayRandomCrashMed() {
		System.Random random = new System.Random();
		Debug.Log(crashSoundsMed.Length);
		crashSoundsMed[random.Next(0, crashSoundsMed.Length - 1)].source.Play();
	}
	
	public void PlayRandomCrashLong() {
		System.Random random = new System.Random();
		crashSoundsLong[random.Next(0, crashSoundsLong.Length)].source.Play();
	}
	
	void SetCrashVolume() {
		
		Sound[] crashSounds = new Sound[0];
		crashSounds = crashSounds.Concat(crashSoundsShort).ToArray();
		crashSounds = sounds.Concat(crashSoundsMed).ToArray();
		crashSounds = sounds.Concat(crashSoundsLong).ToArray();
		
		foreach(Sound s in crashSounds) {
			s.source.volume = crashSoundVolume;
		}
	}
}
