using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurveyController : MonoBehaviour
{
    int nextTrialType;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("GameState","Survey");
        nextTrialType =PlayerPrefs.GetInt("nextTrialType");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchBackToGame()
    {
        SceneManager.LoadScene("EffortCalibrator");

        // if (nextTransitionState==1)
        // {
        //     SceneManager.LoadScene("AcornTutorial");
        // }
        // else
        // {
        //     SceneManager.LoadScene("MainGame");
        // }
    }
}
