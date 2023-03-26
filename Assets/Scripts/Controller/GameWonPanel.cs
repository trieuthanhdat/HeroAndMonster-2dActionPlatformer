using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWonPanel : MonoBehaviour
{
   private void OnEnable()
    {
        MonoAudioManager.instance.PlaySound("GameWon", true ,true);
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        MonoAudioManager.instance.StopSound("GameWon", true);
    }
}
