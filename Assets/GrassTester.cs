using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTester : MonoBehaviour
{

    public MeshRenderer meshR;
    public float wind;
    // Start is called before the first frame update
    void Start()
    {
        wind = 0;
    }



    // Update is called once per frame
    void Update()
    {
    //   meshR.material.SetFloat("_WindForce", wind);  
        meshR.material.SetFloat("_WindMovement", wind);  

    }
}
