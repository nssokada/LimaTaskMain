using System.Resources;
using System.Runtime.Versioning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CSVReader
{

    public List<LimaTrial> trials;

    //trial reading components
    //PREDATOR
    private float attackingProb;

    //REWARDS
    private int cookie1PosX;
    private int cookie1PosY;
    private float cookie1Weight;
    private int cookie1RewardValue;
    private int cookie2PosX;
    private int cookie2PosY;
    private float cookie2Weight;
    private int cookie2RewardValue;
    public int cookie3PosX;
    public int cookie3PosY;
    public float cookie3Weight;
    public int cookie3RewardValue;

    private LimaTrial trial;
    private TextAsset text2read;
    private TextAsset csvFile;


    public TextAsset CSVloader(string filepath)
    {
            // Assuming the file is in a "Resources/Conditions" directory
        string resourcePath = "Conditions/" + filepath;

        // Load the TextAsset from Resources
        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);


        
        return csvFile;
    }
    

 public List<LimaTrial> ReadTrialCSV(string version)
{
    Debug.Log("version is:"+version);
    text2read = CSVloader(version);
    String[] data = text2read.text.Split(new String[] {",","\n"}, StringSplitOptions.None);
    int tablesize = data.Length / 14 - 1; // Adjusted from 10 to 14 to accommodate cookie3 data
    trials = new List<LimaTrial>();

    for(int i = 0; i < tablesize; i++)
    {
        float.TryParse(data[14*(i+1)+1], out attackingProb);
        int.TryParse(data[14*(i+1)+2], out cookie1PosX);
        int.TryParse(data[14*(i+1)+3], out cookie1PosY);
        float.TryParse(data[14*(i+1)+4], out cookie1Weight);
        int.TryParse(data[14*(i+1)+5], out cookie1RewardValue);
        int.TryParse(data[14*(i+1)+6], out cookie2PosX);
        int.TryParse(data[14*(i+1)+7], out cookie2PosY);
        float.TryParse(data[14*(i+1)+8], out cookie2Weight);
        int.TryParse(data[14*(i+1)+9], out cookie2RewardValue);
        int.TryParse(data[14*(i+1)+10], out cookie3PosX);
        int.TryParse(data[14*(i+1)+11], out cookie3PosY);
        float.TryParse(data[14*(i+1)+12], out cookie3Weight);
        int.TryParse(data[14*(i+1)+13], out cookie3RewardValue);

        trial = new LimaTrial(attackingProb, cookie1PosX, cookie1PosY, cookie1Weight, cookie1RewardValue,
                              cookie2PosX, cookie2PosY, cookie2Weight, cookie2RewardValue,
                              cookie3PosX, cookie3PosY, cookie3Weight, cookie3RewardValue);
        trials.Add(trial);
    }
    return trials;
}

}
