using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LimaTask : MonoBehaviour
{
    public GameObject trialController;
    public GameObject SessionGenerator;
    public LimaTask limaTask;

    public GameObject endStateScreen;
    public GameObject escapeStateScreen;
    public GameObject freeStateScreen;
    public GameObject capturedStateScreen;
    public GameObject anxiousReport;
    public GameObject confidenceReport;
    public GameObject  startUI;

    public GameObject player;
    public PlayerManager playerManager;
    public GameObject predator;

    public GameObject arena;
    public GameObject map;
    public GameObject probabilityDisplay;

    public GameObject HeadsUpDisplay;
    public GameObject mainCamera;

    private LimaTrial trial;
    private TrialDataHandler dataHandler;
    public bool trialEndable;
    public bool shouldStopCoroutine = false;

    private bool acorns;
    private Vector3 home;
    private bool cookiesActive;
    private bool acornsActive;

    private int currentTransition;
    private float startTime;
    private float endTime;
    private Coroutine freeMovementCoroutine; // Reference to store the coroutine
    private GameState gameState;

    #region Enums and State Management

    public enum GameState
    {
        AcornPeriod,
        SpawningPeriod,
        ClickingPeriod,
        EffortPeriod,
        EndingPeriod,
        NextTrialPeriod
    }

    #endregion

    #region Running a Trial

    // Upon enabling this gameobject the first trial will run.
    public void OnEnable()
    {
        dataHandler = SessionGenerator.GetComponent<TrialDataHandler>();
        dataHandler.instantiateTrialDataHandlers();
    }

    public void StartNextTrial()
    {
        startUI.SetActive(false);
        dataHandler.ClearAllTrialDataHandlers();
        trialEndable = true;
        endTime = -1.0f;

        int trialNum = PlayerPrefs.GetInt("trialNum");
        trial = SessionGenerator.GetComponent<SessionGenerator>().trials[trialNum];
        startTime = Time.realtimeSinceStartup;
        dataHandler.recordMouseStartPosition();
        
        if(trial.type == 4)
        {
             gameState=GameState.AcornPeriod;
            mainCamera.GetComponent<ChangeCursor>().setMoveCursor();
        }
        else
        {
             gameState=GameState.SpawningPeriod;
            mainCamera.GetComponent<ChangeCursor>().setTargetCursor();
        }

        HandleGameState(gameState);
    }

    public void HandleGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.AcornPeriod:
                StartCoroutine(HandleAcornSpawnSequence(trial));
                break;
            case GameState.SpawningPeriod:
                StartCoroutine(HandleSpawnSequence(trial));
                break;
            case GameState.ClickingPeriod:
                BeginClickingPeriod();
                break;
            case GameState.EffortPeriod:
                dataHandler.stopRecordContinuousMouse("choiceperiod");

                if(currentTransition==5) // ANXIOUS
                {
                    anxiousReport.SetActive(true);
                }
                else if(currentTransition==6) //CONFIDENCE
                {
                    confidenceReport.SetActive(true);
                }
                else
                {
                    BeginEffortPeriod();
                }
                break;
            case GameState.EndingPeriod:
                EndTrial();
                break;
            case GameState.NextTrialPeriod:
                OnTrialEnd();
                break;
        }
    }

    IEnumerator HandleSpawnSequence(LimaTrial trial)
    {
        PrepareArena(trial);
        //Let's set the mouse here
        // mainCamera.GetComponent<ChangeCursor>().MoveCursorToCenterAndUnlock();
        yield return new WaitForSeconds(1.0f);
        ToggleRewards(trial);
        BeginClickingPeriod();
    }

    IEnumerator HandleAcornSpawnSequence(LimaTrial trial)
    {
        PrepareArena(trial);
        // mainCamera.GetComponent<ChangeCursor>().MoveCursorToCenterAndUnlock();
        yield return new WaitForSeconds(1.0f);
        mainCamera.GetComponent<ChangeCursor>().setMoveCursor();
        ToggleAcorns();
        EnableAcornPeriod();
    }

    private void PrepareArena(LimaTrial trial)
    {
        arena.SetActive(true);
        ToggleProbability(trial);
        map.SetActive(true);
        TogglePlayer();
    }

    public void BeginClickingPeriod()
    {
        dataHandler.startRecordContinuousMouse("choiceperiod");
        EnableClickingPeriod();
    }

    public void BeginEffortPeriod()
    {
        Debug.Log("Starting free movement sequence");
        mainCamera.GetComponent<ChangeCursor>().setMoveCursor();
        dataHandler.startRecordContinuousMouse("effortperiod");
        dataHandler.StartRecordingPlayerPosition();
        EnableEffortPhase();
    }

    public void EnableClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
    }

    public void EnableEffortPhase()
    {
        HeadsUpDisplay.SetActive(true);
        ToggleEffort();
        TogglePredator();
        SetPredator(trial);

        // Start free movement and store the reference
        if (freeMovementCoroutine == null)
        {
            shouldStopCoroutine = false;  // Reset the flag before starting the coroutine
            ToggleRewards(trial);
            freeMovementCoroutine = StartCoroutine(FreeMovement());
        }
    }

    IEnumerator FreeMovement()
    {
        Debug.Log("Free movement started");
        InvokeRepeating("UpdateTimer", 0f, 0.01f);

        float elapsedTime = 0f;
        while (elapsedTime < 10.0f)
        {
            if (shouldStopCoroutine) // Check if the coroutine should stop
            {
                Debug.Log("Coroutine stopped");
                yield break;  // Exit the coroutine immediately
            }

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Check if the trial is still endable after movement
        if (trialEndable && !shouldStopCoroutine)
        {
            HandleGameState(GameState.EndingPeriod);
        }

        freeMovementCoroutine = null;  // Reset reference when finished
    }

    #endregion

    #region Ending a Trial

    public void EndTrial()
    {
         // Set flag to stop the coroutine at the next check
        shouldStopCoroutine = true;

        // Ensure that the free movement coroutine is stopped
        if (freeMovementCoroutine != null)
        {
            StopCoroutine(freeMovementCoroutine);
            freeMovementCoroutine = null;
        }

        trialEndable = false;


        CancelInvoke("UpdateTimer");
        endTime = Time.realtimeSinceStartup;
        StartCoroutine(TrialEndRoutine(trial));
    }

        public void OnTrialEnd()
    {
        // Retrieve and update trial number
        int trialNum = PlayerPrefs.GetInt("trialNum");
        LogTrialData(trial);

        Debug.Log("Pushing trial data");
        SessionGenerator sessionGen = SessionGenerator.GetComponent<SessionGenerator>();
        sessionGen.pushTrialData(trial, trialNum);

        // Get the current transition state for this trial
        currentTransition = sessionGen.trials[trialNum].transitionState;

        // Increment and save the updated trial number
        trialNum++;
        PlayerPrefs.SetInt("trialNum", trialNum);
        PlayerPrefs.Save();


        // Check if there are more trials left
        if (trialNum <= sessionGen.numTrials)
        { 

            // Handle transitions based on the current transition state
            if (currentTransition == 1)
            {
                SceneManager.LoadScene("EndGame");
            }
            else if (currentTransition == 2 || currentTransition == 3 || currentTransition == 4)
            { 
                int nextTrialType = sessionGen.trials[trialNum].type;
                PlayerPrefs.SetInt("nextType", nextTrialType);
                PlayerPrefs.SetInt("transitionState", currentTransition);
                SceneManager.LoadScene("SurveyScene"); // Placeholder for the questionnaire scene
            }
            else
            {
                startUI.SetActive(true);
            }
        }
    }


    IEnumerator TrialEndRoutine(LimaTrial trial)
    {
        ResetTrialSettings();
        yield return new WaitForSeconds(2.0f);
        ToggleEndStateScreen(playerManager.playerState);
        ResetArena(trial);
        HandleGameState(GameState.NextTrialPeriod);
    }

    private void ResetTrialSettings()
    {
        player.GetComponent<PlayerMovement>().effortPeriod = false;
        predator.GetComponent<PredatorControls>().circaStrike = false;
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
        HeadsUpDisplay.SetActive(false);
        ToggleEndStateScreen(playerManager.playerState);
        TogglePlayer();
        TogglePredator();
    }

    private void ResetArena(LimaTrial trial)
    {

        if (trial.type == 4)
        {
            ToggleAcorns();
        }
       
        ToggleProbability(trial);
        ResetTimer();
    }

    #endregion

    #region Wrappers and Helpers

    public void registerSubjectiveReport(int clickResponse)
    {
        SubjectiveReport report = new SubjectiveReport();
        report.trialNumber = PlayerPrefs.GetInt("trialNum");
        report.attackingProb = trial.attackingProb;
        report.trialCookie = playerManager.cookieChoice;
        report.response =clickResponse;  //how do I know which button they pressed? ;
        report.questionType = currentTransition; // 5 ANXIOUS / 6 CONFIDENCE

        string reportName = "";

        if(currentTransition==5)
        {
            anxiousReport.SetActive(false);
            reportName = "Anxiety_"+report.trialNumber;
        }
        else if(currentTransition==6)
        {
            confidenceReport.SetActive(false);
            reportName = "Confidence_"+report.trialNumber;
        }
        
        SessionGenerator.GetComponent<SessionGenerator>().pushSubjectiveData(report, reportName);
        BeginEffortPeriod();
    }



    public void EnableAcornPeriod()
    {
        HeadsUpDisplay.SetActive(true);
        dataHandler.StartRecordingPlayerPosition();
        player.GetComponent<PlayerMovement>().enableAcorns();
        SetPredator(trial);
        // Start free movement and store the reference
        if (freeMovementCoroutine == null)
        {
            shouldStopCoroutine = false;  // Reset the flag before starting the coroutine
            freeMovementCoroutine = StartCoroutine(FreeMovement());
        }
    }

    void ToggleProbability(LimaTrial trial)
    {
        bool activeState = probabilityDisplay.activeSelf;
        trialController.GetComponent<TrialController>().setProbability(trial.attackingProb);
        probabilityDisplay.SetActive(!activeState);
    }

    void TogglePlayer()
    {
        player.SetActive(!player.activeSelf);
        if (player.activeSelf)
        {
            playerManager.playerState = "free";
            home = player.transform.position;
        }
        else
        {
            player.GetComponent<PlayerMovement>().effortPeriod = false;
            player.GetComponent<PlayerMovement>().clickingPeriod = false;
            HandleCarrying();
            player.transform.position = home;
        }
    }

    private void HandleCarrying()
    {
        if (playerManager.carrying)
        {
            playerManager.carrying = false;
            foreach (Transform child in player.transform)
            {
                if (child.CompareTag("Cookie")) Destroy(child.gameObject);
            }
        }
    }

    void TogglePredator()
    {
        if (!predator.activeSelf)
        {
            Debug.Log("Activating predator");
            predator.SetActive(true);
        }
        else
        {
            Debug.Log("Deactivating predator");
            predator.SetActive(false);
        }
    }

    void ToggleEffort() 
    {
        float latency = PlayerPrefs.GetFloat("PressLatency"); 
        player.GetComponent<PlayerMovement>().enableEffort(latency);
    }

    void ToggleEndStateScreen(string state)
    {
        bool isCurrentlyActive = endStateScreen.activeSelf;
        endStateScreen.SetActive(!isCurrentlyActive);

        // If the screen was already active, we exit early.
        if (isCurrentlyActive) return;

        // First, ensure all state screens are deactivated before showing the correct one
        DeactivateAllStateScreens();

        // Activate the appropriate state screen based on the provided state
        switch (state)
        {
            case "escaped":
                escapeStateScreen.SetActive(true);
                break;
            case "captured":
                capturedStateScreen.SetActive(true);
                break;
            case "free":
                freeStateScreen.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unknown state: {state}");
                break;
        }
    }

    void DeactivateAllStateScreens()
    {
        // Ensure all state screens are deactivated to avoid overlapping
        escapeStateScreen.SetActive(false);
        capturedStateScreen.SetActive(false);
        freeStateScreen.SetActive(false);
    }

    void ToggleRewards(LimaTrial trial)
    {
        if (!cookiesActive)
        {
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie1PosX, trial.cookie1PosY, trial.cookie1Weight, trial.cookie1RewardValue);
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie2PosX, trial.cookie2PosY, trial.cookie2Weight, trial.cookie2RewardValue);
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie3PosX, trial.cookie3PosY, trial.cookie3Weight, trial.cookie3RewardValue);
            cookiesActive = true;
        }
        else
        {
            trialController.GetComponent<TrialController>().despawnRewards();
            cookiesActive = false;
        }
    }

    void ToggleAcorns()
    {
        if(!acorns)
        {
            Debug.Log("spawning acorns");
            trialController.GetComponent<TrialController>().spawnAcorns(5);
            acorns = true;
        }
        else
        {
            Debug.Log("destroying acorns");
            trialController.GetComponent<TrialController>().despawnAcorns();
            acorns = false;
        }
    }


    public void SetPredator(LimaTrial trial) {predator.GetComponent<PredatorControls>().setAttack(trial.attackingProb);}

    void LogTrialData(LimaTrial trial)
    {
        trial.playerPosition = dataHandler.playerPosition;
        trial.predatorPosition = dataHandler.predatorPosition;
        trial.mouseTrackChoicePeriod = dataHandler.mouseTrackChoicePeriod;
        trial.mouseTrackEffortPeriod = dataHandler.mouseTrackEffortPeriod;
        trial.effortRate = dataHandler.effortRate;
        trial.trialStartTime = startTime;
        trial.trialEndTime = endTime;
        trial.trialReward = playerManager.earnedReward;
        trial.trialCookie = playerManager.cookieChoice;
        trial.trialEndState = playerManager.playerState;
        trial.acornsSpawned = trialController.GetComponent<TrialController>().acornLoggedPositions;
        trial.acornsCollected = playerManager.acornsCollected;
    }



    #endregion

    #region Continuous Methods

    void UpdateTimer()
    {
        UIController uiController = HeadsUpDisplay.GetComponent<UIController>();
        if (uiController != null)
        {
            uiController.DecreaseTime(0.01f / 10f);
        }
    }

    void ResetTimer()
    {
        UIController uiController = HeadsUpDisplay.GetComponent<UIController>();
        if (uiController != null)
        {
            uiController.SetTime(1f);
        }
    }

    #endregion
}
