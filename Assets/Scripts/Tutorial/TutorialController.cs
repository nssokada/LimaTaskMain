using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

public class TutorialController : MonoBehaviour
{
    public GameObject instructUI;
    public GameObject videoCanvas;
    public GameObject task;
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoOptions; // List of video clips to choose from
    private int currentVideoIndex = 0; // Track the currently selected video

    public string tutorialState;

    // Start is called before the first frame update
    void Start()
    {
        if (videoOptions.Count > 0)
        {
            videoPlayer.clip = videoOptions[currentVideoIndex];
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
            tutorialState = "tutorialIntro";
        }
    }

    public void buttonRN()
    {
        task.SetActive(true);
        // startScreen.SetActive(false);
    }

    // Method to change the video clip
    public void ChangeVideoClip(int index)
    {
        if (index >= 0 && index < videoOptions.Count)
        {
            currentVideoIndex = index;
            videoPlayer.clip = videoOptions[currentVideoIndex];
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Invalid video index selected.");
        }
    }

    // Example of using the method from a UI button


    public void SwitchToTask()
    {
         instructUI.SetActive(false);
         task.SetActive(true);

    }
    public void NextVideo()
    {
        int nextIndex = (currentVideoIndex + 1) % videoOptions.Count;
        ChangeVideoClip(nextIndex);
    }
    public void WatchAgain()
    {
        instructUI.SetActive(false);
        videoCanvas.SetActive(true);
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.time = 0; // Set the time to 0
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
     {
            Debug.Log("Switching Screen");
            instructUI.SetActive(true);
            videoCanvas.SetActive(false);
            videoPlayer.loopPointReached -= OnVideoEnd;
     }


}
