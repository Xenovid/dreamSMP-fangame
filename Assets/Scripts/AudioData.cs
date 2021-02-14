using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData{
    public AudioClip[] clips;
    [HideInInspector]
    public List<AudioSource> audioSources = new List<AudioSource>();
    public string audioName;
}
