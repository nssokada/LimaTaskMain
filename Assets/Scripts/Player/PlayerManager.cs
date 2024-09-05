using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool carrying;
    public bool acorn_carrying;

    public GameObject player;
    public GameObject task;
    public string playerState;
    public string cookieState;
    public CookieChoice cookieChoice;
    public List<PositionHandler> acornsCollected; 
    public float earnedReward;
    private float rewardPotential;
    public GameObject HeadsUpDisplay;
    private bool exitSafety;

    void OnEnable()
    {
        playerState ="free";
        cookieState = "";
        cookieChoice = new CookieChoice();
        acornsCollected= new List<PositionHandler>();
        earnedReward = 0;
        rewardPotential=0;
        exitSafety = false;
    }
    private void OnTriggerEnter(Collider other) 
    {
        
            if (other.gameObject.tag == "Cookie")
            {
                Debug.Log("cookie hit");
                other.gameObject.GetComponent<Collider>().enabled = false;
                float weight  = other.gameObject.GetComponent<Cookie>().weight;
                float rewardValue = other.gameObject.GetComponent<Cookie>().rewardValue;
                Vector3 cookiePosition = other.gameObject.transform.position;

                other.transform.parent = player.transform;

                if(weight>1) 
                {
                    cookieState = "heavy";
                    // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                    player.GetComponent<PlayerMovement>().cookieWeight = 1f; //speed multiplier higher is faster
                    HeadsUpDisplay.GetComponent<UIController>().SetHUDReward((int)rewardValue);
                }
                else 
                {
                    cookieState = "light";
                    // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                    player.GetComponent<PlayerMovement>().cookieWeight = 2f; //speed multiplier higher is faster
                    HeadsUpDisplay.GetComponent<UIController>().SetHUDReward((int)rewardValue);
                }

                rewardPotential = rewardValue;
                cookieChoice = new CookieChoice(rewardValue,weight,cookiePosition.x,cookiePosition.z,Time.realtimeSinceStartup);
                task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EffortPeriod);
                carrying = true; 
            }

          else if(other.gameObject.tag == "Safety")
        {
            if( exitSafety && task.GetComponent<LimaTask>().trialEndable)
            {
                foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);                        
                        else if(child.CompareTag("Acorn")) Destroy(child.gameObject);

                    }
                    playerState ="escaped";
                    earnedReward = rewardPotential;
                    Debug.Log("earn reward");
                    carrying = false;                    
                    acorn_carrying = false; 
                    task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EndingPeriod);
            }
        }
        
        else if(other.gameObject.tag == "Predator")
        {
            if( exitSafety && task.GetComponent<LimaTask>().trialEndable)
            {
                foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                        else if(child.CompareTag("Acorn")) Destroy(child.gameObject);
                    }
                    playerState ="captured";
                    earnedReward =-25f;
                    carrying = false;                     
                    acorn_carrying = false; 
                    task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EndingPeriod);
            }
        }

        else if(other.gameObject.tag =="Acorn")
            {
                other.gameObject.GetComponent<Collider>().enabled = false;
                logAcornPosition(other.gameObject);
                Debug.Log("Acorn Hit");
                HeadsUpDisplay.GetComponent<UIController>().SetHUDAcorn(2);
                rewardPotential += 2;
                foreach (Transform child in other.gameObject.transform)
                    {
                            // Set each child to active
                            child.gameObject.SetActive(true); // Set to false if you want to deactivate
                    }

                other.transform.parent = player.transform;
                acorn_carrying = true; 
            }
    }


     private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Safety")
        {
            exitSafety = true;
        }
    }


    void logAcornPosition(GameObject gameObject)
    {
        PositionHandler acornPosition = new PositionHandler(gameObject.transform.position.x,gameObject.transform.position.z,Time.realtimeSinceStartup);
        acornsCollected.Add(acornPosition);
    }
  }

