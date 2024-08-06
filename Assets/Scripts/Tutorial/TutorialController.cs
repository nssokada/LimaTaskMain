using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject task;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void buttonRN()
    {
        task.SetActive(true);
        startScreen.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
