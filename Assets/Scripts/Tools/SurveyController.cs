using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Include the TextMeshPro namespace

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
    public TMP_Text introText; // Use TMP_Text instead of Text



    void OnEnable()
    {
        if (currentSurveys != null && currentSurveys.Length > 0)
        {
            if (surveyIndex >= currentSurveys.Length)
            {
                introText.text = "Thank you for your responses! We will now return to the game.";
            }
        }
    }


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
        if (transitionState == 2)
        {
            // Set currentSurveys to elements 0, 1, and 2 from the surveys array
            currentSurveys = new GameObject[3] { surveys[0], surveys[1], surveys[2] };
        }
        else if(transitionState == 3)
        {
            // Set currentSurveys to elements 3, 4, and 5 from the surveys array
            currentSurveys = new GameObject[3] { surveys[3], surveys[4], surveys[5] };
        }
        else
        {
             currentSurveys = new GameObject[2]{ surveys[6], surveys[7]};
        }
    }
}
