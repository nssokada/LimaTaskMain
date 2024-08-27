using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class TutorialController : MonoBehaviour
{
    public GameObject instructUI;
    public GameObject videoCanvas;
    public GameObject task;
    public VideoPlayer videoPlayer;
    public string[] videoOptions; // List of video clips to choose from
    private int currentVideoIndex = 0; // Track the currently selected video

    public string tutorialState;

    public TMP_Text instructText;


    // Start is called before the first frame update
    void Start()
    {
        if (videoOptions.Length > 0)
        {
            StartCoroutine(LoadAndPlayVideo(videoOptions[currentVideoIndex]));
            tutorialState = "cookieSelection";
        }
    }

    IEnumerator LoadAndPlayVideo(string videoFileName)
    {
        string videoUrl = GetStreamingAssetsPath(videoFileName);

        videoPlayer.url = videoUrl;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    string GetStreamingAssetsPath(string fileName)
    {
        string path = System.IO.Path.Combine("https://nssokada.github.io/testVideoRepo/", fileName);

        return path;
    }

    // Method to change the video clip
   // Method to change the video clip
    public void ChangeVideoClip(int index)
    {
        if (index >= 0 && index < videoOptions.Length)
        {
            currentVideoIndex = index;
            StartCoroutine(LoadAndPlayVideo(videoOptions[currentVideoIndex]));
        }
        else
        {
            Debug.LogWarning("Invalid video index selected.");
        }
    }


    // Example of using the method from a UI button


    public void SwitchToTask()
    {
            ToggleTask();
    }
    
    public void ToggleTask()
    {
        bool isTaskActive = task.activeSelf;

        instructUI.SetActive(isTaskActive);
        task.SetActive(!isTaskActive);
    }

    public void ToggleUIAndVideoCanvas()
    {
        bool isInstructUIActive = instructUI.activeSelf;

        // Toggle the active states of instructUI and videoCanvas
        instructUI.SetActive(!isInstructUIActive);
        videoCanvas.SetActive(isInstructUIActive);

        switch (tutorialState)
        {
            case "cookieSelection":
                instructText.text = "Press \"Next\" to start practicing how to select cookies.\n Press \"Watch Again\" if you need to review the instructions on selecting cookies in the game.";
                break;
            case "effortIntro":
                instructText.text = "Press \"Next\" to start practicing how to adjust the acceleration.\n Press \"Watch Again\" if you need to review the instructions on adjusting the acceleration in the game.";
                break;
            case "navigationTutorial":
                instructText.text = "Press \"Next\" to start practicing how to select cookies.\n Press \"Watch Again\" if you need to review the instructions on selecting cookies in ther game.";
                break;
            case "cookieTutorial":
                instructText.text = "Press \"Next\" to start practicing with the different cookies.\n Press \"Watch Again\" if you need to review the instructions on selecting cookies in ther game.";
                break;
            case "mapTutorial":
                instructText.text = "Press \"Next\" to start practicing the game.\n Press \"Watch Again\" if you need to review the instructions on about the game.";
                break;
            case "acornTutorial":
                instructText.text = "Press \"Next\" to start practicing the free movement trial.\n Press \"Watch Again\" if you need to review the instructions on about the free movement trial.";
                break;
        }
    }

    public void SwitchScene()
    {
            SceneManager.LoadScene(0);
    }
    
    public void NextVideo(string state)
    {
        tutorialState = state;
        int nextIndex = (currentVideoIndex + 1) % videoOptions.Length;
        ChangeVideoClip(nextIndex);
        ToggleUIAndVideoCanvas();
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
      
    }
    public void WatchAgain()
    {
        ToggleUIAndVideoCanvas();
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.time = 0; // Set the time to 0
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
     {
            Debug.Log("Switching Screen");
            ToggleUIAndVideoCanvas();
            videoPlayer.loopPointReached -= OnVideoEnd;
     }

}



