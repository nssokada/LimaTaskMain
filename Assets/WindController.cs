using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public GameObject windLayer1;
    public GameObject windLayer2;
    public GameObject windLayer3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void activateWindLayer(string layer)
    {
        if(layer=="close")
        {
            windLayer2.SetActive(true);
            windLayer3.SetActive(true);
        }
        else
        {
            windLayer1.SetActive(true);
            windLayer2.SetActive(true);
            windLayer3.SetActive(true);
        }
    }

    public void deactivateWindLayer()
    {
        windLayer1.SetActive(false);
        windLayer2.SetActive(false);
        windLayer3.SetActive(false);
    }

}
