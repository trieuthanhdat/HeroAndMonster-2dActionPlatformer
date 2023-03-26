using UnityEngine.Video;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    private float timer = 60f;
    bool hasLoaded = false;
    // Start is called before the first frame update
    private void Start()
    {
        videoPlayer.Play();
    }
   
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            if(hasLoaded == false)
                EnterGame();
        }
    }
    private void EnterGame()
    {
        hasLoaded = true;
        videoPlayer.gameObject.SetActive(false);
        LoadingScreenManager.instance.LoadScene("MainMenu");
        // Do something here, e.g. load next scene or show a menu
    }
}
