using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class probability : MonoBehaviour
{
    public Material low;
    public Material med;
    public Material high;

    public void setProbabilityLow()
    {
        this.GetComponent<MeshRenderer>().material = low;
    }

    public void setProbabilityMed()
    {
    
        this.GetComponent<MeshRenderer>().material = med;
    }

    public void setProbabilityHigh()
    {
    
        this.GetComponent<MeshRenderer>().material = high;
    }
}
