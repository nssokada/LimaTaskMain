using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class LimaTrial
{
    //PREDATOR
    public float attackingProb;

    //REWARDS
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

    //DATA TO RECORD
    


    public LimaTrial(float attackingProb,int cookie1PosX,int cookie1PosY,float cookie1Weight,int cookie1RewardValue,int cookie2PosX,int cookie2PosY,float cookie2Weight,int cookie2RewardValue, int cookie3PosX,int cookie3PosY,float cookie3Weight,int cookie3RewardValue)
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
    }
}
