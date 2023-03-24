using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoSingleton<OptionsManager>
{

    public AudioSource bgmAudioSource;
    public List<MonoAudioPlayer> sfxAudioSource;
    public Toggle bgmToggle;
    public Toggle sfxToggle;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private bool isBgmOn;
    private bool isSfxOn;

    
    void Start()
    {

        isBgmOn = true;
        isSfxOn = true;

        bgmToggle.isOn = true;
        sfxToggle.isOn = true;

        bgmSlider.value = bgmAudioSource.volume;
        

        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }
    public void AddAudioSFXs(MonoAudioPlayer sound)
    {
        sfxAudioSource.Add(sound);
    }
    public void ToggleBgm()
    {
        if (isBgmOn)
        {
            bgmAudioSource.Pause();
        }
        else
        {
            bgmAudioSource.Play();
        }
        isBgmOn = !isBgmOn;
    }
   
    public void ToggleSfx()
    {
        if (isBgmOn)
        {
            SetSfxVolume(0);
        }
        else
        {
            SetSfxVolume(sfxSlider.value);
        }
        isSfxOn = !isSfxOn;
    }

    public bool IsBgmOn()
    {
        return isBgmOn;
    }

    public bool IsSfxOn()
    {
        return isSfxOn;
    }


    public void SetBgmVolume(float volume)
    {
        bgmAudioSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        if(sfxAudioSource.Count <=0 || sfxAudioSource == null) return;
       foreach(MonoAudioPlayer  player in sfxAudioSource)
       {
            player.OriginVolume = volume*player.OriginVolume / 100;
       }
    }
}
