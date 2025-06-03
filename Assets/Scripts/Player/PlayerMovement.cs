using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{


    //Public Objects
    public GameObject player;
    public PlayerManager playerManager;
    public GameObject HeadsUpDisplay;
    public TrialDataHandler dataHandler;
    public Camera mainCamera;
    public GameObject centerPt;


    //Public values
    public float speed;
    public int numPress;
    public float radius;
    public float resistance;
    public float rotationSpeed;
    private float minSpeed = 0f;
    private float baseSpeed = 4f;
    private float maxSpeed = 2f;
    public float cookieWeight;
    public float thresholdLow = 0.1f; // Example threshold for low latency
    public float smoothingFactor = 0.1f;
    public float currentPressRate = 0f;   // Current presses per second
    private float decayRate = 0.9f; // Speed decay rate when not maintaining effort

    public float requiredPressRate;
    public float requiredPressRate75;
    public float requiredPressRate50;
    public float requiredPressRate25;
    
    public float desiredSpeedMax;

    //Togglable Bools
    public bool effortPeriod;
    public bool clickingPeriod;
    public bool acornPeriod;

    //Latency Information
    private float MinPressLatency;
    private float MinPressCount;
    public float stepSize;
    public float stepSize_Heavy;
    public float stepSize_Light;
    public float lightSpeed;
    Vector3 targetPosition;

    //Private Objects
    PlayerInput playerInput;
    InputAction moveAction;
    Animator animator;

    //Private Values
    private bool isMoving = false;
    Vector3 movementDirection;
    float lastPressTime;
    float latency;
    float currentSpeed;

    // Internal tracking of button press times
    private List<float> pressTimes;

     // Smoothing factors
    [Range(0f, 1f)]
    public float pressRateSmoothing = 0.8f;   // Higher value = more smoothing
    public float speedTransitionSpeed = 5f;   // Higher value = faster speed transitions
    public int counter;
    public float distance;
    public float remainingPresses;

    void OnEnable()
    {
        pressTimes = new List<float>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        speed = baseSpeed;
        counter=0;
        // Fetch and log press latency from PlayerPrefs
        MinPressLatency = PlayerPrefs.GetFloat("PressLatency"); // Add default value to avoid null issues
        MinPressCount = PlayerPrefs.GetFloat("PressCount");
        Debug.Log($"Initialized with Press Latency: {MinPressLatency} and Press Count {MinPressCount}");
    }

    public void SetStepSize(float weight, Vector3 playerPos)
    {
        // Calculate distance from (0,0) using Pythagorean theorem
        distance = Vector3.Distance(player.transform.position, centerPt.transform.position);
        float stepCount = (MinPressCount*1.1f); 

        // // Initialize step size variables
        float stepSize_Heavy;
        float stepSize_Light;

        // Determine step sizes based on distance
        if (distance >= 7 && weight >= 3f) // Far cookie
        {
            //Step Size =  TimeToReturn / %MaxPressCount
            remainingPresses = stepCount;//(stepCount); // Heavy step size
        }
        else if (distance >= 5 && weight >= 3f) // Middle cookie
        {
            remainingPresses = stepCount*0.8f;//(stepCount*0.8f)); // Adjust for middle
        }
        else if(distance >= 2 && weight >= 3f) // Near cookie
        {
            remainingPresses = stepCount*0.6f;//(stepCount*0.6f)); // Adjust for near
        }
        else
        {
            remainingPresses = stepCount*0.2f; // Adjust for near
        }
    }


    public void SetLightSpeed()
    {
        speed = lightSpeed;
    }

   


    void OnDisable()
    {
        effortPeriod = false;
        acornPeriod = false;
        clickingPeriod = false;
        // Reset energy display when the player is disabled
        if (HeadsUpDisplay != null) HeadsUpDisplay.GetComponent<UIController>()?.SetEnergy(0f);
    }


    void Update()
    {

        if (clickingPeriod == true)
        {
            mouseMove();
        }
        if (effortPeriod == true)
        {

            mouseMove();
            effortUI();
            CalculatePressRate();
            AdjustSpeed();
        }
        else if (acornPeriod == true)
        {
            speed = 4f;
            mouseMove();
        }
    }



    #region Clicking To Select Reward
    // void OnMouseClick()
    // {
    //     if (!clickingPeriod) return;

    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         if (hit.transform.CompareTag("Cookie"))
    //         {
    //             Debug.Log("Reward hit");
    //             clickingPeriod = false;
    //             StartCoroutine(MoveObject(hit.transform.position));
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("No object hit.");
    //     }
    // }

    IEnumerator MoveObject(Vector3 newPosition)
    {
        float startTime = Time.time;
        float speed = 0.9f;
        Quaternion targetRotation = Quaternion.LookRotation(newPosition - transform.position);
        float journeyLength = Vector3.Distance(transform.position, newPosition);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

        while (!playerManager.carrying)
        {
            animator.SetBool("IsRunning", true);

            // Calculate the progress and move the player
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, newPosition, fractionOfJourney);

            yield return null;
        }

        // Stop running animation when done
        animator.SetBool("IsRunning", false);
        Debug.Log("Arrived at the target position.");
    }

    #endregion


    #region Player Movement
    void mouseMove()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            targetPosition = GetWorldPosition(mouseScreenPosition);

            setMovementDirection();
        }
    }

    void setMovementDirection()
    {
        // Calculate direction towards the mouse
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Prevent vertical movement

        // Calculate the desired new position
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Check if the new position is within the allowed radius
        float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
        if (distanceFromCenter < radius)
        {
            // If within the radius, move normally
            transform.position = newPosition;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            // If outside the radius, move to the closest point on the boundary
            Vector3 clampedPosition = centerPt.transform.position + (newPosition - centerPt.transform.position).normalized * radius;
            transform.position = clampedPosition;
            animator.SetBool("IsRunning", true);
        }

        // Check if close enough to stop moving
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            animator.SetBool("IsRunning", false);
        }
    }

    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Player moves on XZ plane
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 worldPosition = ray.GetPoint(rayDistance);
            worldPosition.y = 0; // Ensure Y is set to zero
            return worldPosition;
        }

        return Vector3.zero;
    }

    #endregion

    #region Effort Control

    void OnEffort()
    {
        if (effortPeriod == true)
        {
            if (Keyboard.current.sKey.isPressed && Keyboard.current.dKey.isPressed && Keyboard.current.fKey.isPressed)
            {
                float currentTime = Time.time;
                latency = currentTime - lastPressTime;
                lastPressTime = currentTime;
                pressTimes.Add(Time.time);
                counter++;
                dataHandler.recordEffort();  
                // effort();      
            }
        }
    }

     public void SetPressRate(float weight)
    {
        // Reset speed to 0
        distance = Vector3.Distance(player.transform.position, centerPt.transform.position);
         // Determine desired speeds for each distance category
    
        // Adjust required press rates and decay rate based on weight
        if (distance >= 7 ) // Far cookie
        {
            desiredSpeedMax = distance/9f;
        }
        else if (distance >= 5 ) // Middle cookie
        { 
            desiredSpeedMax = distance/7f;
        }
        else  // Near cookie
        { 
            desiredSpeedMax = distance/5f;
        }
        
        if(weight >= 3f)
        {
            requiredPressRate = (MinPressCount/10f);
            decayRate = 4f;
        }
        else
        {   
            requiredPressRate= (MinPressCount/10f) * 0.4f;
            decayRate = 1f;
        }


    }

    void CalculatePressRate()
    {
        // Remove timestamps older than the time window
        pressTimes.RemoveAll(timestamp => timestamp < Time.time - 1f);

        // Calculate the press rate as the number of presses per second
        currentPressRate = pressTimes.Count / 1f;
    }

    /// <summary>
    /// Adjusts the speed based on the current press rate and defined thresholds.
    /// </summary>
