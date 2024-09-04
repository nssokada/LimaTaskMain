using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LandingPageButtons : MonoBehaviour
{
    // Constants for PlayerPrefs keys
    private const string UserIdKey = "userID";
    private const string CheckPointKey = "CheckPoint";
    private const string GameStateKey = "GameState";

    public TMP_InputField participantID;

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
            else
            {
                GenerateNewUser(username);
            }
        }
        else
        {
            GenerateNewUser(username);
        }
    }

    // Generate a new user and go to the tutorial for this user
    void GenerateNewUser(string username)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(UserIdKey, username);
        Debug.Log($"New user generated: {username}");
        SceneManager.LoadScene("Tutorial");
    }

    // Load from old user wherever they left off
    void LoadFromOldUser()
    {
        string oldCheckPoint = PlayerPrefs.GetString(CheckPointKey);

        // Dictionary mapping checkpoints to scene names
        var sceneMap = new Dictionary<string, string>()
        {
            { "Tutorial", "Tutorial" },
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

