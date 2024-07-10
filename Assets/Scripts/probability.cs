using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class probability : MonoBehaviour
{
    public Material low;
    public Material med;
    public Material high;

    public Renderer mesh;

    public void setProbabilityLow()
    {
        Debug.Log("change material");
        mesh.material = low;
    }

    public void setProbabilityMed()
    {
        Debug.Log("change material");
        mesh.material = med;
    }

    public void setProbabilityHigh()
    {
        Debug.Log("change material");
        mesh.material = high;
    }
}
