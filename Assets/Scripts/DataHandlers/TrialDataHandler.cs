using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialDataHandler : MonoBehaviour
{
    public List<PositionHandler> playerPosition;
    public List<PositionHandler> predatorPosition;
    public List<PositionHandler> mouseTrackChoicePeriod;
    public List<PositionHandler> mouseTrackEffortPeriod;
    public GameObject player;
    public GameObject predator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   // Update is called once per frame

    #region Public Access Methods

    //This method will instantiate a list of objects that will be used to store continous data throughout a given trial.
    public void instantiateTrialDataHandlers()
    {
        playerPosition = new List<PositionHandler>();
        predatorPosition = new List<PositionHandler>();
        mouseTrackChoicePeriod = new List<PositionHandler>();
        mouseTrackEffortPeriod = new List<PositionHandler>();
    }

    // Method to clear all the lists
    public void ClearAllTrialDataHandlers()
    {
        playerPosition.Clear();
        predatorPosition.Clear();
        mouseTrackChoicePeriod.Clear();
        mouseTrackEffortPeriod.Clear();

        Debug.Log("All position lists have been cleared.");
    }



    #endregion

    #region Mouse Tracking
    //public methods

    public void recordMouseStartPosition()
    {
        RecordMousePosition(mouseTrackChoicePeriod);
    }

    public void startRecordContinuousMouse(string period)
    {
        string currentPeriod = period.ToLower(); // Track the current period for cancelling

        switch (currentPeriod)
        {
            case "choiceperiod":
                InvokeRepeating("RecordMousePositionChoicePeriod", 0f, 0.03f);
                break;

            case "effortperiod":
                InvokeRepeating("RecordMousePositionEffortPeriod", 0f, 0.03f);
                break;

            default:
                Debug.LogWarning("Invalid period specified: " + period);
                break;
        }
    }


    // Method to stop the recording process
    public void stopRecordContinuousMouse(string period)
    {
        string currentPeriod = period.ToLower(); // Track the current period for cancelling
        
        switch (currentPeriod)
        {
            case "choiceperiod":
                CancelInvoke("RecordMousePositionChoicePeriod");
                break;

            case "effortperiod":
                CancelInvoke("RecordMousePositionEffortPeriod");
                break;

            default:
                Debug.LogWarning("No recording to stop or invalid period: " + currentPeriod);
                break;
        }
    }

 
    //private method

     // Method to record mouse position during choice period
    private void RecordMousePositionChoicePeriod()
    {
        RecordMousePosition(mouseTrackChoicePeriod);
    }

    // Method to record mouse position during effort period
    private void RecordMousePositionEffortPeriod()
    {
        RecordMousePosition(mouseTrackEffortPeriod);
    }
    void RecordMousePosition(List<PositionHandler> mouseTrackerList)
    {
        // Get the current mouse position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 21.54f; //distance of camera from the ground 

        // Convert the screen position to world position
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Create a Mouse Position handler
        PositionHandler mousePosHandler = new PositionHandler(mousePosition.x, mousePosition.z,Time.realtimeSinceStartup); // need to do Z since the game is XZ oriented

        // Add the position to the list
        mouseTrackerList.Add(mousePosHandler);

        Debug.Log(mousePosition);
    }

    #endregion

    #region Position Handlers
    //public methods

    //player
    public void StartRecordingPlayerPosition()
    {
        InvokeRepeating("RecordPlayerPosition", 0f, 0.03f);
    }

    public void StopRecordingPlayerPosition()
    {
        CancelInvoke("RecordPlayerPosition");
    }

    //predator
    public void StartRecordingPredatorPosition()
    {
        InvokeRepeating("RecordPredatorPosition", 0f, 0.03f);
    }

    public void StopRecordingPredatorPosition()
    {
        CancelInvoke("RecordPredatorPosition");
    }



    //private methods
    void RecordPlayerPostion()
    {
        Vector3 position = player.transform.position;
        PositionHandler playerPos = new PositionHandler(position.x, position.z,Time.realtimeSinceStartup);
        playerPosition.Add(playerPos);
    }

     void RecordPredatorPostion()
    {
        Vector3 position = predator.transform.position;
        PositionHandler predatorPos = new PositionHandler(position.x, position.z,Time.realtimeSinceStartup);
        predatorPosition.Add(predatorPos);
    }


    #endregion


}
