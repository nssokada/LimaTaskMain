using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool carrying;
    public GameObject player;
    public GameObject task;
    public string playerState;
    public string cookieState;
    // public string playerLayer;
    public GameObject HeadsUpDisplay;

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
 
                task.GetComponent<LimaTask>().EnableEffortPhase();
                carrying = true; 
            }

            
        }

        else if(carrying)
        {
            
            if(task.GetComponent<LimaTask>().trialEndable)
            {
                if (other.gameObject.tag == "Safety")
                {
                    foreach(Transform child in player.transform)
                    {
                        if(child.CompareTag("Cookie")) Destroy(child.gameObject);
                    }
                    playerState ="escaped";
                    task.GetComponent<LimaTask>().EndTrial();
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
                    task.GetComponent<LimaTask>().EndTrial();
                    carrying = false; 
                }
            }

          
        }

    
       
       
    }
  }

  
    // private void OnTriggerExit(Collider other) {

        //   if (other.CompareTag("Wind"))
        //     {
        //         Vector3 driftVector = player.GetComponent<PlayerMovement>().driftdrawVector; 
        //         HeadsUpDisplay.GetComponent<UIController>().setWind(driftVector,other.GetComponent<Wind>().resistance);
        //         player.GetComponent<PlayerMovement>().resistance = 0f;
        //         player.GetComponent<PlayerMovement>().theta = 0f;
        //     }

        // if (other.CompareTag("MediumEffort"))
        // {
        //     if(cookieState=="heavy")
        //     {
        //         player.GetComponent<PlayerMovement>().stepSize = 0.15f;
        //         player.GetComponent<PlayerMovement>().pressLimit = 2;
        //         player.GetComponent<PlayerMovement>().resetEnergy();
        //     }
        //     else if(cookieState=="light")
        //     {
        //         player.GetComponent<PlayerMovement>().stepSize = 0.5f;
        //         player.GetComponent<PlayerMovement>().pressLimit = 2;
        //         player.GetComponent<PlayerMovement>().resetEnergy();
        //     }
        // }

        // if (other.CompareTag("LowEffort"))
        // {
        //      if(cookieState=="heavy")
        //     {
        //         player.GetComponent<PlayerMovement>().stepSize = 0.15f;
        //         player.GetComponent<PlayerMovement>().pressLimit = 2;
        //         player.GetComponent<PlayerMovement>().resetEnergy();
        //     }
        //     else if(cookieState=="light")
        //     {
        //         player.GetComponent<PlayerMovement>().stepSize = 0.5f;
        //         player.GetComponent<PlayerMovement>().pressLimit = 2;
        //         player.GetComponent<PlayerMovement>().resetEnergy();
        //     }
        // }

        // } 
  

    // private void OnTriggerStay(Collider other)
    // {
    //     // Check if the collider in the trigger area has a specific tag
    //     if (other.CompareTag("HighEffort"))
    //     {
    //         player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/4;
    //     }

    //     if (other.CompareTag("MediumEffort"))
    //     {
    //         player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/3;
    //     }

    //     if (other.CompareTag("LowEffort"))
    //     {
    //         player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/2;
    //     }
    // }

// if (other.CompareTag("Wind"))
//             {
//                  Vector3 driftVector = player.GetComponent<PlayerMovement>().driftdrawVector; 
//                 HeadsUpDisplay.GetComponent<UIController>().setWind(driftVector,other.GetComponent<Wind>().resistance);
//                 player.GetComponent<PlayerMovement>().resistance = other.GetComponent<Wind>().resistance;
//                 player.GetComponent<PlayerMovement>().theta = other.GetComponent<Wind>().theta;
//             }

    //   if (other.CompareTag("HighEffort"))
    //         {
    //             if(cookieState=="heavy")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.1f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 8;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }
    //             else if(cookieState=="light")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.1625f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 8;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }

    //         }

    //         if (other.CompareTag("MediumEffort"))
    //         {
    //             if(cookieState=="heavy")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.2f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 4;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }
    //             else if(cookieState=="light")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.325f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 4;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }
    //         }

    //         if (other.CompareTag("LowEffort"))
    //         {
    //             if(cookieState=="heavy")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.26f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 3;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }
    //             else if(cookieState=="light")
    //             {
    //                 player.GetComponent<PlayerMovement>().stepSize = 0.43f;
    //                 player.GetComponent<PlayerMovement>().pressLimit = 3;
    //                 player.GetComponent<PlayerMovement>().resetEnergy();
    //             }
    //         }

    //         if (other.CompareTag("Wind"))
    //         {
    //             Vector3 driftVector = player.GetComponent<PlayerMovement>().driftdrawVector; 
    //             HeadsUpDisplay.GetComponent<UIController>().setWind(driftVector,other.GetComponent<Wind>().resistance);
    //             player.GetComponent<PlayerMovement>().resistance = other.GetComponent<Wind>().resistance;
    //             player.GetComponent<PlayerMovement>().theta = other.GetComponent<Wind>().theta;
    //         }