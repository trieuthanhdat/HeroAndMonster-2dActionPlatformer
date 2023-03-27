using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager :MonoBehaviour
{
    public OptionsManager optionsManager;
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject confirmExitPanel;
    public GameObject confirmNewGamePanel;

    private bool isGamePaused;

   

    void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);
        confirmNewGamePanel.SetActive(false);
        isGamePaused = false;
        optionsManager.CheckBgmToggle();
        MonoAudioManager.instance.PlaySound("LoadingBG", true, true);
    }

    public void ShowOptionsMenu()
    {
        MonoAudioManager.instance.PlaySound("ButtonConfirm");
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void HideOptionsMenu()
    {
        MonoAudioManager.instance.PlaySound("ButtonDecline");
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ShowConfirmExitPanel()
    {
        MonoAudioManager.instance.PlaySound("ButtonConfirm");
        mainMenuPanel.SetActive(false);
        confirmExitPanel.SetActive(true);
    }

    public void HideConfirmExitPanel()
    {
        MonoAudioManager.instance.PlaySound("ButtonDecline");
        mainMenuPanel.SetActive(true);
        confirmExitPanel.SetActive(false);
    }

    public void ShowConfirmNewGamePanel()
    {
        MonoAudioManager.instance.PlaySound("ButtonDecline");
        mainMenuPanel.SetActive(false);
        confirmNewGamePanel.SetActive(true);
    }

    public void HideConfirmNewGamePanel()
    {
        MonoAudioManager.instance.PlaySound("ButtonDecline");
        mainMenuPanel.SetActive(true);
        confirmNewGamePanel.SetActive(false);
    }

    public void StartNewGame()
    {
        
        LoadingScreenManager.instance.LoadScene("ChooseLevel");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        HideOptionsMenu();
    }

    public void ExitGame()
    {
        MonoAudioManager.instance.PlaySound("ButtonDecline");
        Application.Quit();
    }

    public void TogglePauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            ShowOptionsMenu();
        }
        else
        {
            Time.timeScale = 1;
            isGamePaused = false;
            HideOptionsMenu();
        }
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        MonoAudioManager.instance.StopSound("LoadingBG", true);
    }
}
