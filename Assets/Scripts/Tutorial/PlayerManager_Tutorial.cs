using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Tutorial : PlayerManager
{

    private bool exitSafety;

    private void OnEnable()
    {
        exitSafety = false;
    }
    private void OnTriggerEnter(Collider other) 
    {
        
            if (other.gameObject.tag == "Cookie")
            {
                if(!carrying)
                { 
                    other.transform.parent = player.transform;

                    if(other.gameObject.GetComponent<Cookie>().weight>1) 
                    {
                        cookieState = "heavy";
                        // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                        player.GetComponent<PlayerMovement>().cookieWeight = 1f; //speed multiplier higher is faster
                        HeadsUpDisplay.GetComponent<UIController>().SetHUDReward(other.gameObject.GetComponent<Cookie>().rewardValue);
                    }
                    else 
                    {
                        cookieState = "light";
                        // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                        player.GetComponent<PlayerMovement>().cookieWeight = 2f; //speed multiplier higher is faster
                        HeadsUpDisplay.GetComponent<UIController>().SetHUDReward(other.gameObject.GetComponent<Cookie>().rewardValue);
                    }
                    carrying = true; 
                    if (task.GetComponent<TutorialLimaTask>().TutorialController.tutorialState == "cookieSelection") task.GetComponent<TutorialLimaTask>().gameStateController("cookieSelection");
                    else task.GetComponent<TutorialLimaTask>().gameStateController("effortPeriod");

                }
            }

       
        else if(other.gameObject.tag =="Acorn")
            {
                other.gameObject.GetComponent<Collider>().enabled = false;
                Debug.Log("Acorn Hit");
                HeadsUpDisplay.GetComponent<UIController>().SetHUDAcorn(2);
                foreach (Transform child in other.gameObject.transform)
                    {
                            // Set each child to active
                            child.gameObject.SetActive(true); // Set to false if you want to deactivate
                    }

                    other.transform.parent = player.transform;
                    acorn_carrying = true; 
            }

        
        else if(other.gameObject.tag == "Safety")
        {
            if( exitSafety && task.GetComponent<TutorialLimaTask>().trialEndable)
            {
                foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);                        
                        else if(child.CompareTag("Acorn")) Destroy(child.gameObject);

                    }
                    playerState ="escaped";
                    
                
                    task.GetComponent<TutorialLimaTask>().gameStateController("endingPeriod");
                    Debug.Log("earn reward");
                    carrying = false;                    
                    acorn_carrying = false; 
 
            }
        }

        else if(other.gameObject.tag == "Predator")
        {
            if( exitSafety && task.GetComponent<TutorialLimaTask>().trialEndable)
            {
                foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                        else if(child.CompareTag("Acorn")) Destroy(child.gameObject);
                    }
                    playerState ="captured";
                    task.GetComponent<TutorialLimaTask>().gameStateController("endingPeriod");
                    carrying = false;                     
                    acorn_carrying = false; 

            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Safety")
        {
            exitSafety = true;
        }
    }

  }
