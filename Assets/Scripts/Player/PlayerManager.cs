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
                    Destroy(transform.GetChild(3).gameObject);
                    player.GetComponent<PlayerMovement>().stepSize = 1;
                    task.GetComponent<LimaTask>().EndTrial();
                    Debug.Log("earn reward");
                    carrying = false; 
                }
                 else if (other.gameObject.tag == "Predator")
                {
                    Destroy(transform.GetChild(3).gameObject);
                    player.GetComponent<PlayerMovement>().stepSize = 1;
                    task.GetComponent<LimaTask>().EndTrial();
                    carrying = false; 
                }      
            }
        }
    }

}
