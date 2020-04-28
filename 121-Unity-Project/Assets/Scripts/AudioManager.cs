using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public List<string> songName = new List<string>{ "Theme1", "Theme2", "Theme3"};
    public System.Random random = new System.Random();
    public string justPlayed;
    public bool playingMusic;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        if (getSceneName() == "CoolHouse") 
        {
            int index = random.Next(songName.Count);
            Debug.Log("playing: "+ songName[index]);
            Play(songName[index]);
            justPlayed = songName[index];   
        }
        if (getSceneName() == "Offline") 
         {
             Play("Theme");
         }
        
    }

    void Update() {
         if (getSceneName() == "CoolHouse") 
         {
             foreach(Sound s in sounds) {
                 if (s.source.isPlaying && s.isMusic) {
                    playingMusic = true;
                 }
             }
            if (!playingMusic)
            {
                Debug.Log("can't hear");
                int index = random.Next(songName.Count);
                // make sure the next song is different from the one that just played
                while(songName[index] == justPlayed) {
                    index = random.Next(songName.Count);
                }
                Play(songName[index]);
                justPlayed = songName[index];
            }
         }

    }

    public string getSceneName(){
        Scene currentScene = SceneManager.GetActiveScene ();
        return currentScene.name;
    }

    public void Play(string name) {
        Sound toPlay = Array.Find(sounds, sound => sound.name == name);
        if (toPlay == null) {
            return;
        }
        toPlay.source.Play();
    }
}
