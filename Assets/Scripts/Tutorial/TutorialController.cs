using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
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

    public TMP_Text instructText;


    // Start is called before the first frame update
    void Start()
    {
        if (videoOptions.Count > 0)
        {
            videoPlayer.clip = videoOptions[currentVideoIndex];
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
            tutorialState = "acornTutorial";
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
            videoPlayer.loopPointReached += OnVideoEnd;
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
        }
    }

    
    public void NextVideo(string state)
    {
        tutorialState = state;
        int nextIndex = (currentVideoIndex + 1) % videoOptions.Count;
        ToggleUIAndVideoCanvas();
        ChangeVideoClip(nextIndex);
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
