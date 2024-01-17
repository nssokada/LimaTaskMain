using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimaTask : MonoBehaviour
{
    public GameObject trialController;
    public GameObject SessionGenerator;


    public GameObject player;
    public GameObject predator;

    public GameObject arena;
    public GameObject map;

    public GameObject probabilityDisplay;


    private LimaTrial trial;


    bool trial_timeUp;
    public bool trialEndable;
    Vector3 home;
    bool cookies;
    
  

#region  Running a Trial

    //Upon enabling this gameobject the first trial will run.
    public void OnEnable()
    {
       startNextTrial();
    }

    public void startNextTrial()
    {
        trialEndable = true;
        int trialNum = PlayerPrefs.GetInt("trialNum");
        trial  = SessionGenerator.GetComponent<SessionGenerator>().trials[trialNum];
        StartCoroutine(LimaSpawnSequence(trial));
        //NEED TO INVOKE DATA METHODS
    }

    IEnumerator LimaSpawnSequence(LimaTrial trial)
    {
        arena.SetActive(true);
        
        //Set Probabilty -> sets probability material shows probability display for 1 second 
        toggleProbability(trial);
        yield return new WaitForSeconds(1.0f);
        map.SetActive(true);
        //Set Player -> sets player home status, enables player
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        //Set Rewards -> Spawns 2 cookies based on trial information
        toggleRewards(trial);     
        yield return new WaitForSeconds(1.0f);
        //check this out next
               
        ClickingPeriod();  
        Debug.Log("End Lima Sequence");   
    }


    //This period is ended by PlayerManager when player picks up the cookie
    public void ClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
        //check this
        //try this out
    }

     public void EffortPeriod()
    {

        Debug.Log("Starting freemovement Sequence");   

        player.GetComponent<PlayerMovement>().effortPeriod = true;
         
         //Sets Predator probability and attack 
        togglePredator();
        setPredator(trial);  
        StartCoroutine("freeMovement");
    }

    IEnumerator freeMovement()
    { 
        yield return new WaitForSeconds(10.0f);
        if(trialEndable) EndTrial();
    }

 


#endregion

#region  Ending a Trial

    public void EndTrial()
    {
        trialEndable = false;
        StopCoroutine("freeMovement");
        StartCoroutine(TrialEndRoutine(trial));
    }

    public void OnTrialEnd()
    {
        //NEED TO PUSH DATA HERE
        int trialNum = PlayerPrefs.GetInt("trialNum");
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
        yield return new WaitForSeconds(2.0f);
        arena.SetActive(false);
        map.SetActive(false);
        toggleRewards(trial);
        togglePlayer();
        toggleProbability(trial);
        togglePredator();
        OnTrialEnd();
    }
#endregion

#region  Wrappers and Helpers
   
 void toggleProbability(LimaTrial trial)
    {
        if(!probabilityDisplay.activeSelf)
        {
            probabilityDisplay.SetActive(true);
            trialController.GetComponent<TrialController>().setProbability(trial.attackingProb);
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
            home = player.transform.position;
            player.SetActive(true);
        }

        else
        {
           player.GetComponent<PlayerManager>().playerState = "0";
           player.GetComponent<PlayerMovement>().effortPeriod = false;
           player.GetComponent<PlayerMovement>().clickingPeriod = false;
           if(player.GetComponent<PlayerManager>().carrying)
           {
                player.GetComponent<PlayerManager>().carrying = false;
                Destroy(player.transform.GetChild(4).gameObject);
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

#endregion


   
}
