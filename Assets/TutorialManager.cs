using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public void learnButton()
    {
        // SceneManager.loadScene()
    }
    public void calibrateButton()
    {
        SceneManager.LoadScene("EffortCalibrator");
    }
    public void playButton()
    {
        SceneManager.LoadScene("GrassTest");
    }
}
