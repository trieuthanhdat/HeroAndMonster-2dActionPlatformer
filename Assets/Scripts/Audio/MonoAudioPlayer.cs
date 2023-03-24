using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Time = UnityEngine.Time;

public class MonoAudioPlayer : MonoBehaviour
{
    public Sound sound;
    public float fadeInTimer = 2f;
    public float fadeOutTimer = 2f;
    public bool isMute = false;

    float timeGradient = 0;
    bool startPlaying = false;
    bool stopPlaying = false;
    float originVolume = 1;

    public float OriginVolume{get => originVolume; set => originVolume = value;}
    private void Start()
    {
        originVolume = sound.audioSrc.volume;
         if(isMute) return;
        if (sound.playOnAwake)
            PlaySound();
    }

    private void Update()
    {
        if(isMute) return;
        if(startPlaying && !stopPlaying)
        {
            if(sound.isFadeIntGradient)
            {
                PlayFadeInOutEffect(true);
            }
            else
            {
                sound.audioSrc.Play();
                startPlaying = false;
            }
        }

        if(stopPlaying && !startPlaying)
        {
            if (sound.isFadeOutGradient)
            {
                PlayFadeInOutEffect(false);
            }
            else
            {
                sound.audioSrc.Stop();
                stopPlaying = false;
            }
        }
    }

    public void PlaySound(float delay = 0)
    {
        ResetCounter();
        
        startPlaying = true;
        stopPlaying = false;

        if(delay >0)
            sound.audioSrc.PlayDelayed(delay);
        else
            sound.audioSrc.Play();
    }
    public void StopSound()
    {
        if (!sound.audioSrc.isPlaying) return;
        ResetCounter();
        stopPlaying = true;
        startPlaying = false;
    }
    public void ResetCounter()
    {
        timeGradient = 0;
    }

    private void PlayFadeInOutEffect(bool isStart)
    {
        sound.audioSrc.volume = isStart == true ? 0 : originVolume;
        
        timeGradient += Time.deltaTime;
        
        if (isStart)
        {
            sound.audioSrc.volume += timeGradient / fadeInTimer;
            if (sound.audioSrc.volume >= originVolume)
            {
                startPlaying = false;
            }
        }
        else
        {
            sound.audioSrc.volume -= timeGradient / fadeOutTimer;
            if(sound.audioSrc.volume <= 0)
            {
                stopPlaying = false;
                sound.audioSrc.Stop();
            }
        }


    }
}