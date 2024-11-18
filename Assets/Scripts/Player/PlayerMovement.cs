using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private float maxSpeed = 0.9f;
    public float cookieWeight;
    public float thresholdLow = 0.1f; // Example threshold for low latency
    public float smoothingFactor = 0.1f;
    public float requiredPressRate = 5f; // Presses per second required (e.g., 10 for heavy)
    public float currentPressRate = 0f;   // Current presses per second
    private float decayRate = 0.9f; // Speed decay rate when not maintaining effort


    //Togglable Bools
    public bool effortPeriod;
    public bool clickingPeriod;
    public bool acornPeriod;

    //Latency Information
    private float MinPressLatency;
    private float MinPressCount;
    public float stepSize;
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




    void OnEnable()
    {
        pressTimes = new List<float>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        speed = baseSpeed;

        // Fetch and log press latency from PlayerPrefs
        MinPressLatency = PlayerPrefs.GetFloat("PressLatency"); // Add default value to avoid null issues
        MinPressCount = PlayerPrefs.GetFloat("PressCount");
        SetStepSize();
        Debug.Log($"Initialized with Press Latency: {MinPressLatency} and Press Count {MinPressCount}");
    }

    void SetStepSize()
    {
        stepSize = 8.25f / (MinPressCount * 0.9f);
        lightSpeed = stepSize / (MinPressLatency * 0.65f);
    }

    public void SetLightSpeed()
    {
        speed = lightSpeed;
    }

    public void SetPressRate(float weight)
    {
        speed = 0f;
        if (weight >= 3f)
        {
            requiredPressRate = (MinPressCount / 10f);
            decayRate =1f;
        }
        else
        {
            requiredPressRate = (MinPressCount / 10f) * 0.05f;
            decayRate =0.8f;
        }
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
                dataHandler.recordEffort();
            }
        }
    }

    void CalculatePressRate()
    {
        float currentTime = Time.time;

        // Remove presses older than 1 second
        pressTimes.RemoveAll(time => time < currentTime - 1f);

        // Update current press rate
        currentPressRate = pressTimes.Count; // Since we're considering the last second
    }

    void AdjustSpeed()
    {
        // Determine the desired speed based on press rate
        float desiredSpeed = Mathf.Clamp(currentPressRate / requiredPressRate, 0f, 1f) * maxSpeed;

        // Smoothly interpolate current speed towards desired speed
        speed = Mathf.MoveTowards(speed, desiredSpeed, Time.deltaTime * maxSpeed);

        // If the press rate is below the required rate, apply decay
        if (currentPressRate < requiredPressRate)
        {
            speed -= decayRate * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0f, maxSpeed);
        }
    }

    private float currentUIspeed = 0f;
    public float transitionSpeed = 5f; // Adjust this value to control the smoothness

    void effortUI()
    {
        float targetUIspeed = speed / maxSpeed;
        currentUIspeed = Mathf.Lerp(currentUIspeed, targetUIspeed, Time.deltaTime * transitionSpeed);
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(speed);
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

    // void effort()
    // {   
    //     if (cookieWeight >= 3)
    //     {
    //        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
    //        targetPosition = GetWorldPosition(mouseScreenPosition);
    //        setStepDirection();
    //     }
    // }

    //  void setStepDirection()
    // {
    //     // Calculate direction towards the mouse
    //     Vector3 direction = (targetPosition - transform.position).normalized;
    //     direction.y = 0; // Prevent vertical movement

    //     // Calculate the desired new position
    //     Vector3 newPosition = transform.position + direction * stepSize;

    //     // Check if the new position is within the allowed radius
    //     float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
    //     if (distanceFromCenter < radius)
    //     {
    //         // If within the radius, move normally
    //         transform.position = newPosition;
    //         Quaternion targetRotation = Quaternion.LookRotation(direction);
    //         player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
    //         animator.SetBool("IsRunning", true);
    //     }
    //     else
    //     {
    //         // If outside the radius, move to the closest point on the boundary
    //         Vector3 clampedPosition = centerPt.transform.position + (newPosition - centerPt.transform.position).normalized * radius;
    //         transform.position = clampedPosition;
    //         animator.SetBool("IsRunning", true);
    //     }
    //     // Check if close enough to stop moving
    //     float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
    //     if (distanceToTarget < 0.1f) 
    //     {
    //         animator.SetBool("IsRunning", false);
    //     }
    // }
    #endregion

}

