using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorControls : MonoBehaviour
{
    public GameObject player;
    public GameObject predator;
    public GameObject fox;
    public Vector3[] spawningPositionsPredator;
    public TrialDataHandler dataHandler;
    public bool tutorialMode;
    public bool circaStrike_bool;
    public bool strike_bool;

    private float speed;
    public Animator foxAnimator;

    private void OnEnable()
    {
        if (player.GetComponent<PlayerMovement>().acornPeriod)
        {
            speed = 8f;
        }
        else
        {
            speed = CalculatePredatorSpeed();
        }
    }

    float CalculatePredatorSpeed()
    {
        // Fetch and log press latency from PlayerPrefs
        float MinPressLatency = PlayerPrefs.GetFloat("PressLatency"); // Add default value to avoid null issues
        float MinPressCount = PlayerPrefs.GetFloat("PressCount");
        float stepSize = 8.25f / (MinPressCount);
        float playerSpeed = stepSize / MinPressLatency;

        //Predator Speed is 4 times faster than Player Speed
        return playerSpeed * 4f;

    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (circaStrike_bool)
        {
            fox.SetActive(true);
            predator.transform.LookAt(player.transform.position, Vector3.up);
            if (strike_bool)
            {
                predator.transform.position = Vector3.MoveTowards(predator.transform.position, player.transform.position, speed * Time.deltaTime);
                predator.transform.LookAt(player.transform.position, Vector3.up);
            }
        }
        else
        {
            foxAnimator.speed = 0f;
        }
    }

    void strike()
    {
        // Set the predator's spawning position to the closest spawning position from the player
        strike_bool = true;
        foxAnimator.speed = 1f;

    }
    void circaStrike()
    {
        predator.transform.position = findClosestSpawningPosition(player.transform.position);
        // Set the predator's spawning position to the closest spawning position from the player
        circaStrike_bool = true;
        if (!tutorialMode) dataHandler.StartRecordingPredatorPosition();
        foxAnimator.speed = 1f;

    }

    public void setAttack(float attackingProb)
    {
        float attackingTime = Random.Range(0.25f, 1f);
        float encounterTime = Random.Range(2.5f, 3f);
        float rand = Random.Range(0.0f, 1.0f);

        if (rand < attackingProb)
        {
            Debug.Log("Attack Set: " + attackingTime);
            Invoke("strike", attackingTime + encounterTime);
            Invoke("circaStrike", encounterTime);

        }
    }

    void OnDisable()
    {
        fox.SetActive(false);
        CancelInvoke("strike");
        CancelInvoke("circaStrike");
        if (!tutorialMode) dataHandler.StopRecordingPredatorPosition();
        circaStrike_bool = false;
        strike_bool = false;
        predator.transform.position = spawningPositionsPredator[0];
    }

    Vector3 findClosestSpawningPosition(Vector3 playerPosition)
    {
        Vector3 closestPosition = Vector3.zero;
        float closestDistance = float.MaxValue;

        // Iterate through all spawning positions to find the closest one
        foreach (Vector3 spawningPosition in spawningPositionsPredator)
        {
            float distance = Vector3.Distance(playerPosition, spawningPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = spawningPosition;
            }
        }

        return closestPosition;
    }

}
