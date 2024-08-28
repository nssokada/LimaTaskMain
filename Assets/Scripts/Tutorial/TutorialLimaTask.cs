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

    public LimaTrial trial;

    bool trial_timeUp;
    public bool trialEndable;
    Vector3 home;
    bool cookies;
    bool acorns;

    float movementStartTime; //likely will change this to trial start time
    public GameObject HeadsUpDisplay;
    int numCookies;
    public TMP_Text instructText;    
    public GameObject HandsOnKeys;
    public GameObject EffortDisplay;
    private List<float> pressTimes = new List<float>();


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
            case "cookieTutorial":
                trialEndable = true;
                if(numCookies<=3) StartCoroutine(cookieTutorial());
                else StartCoroutine(endCookieTutorial());
                break;
            case "mapTutorial":
                trialEndable = true;
                if(numCookies<=3) StartCoroutine(mapTutorial(trial));
                else StartCoroutine(endMapTutorial());
                break;
            case "acornTutorial":
                trialEndable = true;
                if(numCookies<=3) StartCoroutine(acornTutorial());
                else StartCoroutine(endacornTutorial());
                break;

            case "effortPeriod":
                if (TutorialController.tutorialState == "navigationTutorial" | TutorialController.tutorialState == "cookieTutorial") EnableEffortPhase(0);
                else if(TutorialController.tutorialState == "mapTutorial") EnableEffortPhase(1);
                break;
             case "endingPeriod":
                HeadsUpDisplay.GetComponent<UIController>().SetHUDReward(0);
                HeadsUpDisplay.GetComponent<UIController>().SetHUDAcorn(0);
                EndTrial();
                break;
            case "nextTrialPeriod":
                if (TutorialController.tutorialState == "navigationTutorial")  gameStateController("navigationTutorial");
                else if (TutorialController.tutorialState == "cookieTutorial")  gameStateController("cookieTutorial");
                else if(TutorialController.tutorialState == "mapTutorial")  gameStateController("mapTutorial");
                else if (TutorialController.tutorialState == "acornTutorial") gameStateController("acornTutorial");
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

void Update()
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
        }
}

//One coroutine to ensure players are holding the keys
    private void keysSetUp()
    {
        HandsOnKeys.SetActive(true);
    }
    void OnEffort()
    {
        if(TutorialController.tutorialState == "effortIntro")
        {
            if(EffortDisplay.activeSelf)
            {
                if (Keyboard.current.sKey.isPressed && Keyboard.current.dKey.isPressed && Keyboard.current.fKey.isPressed)
                {
                    EffortDisplay.GetComponent<UIController>().IncreaseEnergy(0.05f);
                    float energy = EffortDisplay.GetComponent<UIController>().GetEnergy();
                    float pressTime = Time.time;
                    pressTimes.Add(pressTime);
                    if(energy>=0.9) StartCoroutine(endEffortIntro());
                }
            }
        }
    }

     private IEnumerator endEffortIntro()
    {
        float total=0f;
        for (int i = 1; i < pressTimes.Count; i++)
        {
                 total += (pressTimes[i] - pressTimes[i - 1]);
        }
        float latency =  total / (pressTimes.Count - 1);

        PlayerPrefs.SetFloat("PressLatency", latency);
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

        setCookie(Random.Range(0,1),Random.Range(0,6),1f,10);
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
        HeadsUpDisplay.SetActive(false);
        SwitchToTutorial("cookieTutorial");
}


#endregion


#region cookieTutorial

private IEnumerator cookieTutorial()
{   
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Click on the cookie to begin!";
        arena.SetActive(true);
        map.SetActive(true);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        if(numCookies<1)setCookie(Random.Range(0,1),Random.Range(0,6),3f,100);
        else if(numCookies<2)setCookie(Random.Range(0,1),Random.Range(0,6),1f,100);
        else setCookie(Random.Range(0,1),Random.Range(0,6),1f,10);
        Debug.Log("set cookie");

        EnableClickingPeriod(); 
        numCookies++;
}
private IEnumerator endCookieTutorial()
{        
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Great work!";
        yield return new WaitForSeconds(1.5f);
        instructText.text = "Now let's learn about the map";
        yield return new WaitForSeconds(2.0f);
        EffortDisplay.SetActive(false);
        HeadsUpDisplay.SetActive(false);
        SwitchToTutorial("mapTutorial");
}
#endregion


#region Map Tutorial

