using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;

public class LandingPageButtons : MonoBehaviour
{
    // Constants for PlayerPrefs keys
    private const string UserIdKey = "userID";
    private const string CheckPointKey = "CheckPoint";
    private const string GameStateKey = "GameState";

    public InputField participantID;

public void LoginButton()
{
    PlayerPrefs.SetString(GameStateKey, "Login");
    string username = participantID.text?.Trim();

    if (string.IsNullOrEmpty(username))
    {
        Debug.LogWarning("Login failed: Username is empty.");
        return;
    }    
    if (username != null)
    {
        // Replace any non-alphanumeric character with an empty string
        username = Regex.Replace(username, "[^a-zA-Z0-9]", "");
    }

    string existingUsername = PlayerPrefs.GetString(UserIdKey, null);

    if (!string.IsNullOrEmpty(existingUsername))
    {
        if (existingUsername.Equals(username))
        {
            LoadFromOldUser();
        }
        else if(username.IndexOf("skipTutorial", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            SkipTutorial(username);
        }
        else
        {
            GenerateNewUser(username);
        }
    }
    else if(username.IndexOf("skipTutorial", StringComparison.OrdinalIgnoreCase) >= 0)
    {
        SkipTutorial(username);
    }
    else
    {
        GenerateNewUser(username);
    }

    PlayerPrefs.Save();
}


public void ResetButton()
{
    PlayerPrefs.SetString(GameStateKey, "Login");
    string username = participantID.text?.Trim();

    if (string.IsNullOrEmpty(username))
    {
        Debug.LogWarning("Login failed: Username is empty.");
        return;
    } 

        string existingUsername = PlayerPrefs.GetString(UserIdKey, null);

    if (!string.IsNullOrEmpty(existingUsername))
    {
        if (existingUsername.Equals(username))
        {
            LoadFromOldUser();
        }
        else if(username.IndexOf("skipTutorial", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            SkipTutorial(username);
        }
        else
        {
            GenerateReset1(username);
        }
    }
    else if(username.IndexOf("skipTutorial", StringComparison.OrdinalIgnoreCase) >= 0)
    {
        SkipTutorial(username);
    }
    else
    {
        GenerateReset1(username);
    }

    PlayerPrefs.Save(); 
}
private void SkipTutorial(string username)
{
    Debug.Log("Tutorial skipped.");
    PlayerPrefs.DeleteAll();
    PlayerPrefs.SetString(UserIdKey, username);
    Debug.Log($"New user generated: {username}");
    SceneManager.LoadScene("EffortCalibrator");
    // Implement logic to skip the tutorial and go to the main game
}


    // Generate a new user and go to the tutorial for this user
    void GenerateNewUser(string username)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(UserIdKey, username);
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        Debug.Log($"New user generated: {username}");
        SceneManager.LoadScene("EffortCalibrator");
    }

    void GenerateReset1(string username)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(UserIdKey, username);

        // Extract the trialNum from the username after "RESET"
        int trialNum = 0; // Default value in case of error
        if (!string.IsNullOrEmpty(username))
        {
            // Extract the numeric portion following "RESET"
            string trialNumString = System.Text.RegularExpressions.Regex.Match(username, @"RESET(\d+)").Groups[1].Value;
            if (!string.IsNullOrEmpty(trialNumString))
            {
                trialNum = int.Parse(trialNumString);
            }
        }

        Debug.Log("Extracted trialNum: " + trialNum);

        // Set the trialNum in PlayerPrefs
        PlayerPrefs.SetInt("trialNum", trialNum);

        // Set other PlayerPrefs values
        PlayerPrefs.SetString("ConditionFile", "condition_0");
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        Debug.Log($"New user generated: {username}");
        SceneManager.LoadScene("EffortCalibrator");
    }
    // Load from old user wherever they left off
    void LoadFromOldUser()
    {
        string oldCheckPoint = PlayerPrefs.GetString(CheckPointKey);
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        // Dictionary mapping checkpoints to scene names
        var sceneMap = new Dictionary<string, string>()
        {
            { "Tutorial", "EffortCalibrator" },
            { "MainGame", "MainGame" },
            { "Survey", "SurveyScene" },
            { "AcornGame", "AcornGame" }
        };

        // Try to load the scene based on the checkpoint
        if (sceneMap.TryGetValue(oldCheckPoint, out string sceneName))
        {
            Debug.Log($"Loading scene: {sceneName} for user.");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Error: Checkpoint '{oldCheckPoint}' not found. Unable to load scene.");
        }
    }
}

