using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
public class EffortCalibrator : MonoBehaviour
{
    
    PlayerInput playerInput;
    public GameObject HeadsUpDisplay;
    public GameObject instructUI;
    public TMP_Text instructText;
    int counter;
    bool pressable;
    int repeatNum;
    private List<float> pressTimes = new List<float>();
    private List<float> meanLatencies = new List<float>();


    // Start is called before the first frame update
    public void beginButton()
    {
        if(repeatNum<3)
        {
            HeadsUpDisplay.SetActive(true);
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.08f);
            counter = 0;
            resetTimer();
            StartCoroutine(EffortCalibratorCoroutine());
            instructUI.SetActive(false);
        }
        else
        {
            Debug.Log("Press Latency: " + CalculateAverage(meanLatencies) + " seconds");
            PlayerPrefs.SetFloat("PressLatency", CalculateAverage(meanLatencies));
            SceneManager.LoadScene("GrassTest");
        }
    }

    void resetCalibrator()
    {
        CancelInvoke("UpdateTimer");
        repeatNum++;
        resetTimer();
        HeadsUpDisplay.SetActive(false);
        instructUI.SetActive(true);
        if(repeatNum>2)
        {
            instructText.text = "Great work! Now press \"Next\" to begin the game.";
        }
    }

    IEnumerator EffortCalibratorCoroutine()
    {
        InvokeRepeating("UpdateTimer", 0f, 0.01f);
        pressable=true;
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Current Average Latency: " + CalculateAverage(pressTimes, true) + " seconds");
        meanLatencies.Add(CalculateAverage(pressTimes, true));
        yield return new WaitForSeconds(1.0f);
        resetCalibrator();
    }

    // Update is called once per frame
    void Update()
    {
        HeadsUpDisplay.GetComponent<UIController>().SetEnergyText(counter);
    }

    void OnEffort()
    {
        if(pressable)
        {
            HeadsUpDisplay.GetComponent<UIController>().IncreaseEnergy(0.01f);


            //Catch values for Latency Calculation:
            float pressTime = Time.time;
            pressTimes.Add(pressTime);

            counter++;
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



    #region Continous Methods

    void UpdateTimer()
        {
            // Assuming movementStartTimeHeadsUpDisplay is a GameObject with UIController script attached
            UIController uiController = HeadsUpDisplay.GetComponent<UIController>();

            if (uiController != null)
            {
                // Call the DecreaseTime method from UIController
                uiController.DecreaseTime(0.01f/5f);
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
