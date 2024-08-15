using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialLimaTask : MonoBehaviour
{
    PlayerInput playerInput;

    public GameObject trialController;
    public TutorialController TutorialController;

    public GameObject endStateScreen;
    public GameObject escapeStateScreen;
    public GameObject freeStateScreen;
    public GameObject capturedStateScreen;

    public GameObject player;
    public PlayerManager playerManager;
    public GameObject predator;

    public GameObject arena;
    public GameObject map;
   
    public GameObject probabilityDisplay;

    private LimaTrial trial;

    bool trial_timeUp;
    public bool trialEndable;
    Vector3 home;
    bool cookies;
    float movementStartTime; //likely will change this to trial start time
    public GameObject HeadsUpDisplay;
    int numCookies;
    public TMP_Text instructText;    
    public GameObject HandsOnKeys;
    public GameObject EffortDisplay;


#region  Running the Tutorial

    //Upon enabling this gameobject the first trial will run.
    public void OnEnable()
    {
       numCookies=0;
       Debug.Log(numCookies);
       gameStateController(TutorialController.tutorialState);
    }


/// <GameStateController>
/// This is a game state controller for the Tutorial. It controls the shift in states within the tutorial of the game. 
/// Reference the main game state controller for more specifics.
/// </summary>
/// <param name="gameState"></param>
    public void gameStateController(string gameState)
    {   
        switch (gameState)
        {
            case "cookieSelection":
                Debug.Log("Running tutorial intro");
                if(numCookies<=3) StartCoroutine(CookieSelection());
                else StartCoroutine(endCookieSelection());
                break;
            case "effortIntro":
                keysSetUp();
                break;
            case "navigationTutorial":
                trialEndable = true;
                if(numCookies<=3) StartCoroutine(navigationTutorial());
                else StartCoroutine(endNavigationTutorial());
                break;
            case "navigationTutorialEffortPeriod":
                EnableEffortPhase(0);
                break;
             case "endingPeriod":
                EndTrial();
                break;
            case "nextTrialPeriod":
                if (TutorialController.tutorialState == "navigationTutorial")  gameStateController("navigationTutorial");
                // OnTrialEnd();
                break;
        }
    }

#endregion 


#region  Cookie Selection: Intro 1
    
    private IEnumerator CookieSelection()
    {

        if(numCookies!=0) 
        {
            yield return new WaitForSeconds(1.0f);
            setCookie(Random.Range(0,1),Random.Range(0,6),0f,0);
            togglePlayer();     
        }

        HeadsUpDisplay.SetActive(true);
        instructText.text = "Click on the cookies as they appear!";
        arena.SetActive(true);
        map.SetActive(true);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        setCookie(Random.Range(0,1),Random.Range(0,6),0f,0);
               
        EnableClickingPeriod(); 
        numCookies++;
 
    }

      private IEnumerator endCookieSelection()
    {
        //something weird is happening here so we need to set this up like this. I will evaluate in code review on Friday
        setCookie(Random.Range(0,1),Random.Range(0,6),0f,0);
        togglePlayer();
        Debug.Log("last call");
        instructText.text = "Great work! Now let's learn about movement in the game!";
        yield return new WaitForSeconds(4.0f);
        HeadsUpDisplay.SetActive(false);   
        SwitchToTutorial("effortIntro");
    }

#endregion 


#region Effort Intro

//One coroutine to ensure players are holding the keys
    private void keysSetUp()
    {
        HandsOnKeys.SetActive(true);
    }
    void OnEffort()
    {
        if(TutorialController.tutorialState == "effortIntro")
        {
            if(HandsOnKeys.activeSelf)
            {
                if (Keyboard.current.sKey.isPressed && Keyboard.current.dKey.isPressed && Keyboard.current.fKey.isPressed)
                {
                    HandsOnKeys.SetActive(false);
                    EffortDisplay.GetComponent<UIController>().SetEnergy(0.08f);
                    EffortDisplay.SetActive(true);
                }
            }
            else
            {
                if (Keyboard.current.sKey.isPressed && Keyboard.current.dKey.isPressed && Keyboard.current.fKey.isPressed)
                {
                    EffortDisplay.GetComponent<UIController>().IncreaseEnergy(0.05f);
                    float energy = EffortDisplay.GetComponent<UIController>().GetEnergy();
                    if(energy>=1) StartCoroutine(endEffortIntro());
                }
            }
        }
    }

     private IEnumerator endEffortIntro()
    {
        EffortDisplay.GetComponent<UIController>().energyText.text = "Great work!";
        yield return new WaitForSeconds(4.0f);
        EffortDisplay.GetComponent<UIController>().energyText.text = "Now let's learn about navigating the game!";
        yield return new WaitForSeconds(4.0f);
        EffortDisplay.SetActive(false);
        SwitchToTutorial("navigationTutorial");
    }

//One coroutine for effort


#endregion


#region Navigation Tutorial

private IEnumerator navigationTutorial()
{
    
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Click on the cookie to begin!";
        arena.SetActive(true);
        map.SetActive(true);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        setCookie(Random.Range(0,1),Random.Range(0,6),1f,1);
        Debug.Log("set cookie");

        EnableClickingPeriod(); 
        numCookies++;
}

private IEnumerator endNavigationTutorial()
{        
    
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Great work!";
        yield return new WaitForSeconds(2.0f);
        instructText.text = "Now let's learn about the cookies";
        yield return new WaitForSeconds(2.0f);
        EffortDisplay.SetActive(false);
        SwitchToTutorial("navigationTutorial");
}


#endregion



#region  Ending a Trial

    public void EndTrial()
    {
        trialEndable = false;
        StopCoroutine("freeMovement");
        CancelInvoke("UpdateTimer");
        StartCoroutine(TrialEndRoutine(trial));
            
    }

    // public void OnTrialEnd()
    // {
    //     //NEED TO PUSH DATA HERE
    //     int trialNum = PlayerPrefs.GetInt("trialNum");
    //     trialNum++;
    //     PlayerPrefs.SetInt("trialNum", trialNum);

    //     if(trialNum < SessionGenerator.GetComponent<SessionGenerator>().numTrials)
    //     {
    //         Debug.Log("trialNum"+trialNum);
    //         Debug.Log("evaluating next");
    //         startNextTrial();
    //     }
    //     else
    //     {
    //         Application.Quit();
    //         Debug.Log("end reached");
    //     }        
    // }

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
        // toggleRewards();
        // toggleProbability();
        setCookie(Random.Range(0,1),Random.Range(0,6),0f,0);
        toggleEndStateScreen();
        gameStateController("nextTrialPeriod");
    }
