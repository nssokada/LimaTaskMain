using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimaTask : MonoBehaviour
{
    public GameObject trialController;
    public GameObject SessionGenerator;

    public GameObject endStateScreen;
    public GameObject escapeStateScreen;
    public GameObject freeStateScreen;
    public GameObject capturedStateScreen;

    public GameObject player;
    public PlayerManager playerManager;
    public GameObject predator;

    public GameObject arena;
    public GameObject map;
    public GameObject wind;
    public GameObject mainCamera;
   
    public GameObject probabilityDisplay;

    private LimaTrial trial;
    private TrialDataHandler dataHandler;

    bool trial_timeUp;
    public bool trialEndable;
    Vector3 home;
    bool cookies;
    float movementStartTime; //likely will change this to trial start time
    public GameObject HeadsUpDisplay;
    private float startTime;
    private float endTime;

  

#region  Running a Trial

    //Upon enabling this gameobject the first trial will run.
    public void OnEnable()
    {
       dataHandler = SessionGenerator.GetComponent<TrialDataHandler>();
       dataHandler.instantiateTrialDataHandlers();
       startNextTrial();

    }

    public void startNextTrial()
    {
        //clear any data from last trial
        dataHandler.ClearAllTrialDataHandlers();
        endTime = float.NaN;

        trialEndable = true;
        int trialNum = PlayerPrefs.GetInt("trialNum");
        trial  = SessionGenerator.GetComponent<SessionGenerator>().trials[trialNum];
        startTime = Time.realtimeSinceStartup;
        gameStateController("spawningPeriod");
    }

    public void gameStateController(string gameState)
    {   
        switch (gameState)
        {
            case "spawningPeriod":
                StartCoroutine(LimaSpawnSequence(trial));
                break;
            case "clickingPeriod":
                mainCamera.GetComponent<ChangeCursor>().MoveCursorToCenterAndUnlock();
                dataHandler.recordMouseStartPosition(); //first mouse position centered on screen
                dataHandler.startRecordContinuousMouse("choiceperiod");//enable mouse tracking during clicking period
                EnableClickingPeriod();
                break;
            case "effortPeriod":
                mainCamera.GetComponent<ChangeCursor>().setMoveCursor();
                dataHandler.stopRecordContinuousMouse("choiceperiod"); //cancel the clicking period mouse tracking
                dataHandler.startRecordContinuousMouse("effortperiod"); //enable the effort period mouse tracking
                dataHandler.StartRecordingPlayerPosition(); //player movement recorded here. Pred movement intiated in predator class
                EnableEffortPhase();
                break;
            case "endingPeriod":
                dataHandler.StopRecordingPlayerPosition();
                dataHandler.StopRecordingPredatorPosition();
                dataHandler.stopRecordContinuousMouse("effortperiod"); //cancel the effort period mouse tracking
                EndTrial();
                break;
            case "nextTrialPeriod":
                OnTrialEnd();
                break;
        }
    }

    IEnumerator LimaSpawnSequence(LimaTrial trial)
    {
        arena.SetActive(true);
        
        //Set Probabilty -> sets probability material shows probability display for 1 second 
        toggleProbability(trial);
        map.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        //Set Player -> sets player home status, enables player
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        //Set Rewards -> Spawns 2 cookies based on trial information
        toggleRewards(trial);     
        yield return new WaitForSeconds(1.0f);
        //check this out next
        gameStateController("clickingPeriod");
        Debug.Log("End Lima Sequence");   
    }


    //This period is ended by PlayerManager when player picks up the cookie
    public void EnableClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
    }

     public void EnableEffortPhase()
    {

        Debug.Log("Starting freemovement Sequence");   

        HeadsUpDisplay.SetActive(true);
        toggleEffort();
        // toggleWind();
         //Sets Predator probability and attack 
        togglePredator();
        setPredator(trial);  
        StartCoroutine("freeMovement");
    }

    IEnumerator freeMovement()
    { 
        InvokeRepeating("UpdateTimer", 0f, 0.01f);
        yield return new WaitForSeconds(10.0f);
        if(trialEndable)    gameStateController("endingPeriod");

    }

 


#endregion

