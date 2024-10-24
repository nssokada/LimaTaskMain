using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrialDataHandler : MonoBehaviour
{
    public List<PositionHandler> playerPosition;
    public List<PositionHandler> predatorPosition;
    public List<PositionHandler> mouseTrackChoicePeriod;
    public List<PositionHandler> mouseTrackEffortPeriod;
    public List<float> effortRate;
    public GameObject player;
    public GameObject predator;


    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0)) 
    //     {
    //         recordMouseStartPosition();
    //     }
    // }
   // Update is called once per frame

    #region Public Access Methods

    //This method will instantiate a list of objects that will be used to store continous data throughout a given trial.
    public void instantiateTrialDataHandlers()
    {
        playerPosition = new List<PositionHandler>();
        predatorPosition = new List<PositionHandler>();
        mouseTrackChoicePeriod = new List<PositionHandler>();
        mouseTrackEffortPeriod = new List<PositionHandler>();
        effortRate  = new List<float>();
    }

    // Method to clear all the lists
    public void ClearAllTrialDataHandlers()
    {
        // playerPosition.Clear();
        // predatorPosition.Clear();
        // mouseTrackChoicePeriod.Clear();
        // mouseTrackEffortPeriod.Clear();
        // effortRate.Clear();
        // Debug.Log("All position lists have been cleared.");

         playerPosition = new List<PositionHandler>();
        predatorPosition = new List<PositionHandler>();
        mouseTrackChoicePeriod = new List<PositionHandler>();
        mouseTrackEffortPeriod = new List<PositionHandler>();
        effortRate  = new List<float>();
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

    }

    #endregion

    #region Position Handlers
    //public methods

    //player
        // Generalized Start/Stop Recording
        public void StartRecordingPosition(GameObject entity, Action recordAction)
        {
            InvokeRepeating(recordAction.Method.Name, 0f, 0.03f);
        }

        public void StopRecordingPosition(Action recordAction)
        {
            CancelInvoke(recordAction.Method.Name);
        }

        // Start Recording for Player and Predator
        public void StartRecordingPlayerPosition()
        {
            StartRecordingPosition(player, RecordPlayerPosition);
        }

        public void StartRecordingPredatorPosition()
        {
            StartRecordingPosition(predator, RecordPredatorPosition);
        }

        public void StopRecordingPlayerPosition()
        {
            StopRecordingPosition(RecordPlayerPosition);
        }

        public void StopRecordingPredatorPosition()
        {
            StopRecordingPosition(RecordPredatorPosition);
        }

        // Private methods
        private void RecordPlayerPosition()
        {
            RecordPosition(player, playerPosition);
        }

        private void RecordPredatorPosition()
        {
            RecordPosition(predator, predatorPosition);
        }

        private void RecordPosition(GameObject entity, List<PositionHandler> positionList)
        {
            Vector3 position = entity.transform.position;
            PositionHandler pos = new PositionHandler(position.x, position.z, Time.realtimeSinceStartup);
            positionList.Add(pos);
        }



    #endregion

    #region Effort Tracking

    public void recordEffort()
    {
        RecordEffortInput();
    }

    void RecordEffortInput()
    {
        effortRate.Add(Time.realtimeSinceStartup);
    }
    #endregion

}
