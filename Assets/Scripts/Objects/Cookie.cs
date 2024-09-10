using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie: MonoBehaviour
{
    public float weight; //heavy =4.54
    public int rewardValue; //heavy =4.54
    public Material chocolate;
    public Material plain;
    public Material chocochip;
    public Material strawbezzy;

    public void setCookieSize()
    {
        if(weight==3f)
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
         // Access the MeshRenderer component
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        // Get a copy of the materials array
        Material[] materials = renderer.materials;

        // Determine materials based on conditions
        Material material0 = plain;
        Material material1 = chocochip;

        if (rewardValue > 10 && weight >= 3f)
        {
            material0 = chocolate;
            material1 = chocochip;
        }
        else if (rewardValue > 10 && weight >= 2f)
        {
            material0 = plain;
            material1 = strawbezzy;
        }

        // Assign materials only if there are changes
        if (materials[0] != material0 || materials[1] != material1)
        {
            materials[0] = material0;
            materials[1] = material1;
            // Reassign the modified materials array back to the MeshRenderer
            renderer.materials = materials;
        }
    }

}
