using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        LoadingScreenManager.instance.LoadScene(sceneName);
    }
}
