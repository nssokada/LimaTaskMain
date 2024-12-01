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
        speed=8f;
        // if (player.GetComponent<PlayerMovement>().acornPeriod)
        // {
        //     speed = 8f;
        // }
        // else
        // {
        //     speed = CalculatePredatorSpeed();
        // }
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
            predator.transform.position = Vector3.MoveTowards(predator.transform.position, player.transform.position, speed * Time.deltaTime);

            if (strike_bool)
            {
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
        speed =8f;

    }
    void circaStrike()
    {
        predator.transform.position = findClosestSpawningPosition(player.transform.position);
        speed =0.5f;
        // Set the predator's spawning position to the closest spawning position from the player
        circaStrike_bool = true;
        if (!tutorialMode) dataHandler.StartRecordingPredatorPosition();
        foxAnimator.speed = 1f;

    }

    public void setAttack(float attackingProb, float attackingTime, int isAttackTrial)
    {
        float encounterTime = 0f;
        float distance = Vector3.Distance(player.transform.position, new Vector3(0f, 0f, 0f));

        // Set encounterTime based on distance
        if (distance >= 7)
        {
            encounterTime = 5f;
        }
        else if (distance >= 5)
        {
            encounterTime = 3.5f;
        }
        else
        {
            encounterTime = 2.5f;
        }

        if (isAttackTrial == 1)
        {
            // Parameters for Gaussian distribution
            float mean = 2 * encounterTime;
            float stdDev = (mean - encounterTime) / 3f;

            // Generate Gaussian-distributed value using Box-Muller Transform
            float u1 = Mathf.Clamp01(attackingTime); // Use the provided attackingTime for randomness
            float u2 = UnityEngine.Random.value;    // Generate another random value
            float z0 = Mathf.Sqrt(-2.0f * Mathf.Log(Mathf.Max(u1, 1e-7f))) * Mathf.Cos(2.0f * Mathf.PI * u2);
            float transformedTime = mean + stdDev * z0;

            // Check for NaN and fallback to a safe value
            if (float.IsNaN(transformedTime) || transformedTime <= 0)
            {
                Debug.LogWarning("Generated a NaN or invalid attacking time. Fallback to default.");
                transformedTime = 2 * encounterTime; // Safe fallback value
            }

            // Debug and invoke attack events
            Debug.Log($"Attack Set: {transformedTime} seconds after start.");
            Invoke("strike", transformedTime);
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