#region  Ending a Trial

    public void EndTrial()
    {
        trialEndable = false;
        StopCoroutine("freeMovement");
        CancelInvoke("UpdateTimer");
        endTime = Time.realtimeSinceStartup;
        StartCoroutine(TrialEndRoutine(trial));
            
    }

    public void OnTrialEnd()
    {
        int trialNum = PlayerPrefs.GetInt("trialNum");
        //Transfer trial data
        logTrialData(trial);

        SessionGenerator.GetComponent<SessionGenerator>().pushTrialData(trial, trialNum);
        
        trialNum++;
        PlayerPrefs.SetInt("trialNum", trialNum);

        if(trialNum < SessionGenerator.GetComponent<SessionGenerator>().numTrials)
        {
            Debug.Log("trialNum"+trialNum);
            Debug.Log("evaluating next");
            startNextTrial();
        }
        else
        {
            Application.Quit();
            Debug.Log("end reached");
        }        
    }

    IEnumerator TrialEndRoutine(LimaTrial trial)
    {
        player.GetComponent<PlayerMovement>().effortPeriod = false;
        predator.GetComponent<PredatorControls>().circaStrike = false;

        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
        HeadsUpDisplay.SetActive(false);
        toggleEndStateScreen();
        togglePlayer();
        togglePredator();
        yield return new WaitForSeconds(2.0f);
        arena.SetActive(false);
        map.SetActive(false);
        toggleRewards(trial);
        toggleProbability(trial);
        toggleEndStateScreen();
        resetTimer();
        gameStateController("nextTrialPeriod");
    }
#endregion

#region  Wrappers and Helpers
   
 void toggleProbability(LimaTrial trial)
    {
        if(!probabilityDisplay.activeSelf)
        {
            Debug.Log("changingMaterial color");
            trialController.GetComponent<TrialController>().setProbability(trial.attackingProb);
            probabilityDisplay.SetActive(true);

        }

        else
        {
            probabilityDisplay.SetActive(false);
        }
    }

    void togglePlayer()
    {
        if(!player.activeSelf)
        {
            playerManager.playerState = "free";
            home = player.transform.position;
            player.SetActive(true);
        }

        else
        {
           player.GetComponent<PlayerMovement>().effortPeriod = false;
           player.GetComponent<PlayerMovement>().clickingPeriod = false;
           if(playerManager.carrying)
           {
                playerManager.carrying = false;
                  foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                    }
           }
           player.transform.position = home;

           player.SetActive(false);
        }
    }

    void togglePredator()
    {
        if(!predator.activeSelf)
        {
            predator.SetActive(true);
        }

        else
        {
           predator.SetActive(false);
        }
    }

    void toggleEffort()
    {
         player.GetComponent<PlayerMovement>().enableEffort();
    }

    void toggleEndStateScreen()
    {
        if(!endStateScreen.activeSelf)
        {
            endStateScreen.SetActive(true);
            Debug.Log(playerManager.playerState);
            if (playerManager.playerState.Equals("escaped"))
            {
                escapeStateScreen.SetActive(true);
            }
            else if (playerManager.playerState.Equals("captured"))
            {
                capturedStateScreen.SetActive(true);
            }
            else 
            {
                freeStateScreen.SetActive(true);
            }
        }
        else
        {
            endStateScreen.SetActive(false);
            escapeStateScreen.SetActive(false);
            freeStateScreen.SetActive(false);
            capturedStateScreen.SetActive(false);

        }
    }



    void toggleRewards(LimaTrial trial)
    {
        if(!cookies)
        {
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie1PosX,trial.cookie1PosY,trial.cookie1Weight,trial.cookie1RewardValue);
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie2PosX,trial.cookie2PosY,trial.cookie2Weight,trial.cookie2RewardValue);
            cookies = true;
        }
        else
        {
            trialController.GetComponent<TrialController>().despawnRewards();
            cookies = false;
        }

    }

    void setPredator(LimaTrial trial)
    {
        predator.GetComponent<PredatorControls>().setAttack(trial.attackingProb);
    }

    void logTrialData(LimaTrial trial)
    {
        trial.playerPosition =dataHandler.playerPosition;
        trial.predatorPosition = dataHandler.predatorPosition;
        trial.mouseTrackChoicePeriod = dataHandler.mouseTrackChoicePeriod;
        trial.mouseTrackEffortPeriod = dataHandler.mouseTrackEffortPeriod;
        trial.effortRate = dataHandler.effortRate;
        trial.trialStartTime = startTime;
        trial.trialEndTime = endTime;
        trial.trialReward = playerManager.earnedReward;
        trial.trialCookie = playerManager.cookieChoice;
        trial.trialEndState = playerManager.playerState;
    }


   

#endregion

#region Continous Methods

 void UpdateTimer()
    {
        // Assuming movementStartTimeHeadsUpDisplay is a GameObject with UIController script attached
        UIController uiController = HeadsUpDisplay.GetComponent<UIController>();

        if (uiController != null)
        {
            // Call the DecreaseTime method from UIController
            uiController.DecreaseTime(0.01f/10f);
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

#region Old Code
    // void toggleWind()
    // {
    //       if(!wind.activeSelf)
    //     {
    //         wind.SetActive(true);
    //         wind.GetComponent<WindController>().activateWindLayer(playerManager.playerLayer);
    //     }
    //     else
    //     {
    //        wind.GetComponent<WindController>().deactivateWindLayer();
    //        wind.SetActive(false);
    //     }
    // }
#endregion
   
}