#endregion





#region  Wrappers and Helpers

    private void SwitchToTutorial(string tutorialState)
    {
        TutorialController.ToggleTask();
        TutorialController.NextVideo(tutorialState);
    }



 //This period is ended by PlayerManager when player picks up the cookie
    public void EnableClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
    }


     public void EnableEffortPhase(int version)
    {
        if (version == 0)
        {
            Debug.Log("Starting freemovement Sequence");   
            instructText.text = "Now bring the cookie back to safety";
            HeadsUpDisplay.SetActive(true);
            toggleEffort();
        }
        else if (version == 1)
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
      
    }

   
 void toggleProbability()
    {
        if(!probabilityDisplay.activeSelf)
        {
            Debug.Log("changingMaterial color");
            probabilityDisplay.SetActive(true);

        }

        else
        {
            probabilityDisplay.SetActive(false);
        }
    }

    public void togglePlayer()
    {
        if(!player.activeSelf)
        {
            home = player.transform.position;
            player.SetActive(true);
        }

        else
        {
            Debug.Log("Toggle player off");
           playerManager.playerState = "free";
           player.GetComponent<PlayerMovement>().effortPeriod = false;
           player.GetComponent<PlayerMovement>().clickingPeriod = false;
           
           Debug.Log("playerManager.carrying"+playerManager.carrying);
           
           if(playerManager.carrying)
           {
                Debug.Log("does this work?");
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

     void setCookie(int x, int y, float weight, int value)
    {
        if(!cookies)
        {
            trialController.GetComponent<TrialController>().spawnRewards(x,y,weight,value);
            cookies = true;
        }
        else
        {
            cookies = false;
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


}
