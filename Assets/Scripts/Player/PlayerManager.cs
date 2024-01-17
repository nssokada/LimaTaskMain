using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool carrying;
    public GameObject player;
    public GameObject task;
    public string playerState;

    private void OnTriggerEnter(Collider other) 
    {
        
        if(!carrying){
            if (other.gameObject.tag == "Cookie")
            {
                other.transform.parent = transform; 
                player.GetComponent<PlayerMovement>().stepSize = 1f/other.gameObject.GetComponent<Cookie>().weight;
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
        }

    
          // Check if the collider in the trigger area has a specific tag
        if (other.CompareTag("HighEffort"))
        {
            player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/4;
        }

        if (other.CompareTag("MediumEffort"))
        {
            player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/3;
        }

        if (other.CompareTag("LowEffort"))
        {
            player.GetComponent<PlayerMovement>().stepSize = player.GetComponent<PlayerMovement>().stepSize/2;
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
