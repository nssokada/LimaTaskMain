using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SubjectiveReport
{
    public int trialNumber;
    public float attackingProb;
    public CookieChoice trialCookie;
    public int response; //1-7
    public int questionType; //anxious vs confident
    
}
