using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoSingleton<MainMenuManager>
{

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
    }

    public void ShowOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void HideOptionsMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ShowConfirmExitPanel()
    {
        mainMenuPanel.SetActive(false);
        confirmExitPanel.SetActive(true);
    }

    public void HideConfirmExitPanel()
    {
        mainMenuPanel.SetActive(true);
        confirmExitPanel.SetActive(false);
    }

    public void ShowConfirmNewGamePanel()
    {
        mainMenuPanel.SetActive(false);
        confirmNewGamePanel.SetActive(true);
    }

    public void HideConfirmNewGamePanel()
    {
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
}
