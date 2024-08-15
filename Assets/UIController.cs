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
    public GameObject windSignal;

    public Material high;
    public Material medium;
    public Material low;
    public MeshRenderer grass;
    public TMP_Text energyText;

    public TMP_Text rewardTextHUD;




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
        rewardTextHUD.text = "+"+rewardValue.ToString();
    }



    public void setWind(Vector3 drift, float resistance)
    {
        // change sprite color
        if (resistance == 5)
        {
            windSignal.GetComponent<MeshRenderer>().material = high;
            grass.material.SetFloat("_WindMovement", 0.8f);

        }
        else if (resistance == 3)
        {
            windSignal.GetComponent<MeshRenderer>().material = medium;
            grass.material.SetFloat("_WindMovement", 0.45f);

        }
        else
        {
            windSignal.GetComponent<MeshRenderer>().material = low;
            grass.material.SetFloat("_WindMovement", 0.1f);
        }

        Quaternion targetRotation = Quaternion.LookRotation(drift.normalized, Vector3.up);

        // Reset x rotation to zero
        Vector3 euler = targetRotation.eulerAngles;
        euler.x = 0;
        targetRotation = Quaternion.Euler(euler);

        // Apply rotation
        windSignal.transform.rotation = targetRotation;
    }
}

