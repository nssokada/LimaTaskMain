using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Tutorial : PlayerManager
{

    private void OnTriggerEnter(Collider other) 
    {
        
        if(!carrying){
            if (other.gameObject.tag == "Cookie")
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
                else if (task.GetComponent<TutorialLimaTask>().TutorialController.tutorialState == "navigationTutorial") task.GetComponent<TutorialLimaTask>().gameStateController("navigationTutorialEffortPeriod");
                else if (task.GetComponent<TutorialLimaTask>().TutorialController.tutorialState == "cookieTutorial") task.GetComponent<TutorialLimaTask>().gameStateController("cookieTutorialEffortPeriod");
            }

            
        }

        else if(carrying)
        {
            
            if(task.GetComponent<TutorialLimaTask>().trialEndable)
            {
                if (other.gameObject.tag == "Safety")
                {
                    foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                    }
                    playerState ="escaped";
                    
                    
                   
                    task.GetComponent<TutorialLimaTask>().gameStateController("endingPeriod");
                    Debug.Log("earn reward");
                    carrying = false; 
                }
                else if (other.gameObject.tag == "Predator")
                {
                     foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                    }
                    playerState ="captured";
                    task.GetComponent<TutorialLimaTask>().gameStateController("endingPeriod");
                    carrying = false; 
                }
            }

          
        }
       
    }




  }
