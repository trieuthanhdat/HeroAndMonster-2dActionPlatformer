using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.UI
{
    public class ToggleUI : MonoBehaviour
    {
        GameStateManager gameStateManager;
       public KeyCode toggleKey = KeyCode.Escape;
        public GameObject togglePanel;

        private bool isPaused;

        void Start()
        {
            isPaused = false;
            togglePanel.SetActive(false);
            gameStateManager = GameObject.FindObjectOfType<GameStateManager>();
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0;
                togglePanel.SetActive(true);
                gameStateManager.CurrentGameState = GameStateManager.GameStates.GAMEPAUSE;
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1;
                togglePanel.SetActive(false);
                gameStateManager.CurrentGameState = GameStateManager.GameStates.GAMEPLAY;
            }
        }
    }

}
