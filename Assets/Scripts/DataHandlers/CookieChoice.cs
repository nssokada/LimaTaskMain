using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class CookieChoice
{
    public float? rewardValue;  // Nullable float
    public float? weight;       // Nullable float
    public float? xPos;         // Nullable float
    public float? yPos;         // Nullable float
    public float? time;         // Nullable float
    // Constructor
    public CookieChoice(float rewardValue, float weight, float xPos, float yPos, float time)
    {
        this.rewardValue = rewardValue;
        this.weight = weight;
        this.xPos = xPos;
        this.yPos = yPos;
        this.time = time;
    }
    public CookieChoice()
    {
        // Initialize with null values
        this.rewardValue = null;
        this.weight = null;
        this.xPos = null;
        this.yPos = null;
        this.time = null;
    }
}
