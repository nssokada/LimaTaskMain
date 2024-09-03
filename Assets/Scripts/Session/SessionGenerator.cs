using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;

public class SessionGenerator : MonoBehaviour
{
    
    public GameObject Task;
    public GameObject SessionScreen;


    CSVReader reader;
    public string conditionFile;


    public List<LimaTrial> trials;
    public LimaTrial trial;
    public int numTrials;

    private string dataBasePath = "https://limatask-a554a-default-rtdb.firebaseio.com/";
    public static string persistentDataPath; //datapath that will be used throughout the game. A concatenation of the username and DB path
    private string currentState;


    // private void OnEnable() {
    //     // GenerateExperiment();
    //     if (PlayerPrefs.HasKey("GameState"))
    //     {
    //         // The key exists, so we can safely retrieve the string value
    //          currentState = PlayerPrefs.GetString("GameState");
    //         Debug.Log("Current State: " + currentState);
    //     }
    //     else
    //     {
    //          currentState = "Login";
    //         PlayerPrefs.SetString("GameState", currentState);
    //     }


    //     switch(currentState)
    //     {
    //         case "Login":
    //             Debug.Log("waiting for user");
    //             break;
    //         case "Tutorial":
    //             break;
    //         case "CookieGame":
    //             setUpCookieGame();
    //         case "AcornGame":
    //             setUpAcornGame();
    //         default:
    //             Debug.LogError("Unhandled game state: " + currentState);
    //             break;
    //     }
    // }

    public void loginButton(string username)
    {
        //Create Persistent Datapath
        CreatePersistentPath(username);
        PlayerPrefs.SetString("DataPath", persistentDataPath);

        //Determine Condition File:
        conditionFile = pullCondition();
        
        //Set Experiment Info
        createExperimentInfo(username, conditionFile);

        //Set Game State:
        if(PlayerPrefs.HasKey("ReloadCheckPoint"))
        {
            string checkPoint = PlayerPrefs.GetString("ReloadCheckPoint");
            SetGameState(checkPoint);
        } 
        else SetGameState("Tutorial");
       
    }

    public void SetGameState(string transferState)
    {
        switch (transferState)
        {
            case "Tutorial":
                PlayerPrefs.SetString("GameState",transferState);
                SceneManager.LoadScene("Tutorial");
                break;

            case "CookieGame":
                PlayerPrefs.SetString("GameState",transferState);
                SceneManager.LoadScene("MainGame");
                break;
            case "AcornGame":
                PlayerPrefs.SetString("GameState",transferState);
                SceneManager.LoadScene("MainGame");
                break;
            case "Survey":
                PlayerPrefs.SetString("GameState",transferState);
                SceneManager.LoadScene("SurveyScene");
                break;
            default:
                Debug.LogError("Unhandled game state: " + transferState);
                break;
        }
    }

     private  void GenerateExperiment()
    {
        CSVReader reader = new CSVReader();
        trials  = reader.ReadTrialCSV(conditionFile);   
        numTrials = trials.Count;
        Debug.Log("num trials"+numTrials);

        PlayerPrefs.SetInt("trialNum", 0);
        Task.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private string CreatePersistentPath(string username)
    {
        return persistentDataPath = dataBasePath+username;
    }

    public void pushTrialData(LimaTrial trial)
    {
        int trialNum = PlayerPrefs.GetInt("trial_num");
        string trial_num = trialNum.ToString("D2");
        writeToFirebase(trial_num, "trial_data", trial);
    }

    private void createExperimentInfo(string username, string conditionFile)
    {
        experimentInfo exp = new experimentInfo();
        exp.condition = conditionFile;
        exp.participantID = username;
        exp.experimentDate = "" + System.DateTime.Now;
        exp.datapath = persistentDataPath;
        writeToFirebase(username, exp);
    }


    public string pullCondition()
    {
        RestClient.Get(dataBasePath + "ActiveCondition.json").Then(response =>
        {
            string conditionFile = response.Text.Trim('"'); // Remove the leading and trailing quotation marks
        }).Catch(err => Debug.LogError("Error: " + err.Message));

        return conditionFile;
    }


    //Push Trial to DB
    //writeToDB
    private void writeToFirebase(string trial_num, string attributeName, LimaTrial attribute)
    {
        RestClient.Put(persistentDataPath + "/trial_" + trial_num + "/" + attributeName + ".json", attribute);
    }
    //Push ExpInfo to DB
    //writeToDB
     private void writeToFirebase(string attributeName, experimentInfo attribute)
    {
        RestClient.Put(persistentDataPath + "/" + attributeName + ".json", attribute);
    }

    //Completing questionnaires?

    //readFromDB

}
