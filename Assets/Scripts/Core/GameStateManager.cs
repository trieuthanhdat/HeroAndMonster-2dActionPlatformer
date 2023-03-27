using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
   
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject tutorialPanel;

    private HeroKnight player;
    private bool isPaused;
    private bool isGameOver;
    private bool isGameWin;

    public enum GameStates
    {
        GAMEPLAY,
        GAMEPAUSE,
        GAMEOVER,
        GAMEWON,
        NONE
    }
    
    private GameStates currentGameState;
    public GameStates CurrentGameState {get => currentGameState; set => currentGameState = value;}

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        MonoAudioManager.instance.PlaySound("GameplayBG", true, true);
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        MonoAudioManager.instance.StopSound("GameplayBG", true);
        MonoAudioManager.instance.StopSound("BossTheme", true);
    }
    void Start()
    {
        isPaused = false;
        isGameOver = false;
        isGameWin = false;

        if(!pausePanel)
            pausePanel = GameObject.FindGameObjectWithTag("PausePanel");
        pausePanel .SetActive(false);
        if(!gameOverPanel)
            gameOverPanel = GameObject.FindGameObjectWithTag("LosePanel");
        gameOverPanel.SetActive(false);
        if(!gameWinPanel)
            gameWinPanel = GameObject.FindGameObjectWithTag("WinPanel");
        gameWinPanel.SetActive(false);
        
        int temp = PlayerPrefs.GetInt("IsDoneTutorial", 0);
        if(temp ==  0)
            OpenTutorial();
        else
            tutorialPanel.SetActive(false);

        player = GameObject.FindObjectOfType<HeroKnight>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused && !isGameOver && !isGameWin)
            {
                PauseGame();
            }
            else if (isPaused && !isGameOver && !isGameWin)
            {
                ResumeGame();
            }
        }
        if(player )
        {
            if(player.GetComponent<Health>().IsDead())
                Invoke("GameOver",1);
        }

    }
    public void OpenTutorial()
    {
        isPaused = true;
        Time.timeScale =  0;
        tutorialPanel.SetActive(true);
        PlayerPrefs.SetInt("IsDoneTutorial", 1);
    }
    public void CloseTutorial()
    {
        isPaused = false;
        Time.timeScale =  1;
        tutorialPanel.SetActive(false);
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale =  0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
    }

    public void GameWin()
    {
        isGameWin = true;
        gameWinPanel.SetActive(true);
    }
    public void Restart()
    {
        Time.timeScale = 1;
         LoadingScreenManager.instance.LoadScene("Level1");
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        LoadingScreenManager.instance.LoadScene("MainMenu");
    }
}
