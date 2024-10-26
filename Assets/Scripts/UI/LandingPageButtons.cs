using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

    // Load from old user wherever they left off
    void LoadFromOldUser()
    {
        string oldCheckPoint = PlayerPrefs.GetString(CheckPointKey);

        // Dictionary mapping checkpoints to scene names
        var sceneMap = new Dictionary<string, string>()
        {
            { "Tutorial", "EffortCalibrator" },
            { "MainGame", "MainGame" },
            { "Survey", "Survey" },
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

