using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoSingleton<GameStateManager>
{

    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

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


    void Start()
    {
        isPaused = false;
        isGameOver = false;
        isGameWin = false;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);

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
         LoadingScreenManager.instance.LoadScene("Level1");
    }
    public void BackToMainMenu()
    {
        LoadingScreenManager.instance.LoadScene("MainMenu");
    }
}
