using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class CookieChoice
{
    public float rewardValue; 
    public float weight;     
    public float xPos;       
    public float yPos;       
    public float time;
            
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
        this.rewardValue = 0f;
        this.weight = 0f;
        this.xPos = 0f;
        this.yPos = 0f;
        this.time = 0f;
    }
}
