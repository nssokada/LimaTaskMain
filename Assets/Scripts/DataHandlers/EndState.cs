using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class EndState
{
    public string endState;
    public float time;

    // Getter and Setter for endState
    public string EndStateProperty
    {
        get { return endState; }
        set { endState = value; }
    }

    // Getter and Setter for time
    public float Time
    {
        get { return time; }
        set { time = value; }
    }

    // Constructor
    public EndState(string endState, float time)
    {
        this.endState = endState;
        this.time = time;
    }
}
