using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        MonoAudioManager.instance.PlaySound("SplashCreenBG", true, true);
    }
    public void LoadScene(string sceneName)
    {
        LoadingScreenManager.instance.LoadScene(sceneName);
        
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        MonoAudioManager.instance.StopSound("SplashCreenBG", true);
    }
}
