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

    public bool circaStrike;
    public float speed;
    public Animator foxAnimator;


    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if(circaStrike)
        {
            fox.SetActive(true);
            predator.transform.position = Vector3.MoveTowards(predator.transform.position, player.transform.position, speed*Time.deltaTime);
            predator.transform.LookAt(player.transform.position, Vector3.up);
        }
        else
        {
            foxAnimator.speed = 0f;
        }
    }

    void strike()
    {
        // Set the predator's spawning position to the closest spawning position from the player
        predator.transform.position = findClosestSpawningPosition(player.transform.position);
        dataHandler.StartRecordingPredatorPosition();
        circaStrike=true;
        foxAnimator.speed = 1f;

    }

    public void setAttack(float attackingProb)
    {
        float attackingTime = Random.Range(1.0f, 3.0f);
        float rand = Random.Range(0.0f, 1.0f);

        if (rand<attackingProb)
        {
            Debug.Log("Attack Set");
            Invoke("strike", attackingTime);
        }
    }

    void OnDisable() 
    {
        fox.SetActive(false);
        CancelInvoke("strike");
        dataHandler.StopRecordingPredatorPosition();
        circaStrike=false; 
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
