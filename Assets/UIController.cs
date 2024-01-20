using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider energySlider;
    public float energyDecreaseRate = 0.1f;

    public void DecreaseEnergy()
    {
        // Decrease energy
        energySlider.value -= energyDecreaseRate;

        // Clamp the energy value to ensure it doesn't go below 0
        energySlider.value = Mathf.Max(energySlider.value, 0f);
    }
    public void SetEnergy(float energyValue)
    {
        // Set energy
        energySlider.value =energyValue;
    }

}
