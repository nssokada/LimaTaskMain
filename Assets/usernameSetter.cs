using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class usernameSetter : MonoBehaviour
{
    public TMP_Text username;
    // Start is called before the first frame update
    void Start()
    {
        username.text = PlayerPrefs.GetString("userID");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
