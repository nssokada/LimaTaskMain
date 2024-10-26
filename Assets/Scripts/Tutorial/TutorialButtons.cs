using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void navigationTutorial()
    {
        //if the navigation is passed move them to the next scene. If it is failed then fail them
        float totalScore = PlayerPrefs.GetFloat("TotalScore");
        if(totalScore<110f)
        {
            PlayerPrefs.SetString("FailReason","Unfortunately, your performance on the game did not qualify for our study.");
            PlayerPrefs.SetString("CompletionPath","https://app.prolific.com/submissions/complete?cc=C1DWO1L4");
            SceneManager.LoadScene("EndGame");
        }
        else
        {
            SceneManager.LoadScene("MainTutorial");
        }

    }

    public void mainTutorial()
    {
        float startTime = PlayerPrefs.GetFloat("StartTime");
        float timeSpent = Time.realtimeSinceStartup - startTime;
        
        if(timeSpent>500f)
        {
            PlayerPrefs.SetString("FailReason","Unfortunately, your performance on the game did not qualify for our study");
            PlayerPrefs.SetString("CompletionPath","https://app.prolific.com/submissions/complete?cc=C1BHFIGB");
            SceneManager.LoadScene("EndGame");
        }
        else
        {
            PlayerPrefs.SetFloat("TotalScore", 0f);
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            SceneManager.LoadScene("EffortCalibrator");
        }
    }



}
