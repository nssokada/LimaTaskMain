using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrialController : MonoBehaviour
{
    public GameObject[] cookieClose;
    public GameObject[] cookieFar;
    public GameObject cookiePrefab;
    public GameObject choose;
    public GameObject move;
    public GameObject probability;

    public GameObject player;
    bool moveInstruct;
    GameObject cookiePos;

    void update()
    {
        if(player.GetComponent<PlayerMovement>().effortPeriod == true && moveInstruct ==false)
        {
            moveInstruct=true;
            StartCoroutine("moveInstructionPeriod");
        }
    }
    
    public void chooseInstruction()
    {
        choose.SetActive(true);
    }

    public void moveInstruction()
    {
        StartCoroutine("moveInstructionPeriod");
    }

    IEnumerator moveInstructionPeriod()
    {
        move.SetActive(true);
        choose.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        move.SetActive(false);
    }

    public void endDisplay()
    {
        //Insert UI for end display here
    }

    public void setProbability(float attackingProb)
    {
        if(attackingProb==0.9)
        {
            Debug.Log("set hight");
            probability.GetComponent<probability>().setProbabilityHigh();
        }
        if(attackingProb==0.5)
        {
            Debug.Log("set med");
            probability.GetComponent<probability>().setProbabilityMed();
        }
        if(attackingProb==0.1)
        {
            Debug.Log("set low");
            probability.GetComponent<probability>().setProbabilityLow();
        }

    }

    public void spawnRewards(int x, int y, float weight, float rewardValue)
    {
        if(x>0)
        {
             cookiePos = cookieFar[y];
        }
        else
        {
             cookiePos = cookieClose[y];
        }
        GameObject newCookie = Instantiate(cookiePrefab, cookiePos.transform.position, cookiePos.transform.rotation);
        newCookie.GetComponent<Cookie>().weight = weight;
        newCookie.GetComponent<Cookie>().rewardValue = rewardValue;
        newCookie.GetComponent<Cookie>().setCookieColor();
        newCookie.GetComponent<Cookie>().setCookieSize();
        newCookie.SetActive(true);
    }

    public void despawnRewards()
    {
        DestroyAllObjectsWithTag("Cookie");
    }

    public void DestroyAllObjectsWithTag(string tag)
    {
        GameObject[] cookies = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject cookie in cookies)
        {
            Destroy(cookie);
        }
    }
}
