using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool carrying;
    public bool acorn_carrying;

    public GameObject player;
    public GameObject task;
    public string playerState;
    public string cookieState;
    public float escapeTime;
    public float captureTime;
    public CookieChoice cookieChoice;
    public List<PositionHandler> acornsCollected;
    public float earnedReward;
    private float rewardPotential;
    public GameObject HeadsUpDisplay;
    public bool exitSafety;
    float rewardValue;
    void OnEnable()
    {
        // Reset player states
        playerState = string.Empty;
        cookieState = string.Empty;
        acornsCollected = new List<PositionHandler>();
        earnedReward = 0;
        rewardPotential = 0;
        escapeTime = -1f;
        captureTime = -1f;
        exitSafety = false;

        HeadsUpDisplay.GetComponent<UIController>().SetTotalScore(PlayerPrefs.GetFloat("TotalScore"));

        // Clear cookies and acorns attached to the player
        ClearPlayerItems();
    }

    void ClearPlayerItems()
    {
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("Cookie") || child.CompareTag("Acorn"))
            {
                Destroy(child.gameObject); // Remove both cookies and acorns
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Cookie")
        {
            other.gameObject.GetComponent<Collider>().enabled = false;
            float weight = other.gameObject.GetComponent<Cookie>().weight;
            rewardValue = other.gameObject.GetComponent<Cookie>().rewardValue;
            Vector3 cookiePosition = other.gameObject.transform.position;
            Debug.Log("cookie hit" + rewardValue);

            other.transform.parent = player.transform;

            if (weight >= 3)
            {
                cookieState = "heavy";
                // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                player.GetComponent<PlayerMovement>().cookieWeight = weight;
                player.GetComponent<PlayerMovement>().SetPressRate(weight);
                player.GetComponent<PlayerMovement>().SetStepSize(weight, cookiePosition);
                // player.GetComponent<PlayerMovement>().speed = 0f;  //speed multiplier higher is faster
                Debug.Log("speed:" + player.GetComponent<PlayerMovement>().speed);
                HeadsUpDisplay.GetComponent<UIController>().SetHUDReward((int)rewardValue);
                HeadsUpDisplay.GetComponent<UIController>().SetInstructText("While holding 'SDF', press 'A' to move back to safety");

            }
            else
            {
                cookieState = "light";
                // playerLayer = other.gameObject.GetComponent<Cookie>().layer;
                player.GetComponent<PlayerMovement>().cookieWeight = weight;
                player.GetComponent<PlayerMovement>().SetPressRate(weight);
                player.GetComponent<PlayerMovement>().SetStepSize(weight, player.transform.position);
                // player.GetComponent<PlayerMovement>().SetLightSpeed(); //speed multiplier higher is faster
                HeadsUpDisplay.GetComponent<UIController>().SetHUDReward((int)rewardValue);
                HeadsUpDisplay.GetComponent<UIController>().SetInstructText("While holding 'SDF', press 'A' to move back to safety");

            }

            player.GetComponent<PlayerMovement>().clickingPeriod = false;
            rewardPotential = rewardValue;
            cookieChoice = new CookieChoice(rewardValue, weight, cookiePosition.x, cookiePosition.z, Time.realtimeSinceStartup);
            task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EffortPeriod);
            carrying = true;
        }

        else if (other.gameObject.tag == "Safety")
        {
            if (exitSafety && task.GetComponent<LimaTask>().trialEndable && (carrying | acorn_carrying | task.GetComponent<LimaTask>().predator.GetComponent<PredatorControls>().circaStrike_bool))
            {
                foreach (Transform child in player.transform)
                {
                    if (child.CompareTag("Cookie")) Destroy(child.gameObject);
                    else if (child.CompareTag("Acorn")) Destroy(child.gameObject);
                }
                playerState = "escaped";
                earnedReward = rewardPotential;
                Debug.Log("earn reward");
                carrying = false;
                acorn_carrying = false;
                escapeTime= Time.realtimeSinceStartup;
                player.GetComponent<PlayerMovement>().stopRunning(); //stop movement
                // task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EndingPeriod);
            }
        }

        else if (other.gameObject.tag == "Predator")
        {
            if (exitSafety && task.GetComponent<LimaTask>().trialEndable && playerState !="escaped")
            {
                foreach (Transform child in player.transform)
                {
                    if (child.CompareTag("Cookie")) Destroy(child.gameObject);
                    else if (child.CompareTag("Acorn")) Destroy(child.gameObject);
                }
                playerState = "captured";
                earnedReward = -5f;
                carrying = false;
                acorn_carrying = false;
                captureTime = Time.realtimeSinceStartup;
                task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EndingPeriod);
            }
            else
            {
                task.GetComponent<LimaTask>().HandleGameState(LimaTask.GameState.EndingPeriod);
            }
        }

        else if (other.gameObject.tag == "Acorn")
        {
            other.gameObject.GetComponent<Collider>().enabled = false;
            logAcornPosition(other.gameObject);
            Debug.Log("Acorn Hit");
            HeadsUpDisplay.GetComponent<UIController>().SetHUDReward(2);
            rewardPotential += 2;
            foreach (Transform child in other.gameObject.transform)
            {
                // Set each child to active
                child.gameObject.SetActive(true); // Set to false if you want to deactivate
            }

            other.transform.parent = player.transform;
            acorn_carrying = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Safety")
        {
            exitSafety = true;
        }
    }


    void logAcornPosition(GameObject gameObject)
    {
        PositionHandler acornPosition = new PositionHandler(gameObject.transform.position.x, gameObject.transform.position.z, Time.realtimeSinceStartup);
        acornsCollected.Add(acornPosition);
    }
}

