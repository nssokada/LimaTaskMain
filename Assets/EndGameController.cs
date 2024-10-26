using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EndGameController : MonoBehaviour
{
    string completionPath;
    string FailReason;
    public TMP_Text instruct;

   
    // Start is called before the first frame update
    void Start()
    {
        completionPath = PlayerPrefs.GetString("CompletionPath");
        FailReason = PlayerPrefs.GetString("FailReason");
        instruct.text = FailReason;
    }

    public void redirectButton()
    {
            Application.OpenURL(completionPath);
    }


}
