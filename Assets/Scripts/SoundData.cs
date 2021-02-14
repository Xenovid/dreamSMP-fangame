using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundData{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource audioSource;
    public string soundName;
}
