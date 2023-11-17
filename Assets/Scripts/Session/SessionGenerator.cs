using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionGenerator : MonoBehaviour
{
    
    public GameObject Task;
    public GameObject SessionScreen;


    CSVReader reader;
    public string conditionFile;


    public List<LimaTrial> trials;
    public LimaTrial trial;
    public int numTrials;

    public void ButtonWrapper()
    {
        GenerateExperiment();
    }

     private  void GenerateExperiment()
    {
        CSVReader reader = new CSVReader();
        // conditionFile = PlayerPrefs.GetString("conditionFile");
        trials  = reader.ReadTrialCSV(conditionFile);   
        numTrials = trials.Count;
        Debug.Log("num trials"+numTrials);

        PlayerPrefs.SetInt("trialNum", 0);
        Task.SetActive(true);
        SessionScreen.SetActive(false);
    }

}
