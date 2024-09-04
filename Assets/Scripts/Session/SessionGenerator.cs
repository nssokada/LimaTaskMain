using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class SessionGenerator : MonoBehaviour
{
    
    public GameObject Task;
    public GameObject SessionScreen;


    CSVReader reader;
    public string conditionFile;
    public string username;

    public List<LimaTrial> trials;
    public LimaTrial trial;
    public int numTrials;

    private string dataBasePath = "https://limatask-a554a-default-rtdb.firebaseio.com/";
    public static string persistentDataPath; //datapath that will be used throughout the game. A concatenation of the username and DB path
    private string currentState;



    public async void MainGameButton()
    {
        username = PlayerPrefs.GetString("userID");

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if(!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "MainGame");
        PlayerPrefs.SetString("GameState", "MainGame");

        //IF OLD USER resume game
        if(PlayerPrefs.HasKey("ConditionFile"))
        {
            conditionFile = PlayerPrefs.GetString("ConditionFile");
            ReLoadExperiment(conditionFile);
        }
        //ELSE Start new game
        else
        {
            Debug.Log("attempting to grab conditionfile");
            //Determine Condition File:
            await pullCondition();
            GenerateExperiment(conditionFile);
        }
    }

    public void TutorialButton()
    {
        username = PlayerPrefs.GetString("userID");
        
        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if(!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "Tutorial");
        PlayerPrefs.SetString("GameState", "Tutorial");


        GenerateTutorial();
    }

    public void SetGameState(string transferState)
    {
        switch (transferState)
        {
            case "Tutorial":
                PlayerPrefs.SetString("GameState",transferState);
                break;

            case "CookieGame":
                PlayerPrefs.SetString("GameState",transferState);
                break;
            case "AcornGame":
                PlayerPrefs.SetString("GameState",transferState);
                break;
            case "Survey":
                PlayerPrefs.SetString("GameState",transferState);
                break;
            default:
                Debug.LogError("Unhandled game state: " + transferState);
                break;
        }
    }

     private  void GenerateExperiment(string conditionFile)
    {
        CSVReader reader = new CSVReader();
        trials  = reader.ReadTrialCSV(conditionFile);   
        numTrials = trials.Count;
        Debug.Log("num trials"+numTrials);

        createExperimentInfo(username, conditionFile);
        PlayerPrefs.SetInt("trialNum", 0);
        Task.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private void ReLoadExperiment(string conditionFile)
    {
        CSVReader reader = new CSVReader();
        trials  = reader.ReadTrialCSV(conditionFile);   
        numTrials = trials.Count;
        Debug.Log("num trials"+numTrials);

        resetExperimentInfo(username, conditionFile);
        Task.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private  void GenerateTutorial()
    {
        createExperimentInfo(username+"_Tutorial", conditionFile);
        SessionScreen.SetActive(false);
        Task.SetActive(true); //in the tutorial scene this will be the main video player object
    }





    private string CreatePersistentPath(string username)
    {
        return persistentDataPath = dataBasePath+username;
    }

    public void pushTrialData(LimaTrial trial, int trialNum)
    {
        string trial_num = trialNum.ToString("D2");
        writeToFirebase(trial_num, "trial_data", trial);
    }

    private void createExperimentInfo(string attributeName, string conditionFile)
    {
        experimentInfo exp = new experimentInfo();
        exp.condition = conditionFile;
        exp.participantID = username;
        exp.experimentDate = "" + System.DateTime.Now;
        exp.datapath = persistentDataPath;
        writeToFirebase(attributeName, exp);
    }

    private void resetExperimentInfo(string username, string conditionFile)
    {
        experimentInfo exp = new experimentInfo();
        exp.condition = conditionFile;
        exp.participantID = username;
        exp.experimentDate = "" + System.DateTime.Now;
        exp.datapath = persistentDataPath;
        writeToFirebase(username+"_reset", exp);
    }


        private async Task pullCondition()
    {
        var tcs = new TaskCompletionSource<bool>();

        RestClient.Get(dataBasePath + "ActiveCondition.json").Then(response =>
        {
            conditionFile = response.Text.Trim('"'); // Remove the leading and trailing quotation marks
            PlayerPrefs.SetString("ConditionFile", conditionFile);
            tcs.SetResult(true); // Signal that the task is complete
        }).Catch(err =>
        {
            Debug.LogError("Error: " + err.Message);
            tcs.SetException(err); // Signal that the task failed
        });

        // Await the task to ensure the method doesn't complete until the asynchronous operation is done
        await tcs.Task;
    }

    //Push Trial to DB
    //writeToDB
    private void writeToFirebase(string trial_num, string attributeName, LimaTrial attribute)
    {
        RestClient.Put(persistentDataPath + "/TrialData/trial_" + trial_num + "/" + attributeName + ".json", attribute);
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
