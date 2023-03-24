using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoSingleton<LoadingScreenManager>
{
    public GameObject loadingScreen;
    public Image fader;
    public TextMeshProUGUI loadingPercentText;
    public Sprite[] loadingSprites; // array of loading screen sprites
    public float loadTime = 2f;

    private AsyncOperation asyncOperation;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        HideLoadingScreenElements();
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

   IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show loading screen elements
        ShowLoadingScreenElements();
        // randomly select a sprite from the array
        int index = Random.Range(0, loadingSprites.Length);
        Image loadingImage = loadingScreen.GetComponent<Image>();
        loadingImage.sprite = loadingSprites[index];

        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        // Wait for a short delay to show the loading screen elements
        float delayTime = 0.5f;
        yield return new WaitForSeconds(delayTime);

        // Start the scene loading process
        float elapsedTime = 0f;
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, progress);
            loadingPercentText.text = Mathf.RoundToInt(progress * 100f).ToString() + "%";

            if (asyncOperation.progress >= 0.9f)
            {
                // If the scene is almost loaded, wait for the fader to fade out before allowing scene activation
                float fadeOutTime = 0.5f;
                while (elapsedTime < delayTime + fadeOutTime)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Hide loading screen elements
        HideLoadingScreenElements();
    }
    void ShowLoadingScreenElements()
    {
        // Show loading screen elements
        loadingScreen.SetActive(true);
        fader.gameObject.SetActive(true);
        fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, 1f); // Set the fader alpha to 1
        loadingPercentText.gameObject.SetActive(true);
        loadingPercentText.text = "0%"; // Set the initial percent to 0

    }
    void HideLoadingScreenElements()
    {
        loadingScreen.SetActive(false);
        fader.gameObject.SetActive(false);
        loadingPercentText.gameObject.SetActive(false);
    }

}
