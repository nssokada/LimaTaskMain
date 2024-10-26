using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider energySlider;
    public float energyDecreaseRate = 0.1f;

    public Slider timeSlider;

    public Material high;
    public Material medium;
    public Material low;
    public MeshRenderer grass;
    public TMP_Text energyText;
    public TMP_Text instructText;

    public TMP_Text rewardTextHUD;
    public TMP_Text totalScoreTextHUD;



    void OnEnable()
    {
        SetEnergy(0f);
    }
    void OnDisable()
    {
        SetHUDReward(0);

    }
    public void DecreaseEnergy(float energyValue)
    {
        float energy =  energySlider.value;
        float decrease = energy - energyValue;
        energySlider.value = decrease;
    }
    
    public void IncreaseEnergy(float energyValue)
    {
        float energy =  energySlider.value;
        float increase = energy + energyValue;
        energySlider.value = increase;
    }
    
    public float GetEnergy()
    {
        float energy =  energySlider.value;
        return energy;
    }
    public void SetEnergyText(int count)
    {
        energyText.text  = count.ToString();
    }

    public void SetInstructText(string instruct)
    {
        instructText.text  = instruct;
    }

    public void SetEnergy(float energyValue)
    {
        // Set energy
        energySlider.value =energyValue;
    }
    public void DecreaseTime(float timeDecreaseRate)
    {
        // Decrease Time
        timeSlider.value -= timeDecreaseRate;

        // Clamp the timer value to ensure it doesn't go below 0
        energySlider.value = Mathf.Max(energySlider.value, 0f);
    }
    public void SetTime(float timeValue)
    {
        // Set Time
        timeSlider.value =timeValue;
    }


    public void SetHUDReward(int rewardValue)
    {
        Debug.Log("Setting HUD reward");
        rewardTextHUD.text = "+"+rewardValue.ToString();
    }
    public void SetTotalScore(float totalScore)
    {
        totalScoreTextHUD.text = ((int)totalScore).ToString();
    }


}

