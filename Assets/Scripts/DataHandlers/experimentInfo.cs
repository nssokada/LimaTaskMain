using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class experimentInfo
{
    public string condition;
    public string participantID;
    public string experimentDate;

    public float effortPressLatency;
    public string datapath;
}