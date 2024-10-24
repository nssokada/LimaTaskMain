using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CSVReader 
{
    public List<LimaTrial> trials;

    public TextAsset CSVLoader(string filepath)
    {
        string resourcePath = "Conditions/" + filepath;
        return Resources.Load<TextAsset>(resourcePath);
    }

    public List<LimaTrial> ReadTrialCSV(string version)
    {
        Debug.Log("Version is: " + version);
        TextAsset text2read = CSVLoader(version);
        string[] lines = text2read.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Assuming the first row is the header
        string[] headers = lines[0].Split(',');
        trials = new List<LimaTrial>();

        // Get the fields of the LimaTrial class
        Type trialType = typeof(LimaTrial);
        FieldInfo[] trialFields = trialType.GetFields();

        for (int i = 1; i < lines.Length; i++) // Start at 1 to skip the header
        {
            string[] data = lines[i].Split(',');
            LimaTrial trial = new LimaTrial();

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = data[j].Trim();

                FieldInfo field = Array.Find(trialFields, f => f.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

                if (field != null && !string.IsNullOrEmpty(value))
                {
                    try
                    {
                        // Convert the value to the appropriate type and set the field dynamically
                        object convertedValue = Convert.ChangeType(value, field.FieldType);
                        field.SetValue(trial, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to convert value '{value}' to {field.FieldType} for field {field.Name}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Field {header} not found on LimaTrial or value is empty.");
                }
            }

            trials.Add(trial);
        }

        return trials;
    }
}