private IEnumerator mapTutorial(LimaTrial trial)
{
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Click on a cookie to begin!";
        arena.SetActive(true);
        toggleProbability();
        map.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

         //Set Rewards -> Spawns 3 cookies based on trial information
        toggleRewards(trial);     
        yield return new WaitForSeconds(1.0f);

        EnableClickingPeriod(); 
        numCookies++;
}
private IEnumerator endMapTutorial()
{        
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Great work!";
        yield return new WaitForSeconds(1.5f);
        instructText.text = "Now let's learn about the free movement condition";
        yield return new WaitForSeconds(2.0f);
        EffortDisplay.SetActive(false);
        HeadsUpDisplay.SetActive(false);
        SwitchToTutorial("acornTutorial");
}
#endregion


#region Acorn Tutorial
private IEnumerator acornTutorial()
{
        HeadsUpDisplay.SetActive(true);
        instructText.text = "Move your mouse to find acorns";
        arena.SetActive(true);
        toggleProbability();
        map.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

         //Set Rewards -> Spawns 3 cookies based on trial information
        toggleAcorns();     
        yield return new WaitForSeconds(1.0f);

        EnableAcornPeriod();
        numCookies++;
}

private IEnumerator endacornTutorial()
{        
            HeadsUpDisplay.SetActive(true);
        instructText.text = "Great work!";
        yield return new WaitForSeconds(1.5f);
        instructText.text = "Now let's play the game";
        yield return new WaitForSeconds(2.0f);
        EffortDisplay.SetActive(false);
        HeadsUpDisplay.SetActive(false);
        TutorialController.SwitchScene();
}

#endregion

#region  Ending a Trial

    public void EndTrial()
    {
        trialEndable = false;
        StopCoroutine("freeMovement");
        CancelInvoke("UpdateTimer");
        if (TutorialController.tutorialState == "mapTutorial")
        {
            Debug.Log("Long Routine Called");
            StartCoroutine(TrialEndRoutine(trial));
        } 
        else if(TutorialController.tutorialState == "navigationTutorial" | TutorialController.tutorialState =="cookieTutorial")
        {
            Debug.Log("Short Routine Called");
            StartCoroutine(TrialEndRoutineShort(trial));
        } 
        else if (TutorialController.tutorialState == "acornTutorial")
        {
            StartCoroutine(TrialEndRoutineAcorn(trial));
        }
            
    }

    IEnumerator TrialEndRoutineShort(LimaTrial trial)
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
        // toggleRewards(); //removed since we don't have this here
        // toggleProbability();
        setCookie(Random.Range(0,1),Random.Range(0,6),0f,0);
        toggleEndStateScreen();
        gameStateController("nextTrialPeriod");
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
        toggleProbability();
        toggleEndStateScreen();
        resetTimer();
        gameStateController("nextTrialPeriod");
    }

    IEnumerator TrialEndRoutineAcorn(LimaTrial trial)
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
        toggleAcorns();
        toggleProbability();
        toggleEndStateScreen();
        resetTimer();
        gameStateController("nextTrialPeriod");
    }
#endregion





#region  Wrappers and Helpers

    private void SwitchToTutorial(string tutorialState)
    {
        Debug.Log("switching back to video");
        TutorialController.ToggleTask();
        TutorialController.NextVideo(tutorialState);
    }



 //This period is ended by PlayerManager when player picks up the cookie
    public void EnableClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
    }

    public void EnableAcornPeriod()
    {
        player.GetComponent<PlayerMovement>().enableAcorns();
        setPredator(trial);  
        StartCoroutine("freeMovement");

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
            instructText.text = "Now bring the cookie back to safety";
            HeadsUpDisplay.SetActive(true);
            toggleEffort();
            // toggleWind();
            //Sets Predator probability and attack 
            togglePredator();
            setPredator(trial);  
            StartCoroutine("freeMovement");

        }
      
    }

IEnumerator freeMovement()
    { 
        InvokeRepeating("UpdateTimer", 0f, 0.01f);
        yield return new WaitForSeconds(10.0f);
        if(trialEndable)    gameStateController("endingPeriod");

    }
   
 void toggleProbability()
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
                playerManager.carrying = false;
                  foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                    }
           }
           if(playerManager.acorn_carrying) 
            {
                playerManager.acorn_carrying = false;
                  foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Acorn")) Destroy(child.gameObject);
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
            trialController.GetComponent<TrialController>().spawnRewards(trial.cookie3PosX,trial.cookie3PosY,trial.cookie3Weight,trial.cookie3RewardValue);
            cookies = true;
        }
        else
        {
            Debug.Log("destroying reward");
            trialController.GetComponent<TrialController>().despawnRewards();
            cookies = false;
        }

    }


    void toggleAcorns()
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
