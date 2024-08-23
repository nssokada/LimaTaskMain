using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie: MonoBehaviour
{
    public float weight; //heavy =4.54
    public int rewardValue; //heavy =4.54
    public Material chocolate;
    public Material plain;



    public void setCookieSize()
    {
        if(weight>1)
        {
            this.transform.localScale = new Vector3 (85.0f, 85.0f, 85.0f);
        }
        else
        {
            this.transform.localScale = new Vector3 (40.0f, 40.0f, 40.0f);
        }
    }

    public void setCookieColor()
    {
         if(rewardValue>10)
        {
            this.GetComponent<MeshRenderer>().material = chocolate;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = plain;
        }
    }

}
