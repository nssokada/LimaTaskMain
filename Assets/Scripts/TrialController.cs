using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrialController : MonoBehaviour
{
    public GameObject[] cookieClose;
    public GameObject[] cookieFar;
    public GameObject cookiePrefab;
    public GameObject acornPrefab;
    public GameObject choose;
    public GameObject move;
    public GameObject probability;

    public GameObject player;
    bool moveInstruct;
    GameObject cookiePos;
    string cookieLayer;

    public Transform centerPt;
    private float radius = 9;
    private float minDistanceBetweenAcorns = 2f; // Minimum distance between acorns
    private List<Vector3> acornPositions = new List<Vector3>();

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
        Debug.Log("probability is"+attackingProb);
        if(attackingProb==0.9f)
        {
            Debug.Log("set high");
            probability.GetComponent<probability>().setProbabilityHigh();
        }
        if(attackingProb==0.5f)
        {
            Debug.Log("set med");
            probability.GetComponent<probability>().setProbabilityMed();
        }
        if(attackingProb==0.1f)
        {
            Debug.Log("set low");
            probability.GetComponent<probability>().setProbabilityLow();
        }

    }

    public void spawnRewards(int x, int y, float weight, int rewardValue)
    {
        if(x>0)
        {
             cookiePos = cookieFar[y];
            //  cookieLayer = "far";
        }
        else
        {
             cookiePos = cookieClose[y];
            //  cookieLayer = "close";
        }
        GameObject newCookie = Instantiate(cookiePrefab, cookiePos.transform.position, cookiePos.transform.rotation);
        newCookie.GetComponent<Cookie>().weight = weight;
        newCookie.GetComponent<Cookie>().rewardValue = rewardValue;
        newCookie.GetComponent<Cookie>().setCookieColor();
        newCookie.GetComponent<Cookie>().setCookieSize();
        newCookie.SetActive(true);
    }


     public void spawnAcorns(int numberOfAcorns)
    {
        acornPositions.Clear(); // Clear previous positions if re-spawning

        for (int i = 0; i < numberOfAcorns; i++)
        {
            Vector3 spawnPosition;
            Quaternion spawnRotation;
            bool positionFound = false;

            for (int j = 0; j < 100; j++) // Try 100 times to find a valid position
            {
                spawnPosition = GetRandomPointInRadius(centerPt.position, radius);

                // Check if the position is within the radius
                if (Vector3.Distance(centerPt.position, spawnPosition) <= radius)
                {
                    // Check if the position is far enough from existing acorns
                    bool isValid = true;
                    foreach (Vector3 pos in acornPositions)
                    {
                        if (Vector3.Distance(pos, spawnPosition) < minDistanceBetweenAcorns)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        acornPositions.Add(spawnPosition);
                        
                        // Generate a random rotation
                        spawnRotation = UnityEngine.Random.rotation;

                        Instantiate(acornPrefab, spawnPosition, spawnRotation);
                        positionFound = true;
                        break;
                    }
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning("Could not find a valid position for acorn " + i);
            }
        }
    }

    private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
    {
        // Generate a random point in a circular area
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomPoint.x, center.y+0.5f, center.z + randomPoint.y);
    }

    public void despawnRewards()
    {
        DestroyAllObjectsWithTag("Cookie");
    }

    public void despawnAcorns()
    {
        DestroyAllObjectsWithTag("Acorn");
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
