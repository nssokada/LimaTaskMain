using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLimaTask : MonoBehaviour
{

    public GameObject trialController;
    public GameObject TutorialController;

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


    bool trial_timeUp;
    public bool trialEndable;
    Vector3 home;
    bool cookies;
    float movementStartTime; //likely will change this to trial start time
    public GameObject HeadsUpDisplay;



#region  Running the Tutorial

    //Upon enabling this gameobject the first trial will run.
    public void OnEnable()
    {
       startTutorialCase();
    }

    public void startTutorialCase()
    {
        trialEndable = true;
        int  tutorialType = 1;
       switch (tutorialType)
        {
            case 1:
                StartCoroutine(CookieSelection());
                break;
            case 2:
                StartCoroutine(CookieSelection());
                break;
            case 3:
                StartCoroutine(CookieSelection());
                break;
            case 4:
                StartCoroutine(CookieSelection());
                break;
        }
    }

#endregion 


#region  Cookie Selection: Intro 1
    
    private IEnumerator CookieSelection()
    {
        arena.SetActive(true);
        map.SetActive(true);
        togglePlayer();     
        yield return new WaitForSeconds(1.0f);

        setCookie(0,0,0f,0);     
               
        EnableClickingPeriod();  
    }

#endregion 





#region  Wrappers and Helpers

 //This period is ended by PlayerManager when player picks up the cookie
    public void EnableClickingPeriod()
    {
        player.GetComponent<PlayerMovement>().clickingPeriod = true;
    }
   
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
            home = player.transform.position;
            player.SetActive(true);
        }

        else
        {
           playerManager.playerState = "free";
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

     void setCookie(int x, int y, float weight, int value)
    {
        if(!cookies)
        {
            trialController.GetComponent<TrialController>().spawnRewards(x,y,weight,value);
            cookies = true;
        }
        else
        {
            trialController.GetComponent<TrialController>().despawnRewards();
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
