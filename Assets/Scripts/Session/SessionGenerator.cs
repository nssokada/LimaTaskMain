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
    public GameObject startUI;
    CSVReader reader;
    public string conditionFile;
    public string username;
    public float playerPressLatency;

    public List<LimaTrial> trials;
    public LimaTrial trial;
    public int numTrials;

    private string dataBasePath = "https://limatask-a554a-default-rtdb.firebaseio.com/";
    public static string persistentDataPath; //datapath that will be used throughout the game. A concatenation of the username and DB path
    private string currentState;

    public void MainGameButton()
    {
        username = PlayerPrefs.GetString("userID");
        PlayerPrefs.SetFloat("StartTime", Time.realtimeSinceStartup);

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if (!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
            Debug.Log("DataPath created: " + persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");
        Debug.Log("Persistent DataPath set: " + persistentDataPath);

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "MainGame");
        PlayerPrefs.SetString("GameState", "MainGame");

        //IF OLD USER resume game
        if (PlayerPrefs.HasKey("ConditionFile"))
        {
            conditionFile = PlayerPrefs.GetString("ConditionFile");
            Debug.Log("Resuming game with condition file: " + conditionFile);
            ReLoadExperiment(conditionFile);
        }
        //ELSE Start new game
        else
        {
            Debug.Log("Attempting to grab condition file...");
            //Determine Condition File:
            conditionFile = "condition_3";
            PlayerPrefs.SetString("ConditionFile", conditionFile);
            GenerateExperiment(conditionFile);
        }
    }



    public void RecordPressLatency(float playerPressLatency, float count)
    {
        pushLatencyToFirebase(playerPressLatency);
        pushCountToFirebase(count);
    }



    public async Task<float> GetPressLatencyFromDB()
    {
        // Call the PullPressLatency method to retrieve the latency from Firebase
        float latency = await PullPressLatency();

        // Return the pulled latency
        return latency;
    }

    public void AcornButton()
    {
        username = PlayerPrefs.GetString("userID");

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if (!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
            Debug.Log("DataPath created: " + persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");
        Debug.Log("Persistent DataPath set: " + persistentDataPath);

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "AcornGame");
        PlayerPrefs.SetString("GameState", "AcornGame");


        Debug.Log("Starting tutorial...");
        GenerateAcornTutorial();

    }

    public void TutorialButton_1()
    {
        username = PlayerPrefs.GetString("userID");
        PlayerPrefs.SetFloat("StartTime", Time.realtimeSinceStartup);

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if (!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
            Debug.Log("DataPath created: " + persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");
        Debug.Log("Persistent DataPath set: " + persistentDataPath);
        persistentDataPath = persistentDataPath + "/Tutorial_1";

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "Tutorial");
        PlayerPrefs.SetString("GameState", "Tutorial");

        Debug.Log("Starting tutorial...");
        GenerateTutorial();
    }
        public void TutorialButton_2()
    {
        username = PlayerPrefs.GetString("userID");
        PlayerPrefs.SetFloat("StartTime", Time.realtimeSinceStartup);

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if (!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
            Debug.Log("DataPath created: " + persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");
        Debug.Log("Persistent DataPath set: " + persistentDataPath);
        persistentDataPath = persistentDataPath + "/Tutorial_2";

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "Tutorial");
        PlayerPrefs.SetString("GameState", "Tutorial");

        Debug.Log("Starting tutorial...");
        GenerateTutorial();
    }



    public void SurveyButton()
    {
        username = PlayerPrefs.GetString("userID");

        //IF THERE IS NOT ALREADY A DATAPATH LET'S CREATE ONE
        if (!PlayerPrefs.HasKey("DataPath"))
        {
            persistentDataPath = CreatePersistentPath(username);
            PlayerPrefs.SetString("DataPath", persistentDataPath);
            Debug.Log("DataPath created: " + persistentDataPath);
        }

        //Set the datapath
        persistentDataPath = PlayerPrefs.GetString("DataPath");
        Debug.Log("Persistent DataPath set: " + persistentDataPath);

        //SET KEYS
        PlayerPrefs.SetString("CheckPoint", "Survey");
        PlayerPrefs.SetString("GameState", "Survey");

        Debug.Log("Starting Questionnaires...");
    }



    public void SetGameState(string transferState)
    {
        Debug.Log("Setting game state to: " + transferState);
        switch (transferState)
        {
            case "Tutorial":
                PlayerPrefs.SetString("GameState", transferState);
                break;
            case "CookieGame":
                PlayerPrefs.SetString("GameState", transferState);
                break;
            case "AcornGame":
                PlayerPrefs.SetString("GameState", transferState);
                break;
            case "Survey":
                PlayerPrefs.SetString("GameState", transferState);
                break;
            default:
                Debug.LogError("Unhandled game state: " + transferState);
                break;
        }
    }

    private void GenerateExperiment(string conditionFile)
    {
        CSVReader reader = new CSVReader();
        trials = reader.ReadTrialCSV(conditionFile);
        numTrials = trials.Count;
        Debug.Log("Generated experiment with condition file: " + conditionFile);
        Debug.Log("Number of trials: " + numTrials);

        createExperimentInfo(username, conditionFile);
        PlayerPrefs.SetInt("trialNum", 0);
        PlayerPrefs.SetFloat("TotalScore", 0.0f);
        Task.SetActive(true);
        startUI.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private void ReLoadExperiment(string conditionFile)
    {
        CSVReader reader = new CSVReader();
        trials = reader.ReadTrialCSV(conditionFile);
        numTrials = trials.Count;
        Debug.Log("Reloaded experiment with condition file: " + conditionFile);
        Debug.Log("Number of trials: " + numTrials);

        resetExperimentInfo(username, conditionFile);
        Task.SetActive(true);
        startUI.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private void GenerateTutorial()
    {
        // Debug.Log("Generating tutorial...");
        // createExperimentInfo(username+"_Tutorial", conditionFile);
        // SessionScreen.SetActive(false);
        // Task.GetComponent<TutorialController>().StartMainTutorial(); 
        CSVReader reader = new CSVReader();
        trials = reader.ReadTrialCSV(conditionFile);
        numTrials = trials.Count;
        Debug.Log("Generated experiment with condition file: " + conditionFile);
        Debug.Log("Number of trials: " + numTrials);

        Debug.Log("Generating tutorial...");
        createExperimentInfo(username + "_Tutorial", conditionFile);
        PlayerPrefs.SetInt("trialNum", 0);
        PlayerPrefs.SetFloat("TotalScore", 0.0f);
        Task.SetActive(true);
        startUI.SetActive(true);
        SessionScreen.SetActive(false);
    }

    private void GenerateAcornTutorial()
    {
        Debug.Log("Generating tutorial...");
        createExperimentInfo(username + "_AcornTutorial", conditionFile);
        SessionScreen.SetActive(false);
        Task.GetComponent<TutorialController>().StartAcornTutorial();
    }

    private string CreatePersistentPath(string username)
    {
        string path = dataBasePath + username;
        Debug.Log("Created persistent path: " + path);
        return path;
    }

    public void pushTrialData(LimaTrial trial, int trialNum)
    {
        string trial_num = trialNum.ToString("0000");

        writeToFirebase(trial_num, "trial_data", trial);
    }

    public void pushSurveyData(QuestionResponseList surveyResponse, string surveyName)
    {

        writeToFirebase(surveyName, surveyResponse);

    }

    public void pushResetExperimentInfo()
    {
        username = PlayerPrefs.GetString("userID");
        conditionFile = PlayerPrefs.GetString("ConditionFile");
        resetExperimentInfo(username, conditionFile);
    }


    public void pushSubjectiveData(SubjectiveReport report, string reportName)
    {

        writeToFirebase(reportName, report);
    }



    private void createExperimentInfo(string attributeName, string conditionFile)
    {
        experimentInfo exp = new experimentInfo();
        exp.condition = conditionFile;
        exp.participantID = username;
        exp.experimentDate = "" + System.DateTime.Now;
        exp.datapath = persistentDataPath;
        exp.effortPressLatency = PlayerPrefs.GetFloat("PressLatency", -1f);
        exp.effortPressCount = PlayerPrefs.GetFloat("PressCount", -1f);
        Debug.Log("Creating experiment info for: " + attributeName);
        writeToFirebase(attributeName, exp);
    }

    private void resetExperimentInfo(string username, string conditionFile)
    {
        experimentInfo exp = new experimentInfo();
        exp.condition = conditionFile;
        exp.participantID = username;
        exp.experimentDate = "" + System.DateTime.Now;
        exp.datapath = persistentDataPath;
        exp.effortPressLatency = PlayerPrefs.GetFloat("PressLatency", -1f);
        exp.effortPressCount = PlayerPrefs.GetFloat("PressCount", -1f);
        Debug.Log("Resetting experiment info for: " + username);
        int resetTime = (int)(Time.realtimeSinceStartup * 1000) % 1000;
        writeToFirebase(username + "_reset_time_"+resetTime, exp);
    }

    // private async Task pullCondition()
    // {
    //     var tcs = new TaskCompletionSource<bool>();
    //     Debug.Log("Pulling condition from Firebase...");

    //     RestClient.Get(dataBasePath + "ActiveCondition.json").Then(response =>
    //     {
    //         conditionFile = response.Text.Trim('"'); // Remove the leading and trailing quotation marks
    //         PlayerPrefs.SetString("ConditionFile", conditionFile);
    //         Debug.Log("Condition file received: " + conditionFile);
    //         tcs.SetResult(true); // Signal that the task is complete
    //     }).Catch(err =>
    //     {
    //         Debug.LogError("Error pulling condition: " + err.Message);
    //         tcs.SetException(err); // Signal that the task failed
    //     });

    //     // Await the task to ensure the method doesn't complete until the asynchronous operation is done
    //     await tcs.Task;
    // }

    // Write trial data to Firebase
    private void writeToFirebase(string trial_num, string attributeName, LimaTrial attribute)
    {
        string path = persistentDataPath + "/TrialData/trial_" + trial_num + "/" + attributeName + ".json";
        Debug.Log($"Writing trial {trial_num} data to Firebase at {path}");

        RestClient.Put(path, attribute).Then(response =>
        {
            Debug.Log($"Successfully wrote trial {trial_num} data to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError($"Failed to write trial {trial_num} data to Firebase. Error: {err.Message}");
        });
    }

    // Write experiment info to Firebase
    private void writeToFirebase(string attributeName, experimentInfo attribute)
    {
        string path = persistentDataPath + "/" + attributeName + ".json";
        Debug.Log($"Writing experiment info for {attributeName} to Firebase at {path}");

        RestClient.Put(path, attribute).Then(response =>
        {
            Debug.Log($"Successfully wrote experiment info for {attributeName} to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError($"Failed to write experiment info for {attributeName} to Firebase. Error: {err.Message}");
        });
    }

    // Write survey info to Firebase
    private void writeToFirebase(string attributeName, QuestionResponseList attribute)
    {
        string path = persistentDataPath + "/Surveys/" + attributeName + ".json";
        Debug.Log($"Writing experiment info for {attributeName} to Firebase at {path}");

        RestClient.Put(path, attribute).Then(response =>
        {
            Debug.Log($"Successfully wrote experiment info for {attributeName} to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError($"Failed to write experiment info for {attributeName} to Firebase. Error: {err.Message}");
        });
    }

    // Write subjective report to Firebase
    private void writeToFirebase(string attributeName, SubjectiveReport attribute)
    {
        string path = persistentDataPath + "/SubjectiveReports/" + attributeName + ".json";
        Debug.Log($"Writing experiment info for {attributeName} to Firebase at {path}");

        RestClient.Put(path, attribute).Then(response =>
        {
            Debug.Log($"Successfully wrote experiment info for {attributeName} to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError($"Failed to write experiment info for {attributeName} to Firebase. Error: {err.Message}");
        });
    }


    private void pushLatencyToFirebase(float latency)
    {
        int resetTime = (int)(Time.realtimeSinceStartup * 1000) % 1000;
        string path = persistentDataPath + "/PlayerData/"+resetTime+"pressLatency.json";
        Debug.Log($"Writing press latency data to Firebase at {path}");

        RestClient.Put(path, latency).Then(response =>
        {
            Debug.Log("Successfully wrote press latency data to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError("Failed to write press latency data to Firebase. Error: " + err.Message);
        });
    }

    private void pushCountToFirebase(float count)
    {
        int resetTime = (int)(Time.realtimeSinceStartup * 1000) % 1000;
        string path = persistentDataPath + "/PlayerData/"+resetTime+"pressCount.json";
        Debug.Log($"Writing press count data to Firebase at {path}");

        RestClient.Put(path, count).Then(response =>
        {
            Debug.Log("Successfully wrote press latency data to Firebase.");
        }).Catch(err =>
        {
            Debug.LogError("Failed to write press latency data to Firebase. Error: " + err.Message);
        });
    }

    private async Task<float> PullPressLatency()
    {
        var tcs = new TaskCompletionSource<float>();
        string path = persistentDataPath + "/PlayerData/pressLatency.json";

        RestClient.Get(path).Then(response =>
        {
            float latency = float.Parse(response.Text);
            playerPressLatency = latency;
            Debug.Log("Pulled player press latency: " + latency);
            tcs.SetResult(latency);
        }).Catch(err =>
        {
            Debug.LogError("Error pulling press latency: " + err.Message);
            tcs.SetException(err);
        });


        return await tcs.Task;
    }


}
