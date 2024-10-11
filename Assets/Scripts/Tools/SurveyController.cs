using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurveyController : MonoBehaviour
{
    int nextTrialType;
    // Start is called before the first frame update
    public GameObject introUI;
    public SessionGenerator sessionGenerator;

    int surveyIndex;
    int transitionState;
    public GameObject[] surveys;
    private GameObject[] currentSurveys;




    void Start()
    {
        PlayerPrefs.SetString("GameState","Survey");
        nextTrialType =PlayerPrefs.GetInt("nextTrialType");
        transitionState =PlayerPrefs.GetInt("transitionState");
        setCurrentSurveys(transitionState);
        sessionGenerator.SurveyButton();
        surveyIndex = 0;
    }


    public void ActionButton()
    {
        Debug.Log("Length:"+currentSurveys.Length);
        if (surveyIndex < currentSurveys.Length)
        {
            introUI.SetActive(false);
            currentSurveys[surveyIndex].SetActive(true);
            surveyIndex++;
        }
        else
        {
            switchBackToGame();
        }
    }





    public void switchBackToGame()
    {
        SceneManager.LoadScene("EffortCalibrator");
    }


    private void setCurrentSurveys(int transitionState)
    {
        if (transitionState == 0)
        {
            // Set currentSurveys to elements 0, 1, and 2 from the surveys array
            currentSurveys = new GameObject[3] { surveys[0], surveys[1], surveys[2] };
        }
        else
        {
            // Set currentSurveys to elements 3, 4, and 5 from the surveys array
            currentSurveys = new GameObject[3] { surveys[3], surveys[4], surveys[5] };
        }
    }
}
