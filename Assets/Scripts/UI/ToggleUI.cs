using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using static GameStateManager;

namespace RPG.UI
{
    public class ToggleUI : MonoBehaviour
    {
       public KeyCode toggleKey = KeyCode.Escape;
        public GameObject togglePanel;

        private bool isPaused;

        void Start()
        {
            isPaused = false;
            togglePanel.SetActive(false);
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
                GameStateManager.instance.CurrentGameState = GameStates.GAMEPAUSE;
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1;
                togglePanel.SetActive(false);
                GameStateManager.instance.CurrentGameState = GameStates.GAMEPLAY;
            }
        }
    }

}
