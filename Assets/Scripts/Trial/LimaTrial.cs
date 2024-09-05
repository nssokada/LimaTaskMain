using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LimaTrial
{
    public float attackingProb;
    public int cookie1PosX;
    public int cookie1PosY;
    public float cookie1Weight;
    public int cookie1RewardValue;
    public int cookie2PosX;
    public int cookie2PosY;
    public float cookie2Weight;
    public int cookie2RewardValue;
    public int cookie3PosX;
    public int cookie3PosY;
    public float cookie3Weight;
    public int cookie3RewardValue;
    public int type;

    public List<PositionHandler> playerPosition;
    public List<PositionHandler> predatorPosition;
    public List<PositionHandler> mouseTrackChoicePeriod;
    public List<PositionHandler> mouseTrackEffortPeriod;
    public List<float> effortRate;

    public float trialStartTime;
    public float trialEndTime;
    public string trialEndState;
    public float trialReward;
    public CookieChoice trialCookie;
    public List<PositionHandler> acornsSpawned;
    public List<PositionHandler> acornsCollected;

    // Constructor with parameters
    public LimaTrial(float attackingProb, int cookie1PosX, int cookie1PosY, float cookie1Weight, int cookie1RewardValue,
                     int cookie2PosX, int cookie2PosY, float cookie2Weight, int cookie2RewardValue,
                     int cookie3PosX, int cookie3PosY, float cookie3Weight, int cookie3RewardValue, int type)
    {
        this.attackingProb = attackingProb;
        this.cookie1PosX = cookie1PosX;
        this.cookie1PosY = cookie1PosY;
        this.cookie1Weight = cookie1Weight;
        this.cookie1RewardValue = cookie1RewardValue;
        this.cookie2PosX = cookie2PosX;
        this.cookie2PosY = cookie2PosY;
        this.cookie2Weight = cookie2Weight;
        this.cookie2RewardValue = cookie2RewardValue;
        this.cookie3PosX = cookie3PosX;
        this.cookie3PosY = cookie3PosY;
        this.cookie3Weight = cookie3Weight;
        this.cookie3RewardValue = cookie3RewardValue;
        this.type = type;

        // Initialize lists
        playerPosition = new List<PositionHandler>();
        predatorPosition = new List<PositionHandler>();
        mouseTrackChoicePeriod = new List<PositionHandler>();
        mouseTrackEffortPeriod = new List<PositionHandler>();
        effortRate = new List<float>();

        // Initialize other fields
        trialStartTime = float.NaN;
        trialEndTime = float.NaN;
        trialEndState = string.Empty; // Initialize to an empty string
        trialReward = float.NaN;
        trialCookie = new CookieChoice();
    }

    // Default constructor
    public LimaTrial()
    {
        attackingProb = float.NaN;
        cookie1PosX = -1;  // Default to -1 to indicate uninitialized
        cookie1PosY = -1;
        cookie1Weight = float.NaN;
        cookie1RewardValue = -1;
        cookie2PosX = -1;
        cookie2PosY = -1;
        cookie2Weight = float.NaN;
        cookie2RewardValue = -1;
        cookie3PosX = -1;
        cookie3PosY = -1;
        cookie3Weight = float.NaN;
        cookie3RewardValue = -1;
        type = -1;

        // Initialize lists
        playerPosition = new List<PositionHandler>();
        predatorPosition = new List<PositionHandler>();
        mouseTrackChoicePeriod = new List<PositionHandler>();
        mouseTrackEffortPeriod = new List<PositionHandler>();
        effortRate = new List<float>();

        // Initialize other fields
        trialStartTime = float.NaN;
        trialEndTime = float.NaN;
        trialEndState = string.Empty;
        trialReward = float.NaN;
        trialCookie = new CookieChoice();
        acornsSpawned = new List<PositionHandler>();
        acornsCollected = new List<PositionHandler>();
    }
}
