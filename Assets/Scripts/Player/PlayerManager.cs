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

    private void OnTriggerEnter(Collider other) 
    {
        
        if(!carrying){
            if (other.gameObject.tag == "Cookie")
            {
                other.transform.parent = transform;

                if(other.gameObject.GetComponent<Cookie>().weight>1) 
                {
                    cookieState = "heavy";
                    player.GetComponent<PlayerMovement>().stepSize = 0.25f;
                }
                else 
                {
                    cookieState = "light";
                    player.GetComponent<PlayerMovement>().stepSize = 0.5f;
                }
 
                task.GetComponent<LimaTask>().EffortPeriod();
                carrying = true; 
            }
        }

        else if(carrying)
        {
            
            if(task.GetComponent<LimaTask>().trialEndable)
            {
                if (other.gameObject.tag == "Safety")
                {
                    Destroy(player.transform.GetChild(4).gameObject);
                    player.GetComponent<PlayerMovement>().stepSize = 1;
                    task.GetComponent<LimaTask>().EndTrial();
                    Debug.Log("earn reward");
                    carrying = false; 
                }
                 else if (other.gameObject.tag == "Predator")
                {
                    Destroy(player.transform.GetChild(4).gameObject);
                    player.GetComponent<PlayerMovement>().stepSize = 1;
                    task.GetComponent<LimaTask>().EndTrial();
                    carrying = false; 
                }
                 else if (other.gameObject.tag == "Bounds")
                {
                    Vector3 newPosition = player.transform.position - (transform.forward * 0.5f);
                    player.transform.position = newPosition;
                }      //Maybe this 
            }

                if (other.CompareTag("HighEffort"))
            {
                if(cookieState=="heavy")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.1f;
                    player.GetComponent<PlayerMovement>().pressLimit = 8;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }
                else if(cookieState=="light")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.1625f;
                    player.GetComponent<PlayerMovement>().pressLimit = 8;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }

            }

            if (other.CompareTag("MediumEffort"))
            {
                if(cookieState=="heavy")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.2f;
                    player.GetComponent<PlayerMovement>().pressLimit = 4;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }
                else if(cookieState=="light")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.325f;
                    player.GetComponent<PlayerMovement>().pressLimit = 4;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }
            }

            if (other.CompareTag("LowEffort"))
            {
                if(cookieState=="heavy")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.26f;
                    player.GetComponent<PlayerMovement>().pressLimit = 3;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }
                else if(cookieState=="light")
                {
                    player.GetComponent<PlayerMovement>().stepSize = 0.43f;
                    player.GetComponent<PlayerMovement>().pressLimit = 3;
                    player.GetComponent<PlayerMovement>().resetEnergy();
                }
            }
        }

    
       
       
    }

    private void OnTriggerExit(Collider other) {
        if(carrying)
        {
            if (other.CompareTag("HighEffort"))
            {
            if(cookieState=="heavy")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.15f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }
            else if(cookieState=="light")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.5f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }

        }

        if (other.CompareTag("MediumEffort"))
        {
            if(cookieState=="heavy")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.15f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }
            else if(cookieState=="light")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.5f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }
        }

        if (other.CompareTag("LowEffort"))
        {
             if(cookieState=="heavy")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.15f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }
            else if(cookieState=="light")
            {
                player.GetComponent<PlayerMovement>().stepSize = 0.5f;
                player.GetComponent<PlayerMovement>().pressLimit = 2;
                player.GetComponent<PlayerMovement>().resetEnergy();
            }
        }

        } 
    }

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

}
