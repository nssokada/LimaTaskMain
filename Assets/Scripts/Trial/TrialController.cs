using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrialController : MonoBehaviour
{
    public GameObject[] cookieClose;
    public GameObject[] cookieFar;
    public GameObject[] cookieMid;

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
    public List<Vector3> acornPositions = new List<Vector3>();
    public List<PositionHandler> acornLoggedPositions;


    void OnEnable()
    {
        acornLoggedPositions = new List<PositionHandler>();
    }
    void update()
    {
        if (player.GetComponent<PlayerMovement>().effortPeriod == true && moveInstruct == false)
        {
            moveInstruct = true;
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
        Debug.Log("probability is" + attackingProb);
        if (attackingProb == 0.9f)
        {
            Debug.Log("set high");
            probability.GetComponent<probability>().setProbabilityHigh();
        }
        if (attackingProb == 0.5f)
        {
            Debug.Log("set med");
            probability.GetComponent<probability>().setProbabilityMed();
        }
        if (attackingProb <= 0.1f)
        {
            Debug.Log("set low");
            probability.GetComponent<probability>().setProbabilityLow();
        }

    }

    public void spawnRewards(int x, int y, float weight, int rewardValue)
    {
        if (x == 2)
        {
            Debug.Log("Attempting far" + y);  // Use Length for arrays or Count for lists
            Debug.Log(cookieFar.Length);  // Use Length for arrays or Count for lists
            cookiePos = cookieFar[y];
        }
        else if (x == 1)
        {
            Debug.Log("Attempting far" + y);  // Use Length for arrays or Count for lists
            Debug.Log(cookieFar.Length);  // Use Length for arrays or Count for lists
            cookiePos = cookieMid[y];
        }
        else
        {
            Debug.Log(cookieClose.Length);  // Use Length for arrays or Count for lists
            Debug.Log("Attempting close" + y);  // Use Length for arrays or Count for lists
            cookiePos = cookieClose[y];
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
    acornLoggedPositions.Clear();

    for (int i = 0; i < numberOfAcorns; i++)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        bool positionFound = false;

        for (int j = 0; j < 100; j++) // Try 100 times to find a valid position
        {
            spawnPosition = GetBiasedRandomPointInRadius(centerPt.position, radius);

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
                    else if (Vector3.Distance(centerPt.position, spawnPosition) < minDistanceBetweenAcorns)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    logAcornPosition(spawnPosition);

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

// Helper method that biases spawning toward the outer edge
private Vector3 GetBiasedRandomPointInRadius(Vector3 center, float maxRadius)
{
    // Generate a random angle
    float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
    
    // Use power function to bias toward outer edge
    // Higher power = more bias toward edge
    float biasStrength = 2f; // Adjust this value (1 = uniform, higher = more edge bias)
    float normalizedRadius = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 1f / biasStrength);
    float distance = normalizedRadius * maxRadius;
    
    // Convert polar coordinates to Cartesian (preserve Y coordinate from center)
    return new Vector3(
        center.x + Mathf.Cos(angle) * distance,
        center.y + 0.5f, // Match the original +0.5f Y offset
        center.z + Mathf.Sin(angle) * distance
    );
    
}


    void logAcornPosition(Vector3 position)
    {
        PositionHandler acornPosition = new PositionHandler(position.x, position.z, Time.realtimeSinceStartup);
        acornLoggedPositions.Add(acornPosition);
    }
    private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
    {
        // Generate a random point in a circular area
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomPoint.x, center.y + 0.5f, center.z + randomPoint.y);
    }

    public void despawnRewards()
    {
        GameObject[] cookies = GameObject.FindGameObjectsWithTag("Cookie");

        foreach (GameObject cookie in cookies)
        {
            if (cookie.transform.parent == null) // Check if the cookie does not have a parent
            {
                Destroy(cookie);
            }
        }
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
