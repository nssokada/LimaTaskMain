using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] options; // For multiple-choice questions
    public int correctAnswerIndex; 
}

