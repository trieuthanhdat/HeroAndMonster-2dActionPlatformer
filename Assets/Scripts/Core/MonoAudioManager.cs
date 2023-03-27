using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool playOnAwake;

    [Range(0,1)]
    public float volume = 1;
    [Range(0f, 1f)]
    public float pitch = 0.7f;
    [Header("SETTINGS FOR GRADIENT CLIP")]
    public float fadeInTimer = 20;
    public float fadeOutTimer = 20;

    [HideInInspector]
    public AudioSource audioSrc;
    [HideInInspector]
    public bool isFadeIntGradient = false;
    [HideInInspector]
    public bool isFadeOutGradient = false;
    [HideInInspector]
    public MonoAudioPlayer player;
}

public class MonoAudioManager : MonoBehaviour
{
    private static MonoAudioManager _instance;

    private int score;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static MonoAudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<MonoAudioManager>();
            }

            return _instance;
        }
    }
    [SerializeField] MonoAudioPlayer audioPlayerPrefabs;
    [SerializeField] Sound[] sounds;

    bool isGradient = false;
    int gradientSoundIndex = -1;
    float time = 0;
    AudioSource currentBGSound;
    private void Start()
    {
        foreach(Sound s in sounds)
        {
            //SETUP PLAYER
            s.player = Instantiate(audioPlayerPrefabs, transform);
            s.player.sound = s;
            s.player.fadeInTimer = s.fadeInTimer;
            s.player.fadeOutTimer = s.fadeOutTimer;

            //SETUP SOUND
            s.audioSrc = s.player.gameObject.AddComponent<AudioSource>();
            s.audioSrc.clip = s.clip;
            s.audioSrc.playOnAwake = s.playOnAwake;
            s.audioSrc.volume = s.volume;
            s.audioSrc.pitch = s.pitch;
        }
       
    }
    
    public void PlaySound(string name,bool isLoop = false, bool isGradient = false, float delay = 0)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound name: "+name +" is missing!!!");
            return;
        }
        s.audioSrc.loop = isLoop;
        s.isFadeIntGradient = isGradient;
        s.player.PlaySound(delay);
    }

    public void StopSound(string name, bool isGradient = false, float modifiedFadeoutTime = -1)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound name: " + s.name + " is missing!!!");
            return;
        }
        s.isFadeOutGradient = isGradient;

        if(modifiedFadeoutTime > -1)
            s.fadeOutTimer = modifiedFadeoutTime;
        s.player.StopSound();
    }   
    public void DisableAllBGSound()
    {
        foreach(Sound s in sounds)
        {
            if(s.name.Contains("BG"))
                if(s.audioSrc.isPlaying)
                {
                    currentBGSound = s.audioSrc;
                    s.audioSrc.Pause();
                }
                    
        }
    }
    public void EnableCurrentBGSound()
    {
        foreach(Sound s in sounds)
        {
            if(s.name.Contains("BG"))
                if(currentBGSound == s.audioSrc)
                    s.audioSrc.Play();
        }
    }

}


