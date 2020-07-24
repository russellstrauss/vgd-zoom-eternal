﻿using System;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable] // show custom class in inspector
public class Sound
{
	public string name;
	public AudioClip clip;
	[Range(0f, 5f)]
	public float volume = 1;
	[HideInInspector]
	public float defaultVolume;
	[Range(.1f, 3f)]
	public float pitch;
	public bool loop;
	[HideInInspector]
	public AudioSource source;
}
