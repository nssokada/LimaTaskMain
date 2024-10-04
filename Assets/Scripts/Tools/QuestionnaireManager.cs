using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Include the TextMeshPro namespace

[System.Serializable]
public class QuestionResponse
{
    public float onsetTime;
    public float responseTime;
    public int response;
}

public class QuestionnaireManager : MonoBehaviour
{
    public TMP_Text questionText; // Use TMP_Text instead of Text
    public Button[] optionButtons;
    public Question[] questions;
    public string surveyName;
    public SessionGenerator sessionGenerator;

    private int currentQuestionIndex = 0;
    private List<QuestionResponse> questionResponses = new List<QuestionResponse>(); // List to store question responses

    void Start()
    {
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText;

            // Create a new QuestionResponse object and store the onset time
            QuestionResponse newResponse = new QuestionResponse();
            newResponse.onsetTime = Time.realtimeSinceStartup;
            questionResponses.Add(newResponse);

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < currentQuestion.options.Length)
                {
                    // Get the TMP_Text component from the button's child
                    TMP_Text buttonText = optionButtons[i].GetComponentInChildren<TMP_Text>();
                    buttonText.text = currentQuestion.options[i];
                    optionButtons[i].gameObject.SetActive(true);

                    int index = i; // Capture the index for the listener
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            EndQuestionnaire();
        }
    }

    void OnOptionSelected(int index)
    {
        // Record the response time and selected response for the current question
        QuestionResponse currentResponse = questionResponses[currentQuestionIndex];
        currentResponse.responseTime = Time.realtimeSinceStartup;
        currentResponse.response = index;

        currentQuestionIndex++;
        DisplayQuestion();
    }

    void EndQuestionnaire()
    {
        sessionGenerator.pushSurveyData(questionResponses, surveyName);
        // Handle the end of the questionnaire
        Debug.Log("Questionnaire completed.");
        // You can process the responses here
    }
}