//    void AdjustSpeed()
//     {
//         float desiredSpeed;

//         // Determine speed based on press rate
//         if (currentPressRate >= requiredPressRate)
//         {
//             desiredSpeed = 1f; // Full speed
//         }
//         else if (currentPressRate >= requiredPressRate * 0.5f)
//         {
//             desiredSpeed = 0.5f; // Reduced speed
//         }
//         else if (currentPressRate >= requiredPressRate * 0.25f)
//         {
//             desiredSpeed = 0.25f; // Slower speed
//         }
//         else
//         {
//             desiredSpeed = 0f; // Speed decays to zero if press rate < 25% of required
//         }

//         // Interpolate the current speed towards the desired speed for smooth transitions
//         speed = Mathf.Lerp(speed, desiredSpeed, Time.deltaTime * speedTransitionSpeed);

//         // Apply decay only if press rate is below the 25% threshold
//         if (currentPressRate < requiredPressRate * 0.25f)
//         {
//             float decayMultiplier = 0.85f; // Decay factor per second
//             speed *= Mathf.Pow(decayMultiplier, Time.deltaTime);
//             speed = Mathf.Clamp(speed, 0f, maxSpeed); // Ensure speed stays within bounds
//         }

//         // Ensure speed does not exceed maxSpeed
//         speed = Mathf.Clamp(speed, 0f, maxSpeed);
//     }
void AdjustSpeed()
{
    float desiredSpeed=0;
    // // Determine speed based on press rate
    if (currentPressRate >= requiredPressRate)
    {
        desiredSpeed = desiredSpeedMax; // Full speed
    }
    else if (currentPressRate >= requiredPressRate * 0.5f)
    {
        desiredSpeed = desiredSpeedMax*0.5f; // Reduced speed
    }
    else if (currentPressRate >= requiredPressRate * 0.25f)
    {
        desiredSpeed = desiredSpeedMax*0.25f; // Slower speed
    }
    else
    {
        desiredSpeed = 0; // Speed decays to zero if press rate < 25% of required
    }

    // Use a steeper transition towards the desired speed
    if (desiredSpeed > speed)
    {
        speed = Mathf.MoveTowards(speed, desiredSpeed, Time.deltaTime * speedTransitionSpeed); // Faster increase
    }
    else
    {
        speed = Mathf.Lerp(speed, desiredSpeed, Time.deltaTime * speedTransitionSpeed*2f); // Smooth decrease
    }

    // Apply decay if press rate is below the 25% threshold
    if (currentPressRate < requiredPressRate * 0.25f)
    {
        // float decayMultiplier = 0.85f; // Decay factor per second
        speed *= Mathf.Pow(decayRate, Time.deltaTime);
        speed = Mathf.Clamp(speed, 0f, maxSpeed); // Ensure speed stays within bounds
    }

    // // Ensure speed does not exceed maxSpeed
    speed = Mathf.Clamp(speed, 0f, maxSpeed);
}


    private float currentUIspeed = 0f;
    public float transitionSpeed = 5f; // Adjust this value to control the smoothness

    void effortUI()
    {
        float targetUIspeed = speed/desiredSpeedMax;
        currentUIspeed = Mathf.Lerp(currentUIspeed, targetUIspeed, Time.deltaTime * transitionSpeed);
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(currentUIspeed);
    }


    public void enableEffort(float latency)
    {
        effortPeriod = true;
        MinPressLatency = latency;
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
    }
    public void enableAcorns()
    {
        acornPeriod = true;
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(1f);
    }

    public void stopRunning()
    {
        animator.SetBool("IsRunning", false);
        effortPeriod=false;
        speed=0f;
        transform.position = new Vector3(0f, 0f,0f);
    }

    #endregion

    #region Escape Control
    void OnEscape() //we're going to map the escape key to the spacebar
    {
        if (acornPeriod == true)
        {
            dataHandler.recordEscape();
            mainCamera.GetComponent<CameraGreyScale>().SetGreyscale(true);
            HeadsUpDisplay.GetComponent<UIController>().SetInstructText("Return to safety!");
            playerManager.canCollectAcorns = true;
        }
    }
    #endregion
    #region Old Code
    /// OLD MOVEMENT CODE DO NOT DELETE UNTIL GAME IS LIVE
    // void move()
    // {
    //             if (effortPeriod == true)
    //     {
    //         float verticalInput = Input.GetAxis("Vertical"); // Get input for forward movement along the y-axis
    //         float horizontalInput = Input.GetAxis("Horizontal"); // Get input for rotation along the x-axis

    //         Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized; // Use vertical input for forward movement
    //         Vector3 mover = new Vector3(0,0, Mathf.Abs(verticalInput)); // Use horizontal input for rotation
    //         Vector3 mover2 = new Vector3(0,0,Mathf.Abs(horizontalInput)); // Use horizontal input for rotation



    //         if (mover != Vector3.zero)
    //         {
    //             // Calculate the target rotation based on the movement direction

    //             // Move the player forward based on movement direction
    //             animator.SetBool("IsRunning", true);
    //             transform.Translate(mover * speed * Time.deltaTime);
    //             Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
    //             player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);

    //         }

    //         else if(mover2 != Vector3.zero)
    //         {
    //             Debug.Log("mover2");
    //             animator.SetBool("IsRunning", true);
    //             transform.Translate(mover2 * speed * Time.deltaTime);
    //             Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
    //             player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
    //         }


    //         else
    //         {
    //             animator.SetBool("IsRunning", false);
    //         }

    //     }
    // }

    // void OnEffort()
    // {
    //     if(effortPeriod==true && numPress<pressLimit)
    //     {
    //         Vector3 newPosition = transform.position + (transform.forward * stepSize);

    //         // Calculate the distance from the center of the circle
    //         float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);

    //         // Check if the new position is within the specified radius
    //         if (distanceFromCenter <= radius)
    //         {
    //             transform.position = newPosition;
    //         }
    //         animator.Play("Running");
    //         HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy();
    //         numPress++;

    //         if(numPress==pressLimit)  StartCoroutine(CoolDown());
    //     }
    // }
    //  public void RotatePlayer()
    // {
    //     float rotationSpeed = 150f;
    //     // float horizontalInput = Input.GetAxis("Horizontal");
    //     // float verticalInput = Input.GetAxis("Vertical");

    //     // Calculate the movement direction based on input
    //     // Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;


    //         // Calculate the target rotation based on the movement direction
    //         // Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
    //          Vector3 targetDirection = centerPt.transform.position - transform.position;
    //          Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //         // Smoothly interpolate towards the target rotation
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,1f);
    //     // }
    // }



    // if(player.GetComponent<PlayerManager>().cookieState == "light")
    // {
    //          if(latency<=0.22f)
    //   {
    //     speed = 7f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.85f);
    //   }
    //   else if (latency<=0.5f)
    //   {
    //     speed = 4f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.5f);

    //   }
    //   else if (latency<=3f)
    //   {
    //     speed = 2.5f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.25f);
    //   }
    //   else
    //   {
    //     speed=2f; 
    //   }
    //   latency+=0.001f;
    //   HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy(0.1f);

    // }

    // else  if(player.GetComponent<PlayerManager>().cookieState == "heavy")
    // {
    //     if(latency<=0.18f)
    //   {
    //     speed = 7f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.85f);
    //   }
    //   else if (latency<=0.3f)
    //   {
    //     speed = 4f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.5f);

    //   }
    //   else if (latency<=2f)
    //   {
    //     speed = 2.5f;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.25f);
    //   }
    //   else
    //   {
    //     speed=1f; 
    //   }
    //   latency+=0.001f;
    //   HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy(0.1f);

    // }

    // #region Drift
    //     void drift()
    //             {
    //                 // Calculate the direction vector from the player to the target
    //                 Vector3 targetDirection = centerPt.transform.position - transform.position;

    //                 Debug.DrawRay(transform.position, targetDirection, Color.blue, debugLineWidth);

    //                 // Apply the offset angle
    //                 Quaternion targetRotation = Quaternion.Euler(0f, theta, 0f);                

    //                 // Calculate the drift vector (theta degrees away from the target direction)
    //                 driftVector =targetRotation * targetDirection;
    //                 // Normalize the drift vector and scale it by the drift speed
    //                 driftVector = new Vector3(driftVector.x, 0f, driftVector.z).normalized * resistance;
    //                 Debug.DrawRay(transform.position, driftVector, Color.red, debugLineWidth);
    //                 // Apply the drift vector to the player's position
    //                 driftdrawVector = transform.position+driftVector;


    //                 Vector3 newPosition = transform.position + driftVector * Time.deltaTime;


    //                 float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
    //                 if (distanceFromCenter <= radius)
    //                 {
    //                     // Apply the drift vector to the player's position
    //                     transform.position = newPosition;
    //                 }
    // //             }
    // #endregion

    // IEnumerator CoolDown()
    // {
    //     yield return new WaitForSeconds(1f);
    //     resetEnergy();
    // }

    // public void resetEnergy()
    // {
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
    //     numPress=0;
    // }

    // public void enableEffort()
    // {
    //     effortPeriod = true;
    //     HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
    //     HeadsUpDisplay.GetComponent<UIController>().windSignal.SetActive(true);
    //     HeadsUpDisplay.SetActive(true);

    // }
    // public void resetEffort()
    // {
    //     pressLimit = defaultPressLimit;
    //     stepSize = defaultStepSize;
    //     numPress=0;
    //     HeadsUpDisplay.GetComponent<UIController>().windSignal.SetActive(false);
    //     HeadsUpDisplay.SetActive(false);

    // }

    void effort()
    {   
           Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
           targetPosition = GetWorldPosition(mouseScreenPosition);
           setStepDirection();
    }

     void setStepDirection()
    {
        // Calculate direction towards the mouse
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Prevent vertical movement
        float remainingDistance = Vector3.Distance(transform.position, centerPt.transform.position);

        // Dynamically adjust step size
        stepSize = remainingDistance / remainingPresses;

        // Calculate the desired new position
        Vector3 newPosition = transform.position + (direction * stepSize);

        // Check if the new position is within the allowed radius
        float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
        if (distanceFromCenter < radius)
        {
            // If within the radius, move normally
            transform.position = newPosition;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            // If outside the radius, move to the closest point on the boundary
            Vector3 clampedPosition = centerPt.transform.position + (newPosition - centerPt.transform.position).normalized * radius;
            transform.position = clampedPosition;
            animator.SetBool("IsRunning", true);
        }
        // Check if close enough to stop moving
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.1f) 
        {
            animator.SetBool("IsRunning", false);
        }

        remainingPresses--;

    }
    #endregion

}


  