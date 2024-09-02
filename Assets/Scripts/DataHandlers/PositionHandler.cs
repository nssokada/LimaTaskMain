using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class PositionHandler
{
    public float x;
    public float y;
    public float time;

    // Getter and Setter for x
    public float X
    {
        get { return x; }
        set { x = value; }
    }

    // Getter and Setter for y
    public float Y
    {
        get { return y; }
        set { y = value; }
    }

    // Getter and Setter for time
    public float Time
    {
        get { return time; }
        set { time = value; }
    }

    // Constructor
    public PositionHandler(float x, float y, float time)
    {
        this.x = x;
        this.y = y;
        this.time = time;
    }
}
