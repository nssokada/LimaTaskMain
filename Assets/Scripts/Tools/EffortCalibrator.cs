using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
public class EffortCalibrator : MonoBehaviour
{

    PlayerInput playerInput;
    public GameObject HeadsUpDisplay;
    public GameObject instructUI;
    public TMP_Text instructText;
    public GameObject SessionGenerator;
    int counter;
    bool pressable;
    int repeatNum;
    private List<float> pressTimes = new List<float>();
    private List<float> meanLatencies = new List<float>();
    private List<int> maxPressCount = new List<int>();

    int nextTrialType;



    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("GameState", "EffortCalibrator");
        PlayerPrefs.Save();

        if (PlayerPrefs.HasKey("nextType"))
        {
            nextTrialType = PlayerPrefs.GetInt("nextType");
        }
        else if (PlayerPrefs.GetInt("TutorialCompleted") == 0)
        {
            nextTrialType = 10;
        }

    }

    public void beginButton()
    {
        if (repeatNum < 3)
        {
            HeadsUpDisplay.SetActive(true);
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.08f);
            counter = 0;
            pressTimes = new List<float>();
            resetTimer();
            instructUI.SetActive(false);
        }
        else
        {
            Debug.Log("Press Latency: " + CalculateAverage(meanLatencies) + " seconds");
            float pressLatency = CalculateAverage(meanLatencies);
            PlayerPrefs.SetFloat("PressLatency", pressLatency);

            float MinPressCount = GetMaxPressCount(maxPressCount); //lets change this to max press count
            PlayerPrefs.SetFloat("PressCount", MinPressCount);
            PlayerPrefs.Save();

            if (MinPressCount < 10f)
            {
                PlayerPrefs.SetString("FailReason", "Unfortunately, you did not press fast enough to pass the attention check. Please return to Prolific and return the study.");
                PlayerPrefs.SetString("CompletionPath", "https://app.prolific.com/submissions/complete?cc=C1FU3LPS");
                SceneManager.LoadScene("EndGame");
            }
            else
            {
                if (nextTrialType == 4)
                {
                    SceneManager.LoadScene("AcornTutorial");
                }
                else if (nextTrialType == 10)
                {
                    SceneManager.LoadScene("MoveTutorial");
                }
                else
                {
                    SceneManager.LoadScene("MainGame");
                }
            }
        }
    }

    void resetCalibrator()
    {
        CancelInvoke("UpdateTimer");
        repeatNum++;
        resetTimer();
        HeadsUpDisplay.SetActive(false);
        instructUI.SetActive(true);
        if (repeatNum > 2)
        {
            instructText.text = "Great work! Now press \"Next\" to begin the game.";
        }
    }

    IEnumerator EffortCalibratorCoroutine()
    {
        InvokeRepeating("UpdateTimer", 0f, 0.01f);
        pressable = true;
        yield return new WaitForSeconds(10.0f);
        Debug.Log("Current Average Latency: " + CalculateAverage(pressTimes, true) + " seconds");
        Debug.Log(counter);
        meanLatencies.Add(CalculateAverage(pressTimes, true));
        maxPressCount.Add(counter);
        yield return new WaitForSeconds(0.5f);
        resetCalibrator();
    }

    // Update is called once per frame
    void Update()
    {
        HeadsUpDisplay.GetComponent<UIController>().SetEnergyText(counter);
    }

    void OnEffort()
    {
        if (counter == 0)
        {
            StartCoroutine(EffortCalibratorCoroutine());
        }
        if (pressable)
        {
            if (Keyboard.current.sKey.isPressed && Keyboard.current.dKey.isPressed && Keyboard.current.fKey.isPressed)
            {
                HeadsUpDisplay.GetComponent<UIController>().IncreaseEnergy(0.01f);
                //Catch values for Latency Calculation:
                float pressTime = Time.time;
                pressTimes.Add(pressTime);
                counter++;
            }

        }

    }



    private float CalculateAverage(List<float> latencies, bool isConsecutiveDifferences = false)
    {
        if (latencies.Count == 0 || (isConsecutiveDifferences && latencies.Count < 2))
        {
            return 0f;
        }

        float total = 0f;
        if (isConsecutiveDifferences)
        {
            for (int i = 1; i < latencies.Count; i++)
            {
                total += (latencies[i] - latencies[i - 1]);
            }
            return total / (latencies.Count - 1);
        }
        else
        {
            foreach (float latency in latencies)
            {
                total += latency;
            }
            return total / latencies.Count;
        }
    }

    private float GetMeanPressCount(List<int> meanPressCount)
    {
        if (meanPressCount == null || meanPressCount.Count == 0)
        {
            // Return 0 if the list is null or empty to avoid division by zero
            return 0f;
        }

        // Calculate the sum of all press counts
        int sum = 0;
        foreach (int pressCount in meanPressCount)
        {
            sum += pressCount;
        }

        // Calculate the mean by dividing the sum by the count of elements in the list
        float mean = (float)sum / meanPressCount.Count;

        return mean;
    }

    private float GetMaxPressCount(List<int> pressCounts)
    {
        if (pressCounts == null || pressCounts.Count == 0)
        {
            return 0f;
        }

        return (float)pressCounts.Max();
    }



    #region Continous Methods

    void UpdateTimer()
    {
        // Assuming movementStartTimeHeadsUpDisplay is a GameObject with UIController script attached
        UIController uiController = HeadsUpDisplay.GetComponent<UIController>();

        if (uiController != null)
        {
            // Call the DecreaseTime method from UIController
            uiController.DecreaseTime(0.01f / 10f);
        }
    }


    void resetTimer()
    {
        // Assuming movementStartTimeHeadsUpDisplay is a GameObject with UIController script attached
        UIController uiController = HeadsUpDisplay.GetComponent<UIController>();

        if (uiController != null)
        {
            // Call the DecreaseTime method from UIController
            uiController.SetTime(1f);
        }
    }
    #endregion

}
