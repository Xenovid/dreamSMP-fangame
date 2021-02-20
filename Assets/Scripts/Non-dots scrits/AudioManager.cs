using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioData[] dialoguess;
    public SoundData[] gameSoundss;
    public SoundData[] gameMusics;

    private static List<AudioData> dialogues = new List<AudioData>();
    private static List<SoundData> gameSounds = new List<SoundData>();
    private static List<SoundData> gameMusic = new List<SoundData>();

    private void Awake(){
        foreach( AudioData dialogue in dialoguess){
            int i = 0;
            foreach(AudioClip clip in dialogue.clips){
                dialogue.audioSources.Add(gameObject.AddComponent<AudioSource>());
                dialogue.audioSources[i].clip = clip;
                i++;
            }
            dialogues.Add(dialogue);
        }
    }


    /// <summary>
    /// starts playing a seleted piece of dialogue
    /// </summary>
    /// <param name="id">the parameter used to determine what chain of diague that you want to play from</param>
    /// <param name="index">the index of the dialogue in the chain of dialogue</param>
    public static void playDialogue(string audioName, int index){
        bool wasFound = false;
        foreach(AudioData dialogue in dialogues){
            Debug.Log("audio found");
            if(dialogue.audioName == audioName){
                try{
                    dialogue.audioSources[index].Play();
                    Debug.Log("played Dialogue");
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to play the dialogue");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("dialogue not found");
        }
    }

    /// <summary>
    /// stops playing a seleted piece of dialogue
    /// </summary>
    /// <param name="id">the parameter used to determine what chain of diague that you want to stop playing from</param>
    /// <param name="index">the index of the dialogue in the chain of dialogue</param>
    public static void stopDialogue(string audioName, int index){
        bool wasFound = false;
        foreach(AudioData dialogue in dialogues){
            if(dialogue.audioName == audioName){
                try{
                    dialogue.audioSources[index].Stop();
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to stop the dialogue");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("dialogue not found");
        }
    }

    /// <summary>
    /// starts playing a choosen sound
    /// </summary>
    /// <param name="name">the name of the sound</param>
    public static void playSound(string name){
        bool wasFound = false;
        foreach(SoundData sound in gameSounds){
            if(sound.soundName == name){
                try{
                    sound.audioSource.Play();
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to play the sound");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("sound not found");
        }
    }

    /// <summary>
    /// starts playing a choosen sound
    /// </summary>
    /// <param name="name">the name of the sound</param>
    public static void stopSound(string name){
        bool wasFound = false;
        foreach(SoundData sound in gameSounds){
            if(sound.soundName == name){
                try{
                    sound.audioSource.Stop();
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to stop the sound effect");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("sound effect not found");
        }
    }

    /// <summary>
    /// starts playing a choosen song
    /// </summary>
    /// <param name="name">the name of the sound</param>
    public static void playSong(string name){
        bool wasFound = false;
        foreach(SoundData sound in gameMusic){
            if(sound.soundName == name){
                try{
                    sound.audioSource.Play();
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to play the Music");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("Music not found");
        }
    }

    /// <summary>
    /// starts playing a choosen song
    /// </summary>
    /// <param name="name">the name of the sound</param>
    public static void stopSong(string name){
        bool wasFound = false;
        foreach(SoundData sound in gameMusic){
            if(sound.soundName == name){
                try{
                    sound.audioSource.Stop();
                }
                catch(Exception e){
                    Debug.Log("something went wrong when trying to stop the music");
                    Debug.Log(e);
                }
                finally{
                    wasFound = true;
                }
            }
        }
        if(!wasFound){
            Debug.Log("music not found");
        }
    }
}

