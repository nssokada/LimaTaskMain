using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider energySlider;
    public float energyDecreaseRate = 0.1f;

    public Slider timeSlider;
    public float timeDecreaseRate = 0.01f;
    public GameObject windSignal;

    public Material high;
    public Material medium;
    public Material low;


    public void DecreaseEnergy(float energyValue)
    {
        float energy =  energySlider.value;
        float decrease = energy - energyValue;
        energySlider.value = decrease;
    }
    public void SetEnergy(float energyValue)
    {
        // Set energy
        energySlider.value =energyValue;
    }
    public void DecreaseTime()
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

    public void setWind(Vector3 drift, float resistance)
    {
        // change sprite color
        if (resistance == 5)
        {
            windSignal.GetComponent<MeshRenderer>().material = high;
        }
        else if (resistance == 3)
        {
            windSignal.GetComponent<MeshRenderer>().material = medium;
        }
        else
        {
            windSignal.GetComponent<MeshRenderer>().material = low;
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

