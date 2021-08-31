using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public SoundData[] dialoguess;
    public SoundData[] gameSoundss;
    public SoundData[] gameMusics;

    private static List<SoundData> dialogues = new List<SoundData>();
    private static List<SoundData> gameSounds = new List<SoundData>();
    private static List<SoundData> gameMusic = new List<SoundData>();
    private static SoundData currentSong;

    public static float volume = .1f;
    
    public static void changeVolume(float newVolume){
        volume = newVolume;

        foreach(SoundData sound in gameSounds)
        {
            sound.audioSource.volume = newVolume;
        }

        foreach(SoundData music in gameMusic)
        {
            music.audioSource.volume = newVolume;
        }

        foreach(SoundData dialogue in dialogues)
        {
            dialogue.audioSource.volume = newVolume;
        }
    }

    private void Awake(){
        foreach( SoundData dialogue in dialoguess){
            dialogue.audioSource = gameObject.AddComponent<AudioSource>();
            dialogue.audioSource.clip = dialogue.clip;
            dialogues.Add(dialogue);
        }

        foreach(SoundData music in gameMusics){
            music.audioSource = gameObject.AddComponent<AudioSource>();
            music.audioSource.clip = music.clip;
            music.audioSource.loop = true;
            gameMusic.Add(music);
        }

        foreach(SoundData sound in gameSoundss){
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            gameSounds.Add(sound);
        }
    }


    /// <summary>
    /// starts playing a seleted piece of dialogue
    /// </summary>
    /// <param name="soundName">the parameter used to determine what dialogue that you want to play from</param>
    public static void playDialogue(string soundName){
        bool wasFound = false;
        foreach(SoundData dialogue in dialogues){
            if(dialogue.soundName == soundName){
                try{
                    dialogue.audioSource.volume = volume;
                    dialogue.audioSource.Play();
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
            playDialogue("default");
            Debug.Log("dialogue not found");
        }
    }

    /// <summary>
    /// stops playing a seleted piece of dialogue
    /// </summary>
    /// <param name="id">the parameter used to determine what chain of diague that you want to stop playing from</param>
    /// <param name="index">the index of the dialogue in the chain of dialogue</param>
    public static void stopDialogue(string soundName){
        bool wasFound = false;
        foreach(SoundData dialogue in dialogues){
            if(dialogue.soundName == soundName){
                try{
                    dialogue.audioSource.Stop();
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
                    sound.audioSource.volume = volume;
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
            Debug.Log(name);
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
    public static void stopCurrentSong(){
        if(currentSong != null){
            stopSong(currentSong.soundName);
        }
    }
    /// <summary>
    /// stop the current music and plays new music
    /// </summary>
    /// <param name="name">the name of the sound</param>
    public static void playSong(string name){
        bool wasFound = false;
        foreach(SoundData sound in gameMusic){
            //Debug.Log("found matching name");
            if(sound.soundName == name){
                try{
                    sound.audioSource.volume = volume;
                    stopCurrentSong();
                    sound.audioSource.Play();
                    
                    currentSong = sound;
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
    public static string GetCurrentSongName(){
        return currentSong.soundName;
    }
}

