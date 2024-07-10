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
    public Camera mainCamera;
    public GameObject centerPt;

    //Public values
    public float speed;
    public int numPress;
    public float radius;
    public float resistance;
    public float rotationSpeed;
    public float minSpeed = 0f;
    public float baseSpeed;
    public float maxSpeed;
    public float cookieWeight;
    public float thresholdLow = 0.1f; // Example threshold for low latency
    public float smoothingFactor = 0.1f;

    //Togglable Bools
    public bool effortPeriod;
    public bool clickingPeriod;


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
    private float MinPressLatency;
    




    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        speed = baseSpeed;
        MinPressLatency = PlayerPrefs.GetFloat("PressLatency");
        Debug.Log("Press Latency:"+MinPressLatency);
        InvokeRepeating("drag", 0.5f, 0.22f);
    }
    
    void Update()
    {
       if(effortPeriod==true)
       {
            mouseMove();
            effortUI();
       }
    }



#region Clicking To Select Reward
    void OnMouseClick()
    {
        if(clickingPeriod ==true)
        {
            Debug.Log("click");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
 
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag  =="Cookie")
                {
                    Debug.Log("Reward hit");
                    clickingPeriod =false;
                    StartCoroutine(moveObject(hit.transform.position));
                }
            } 
            else 
            {
                Debug.Log("Nothing hit");
            }
        }
       
    }

    
    IEnumerator moveObject(Vector3 newPosition)
    {
        float  startTime = Time.time;
        float speed =0.1f;
        Quaternion targetRotation = Quaternion.LookRotation(newPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
        float journeyLength = Vector3.Distance(transform.position, newPosition);
        while (true)
        {
            animator.SetBool("IsRunning", true);
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(transform.position, newPosition, fractionOfJourney);
           

            // If the object has arrived, stop the coroutine
            if (playerManager.carrying)
            {
                Debug.Log("arrived");
                animator.SetBool("IsRunning", false);
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }

#endregion

    
#region Player Movement
    void mouseMove()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            targetPosition = GetWorldPosition(mouseScreenPosition);
            float distanceFromCenter = Vector3.Distance(centerPt.transform.position, targetPosition);
             if (distanceFromCenter <= radius)
            {
                setMovementDirection();
            }
        }
    }

    void setMovementDirection()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Zero out the Y component to avoid vertical movement
        float distance = Vector3.Distance(transform.position, targetPosition);


    //         // Calculate the distance from the center of the circle

    //         // Check if the new position is within the specified radius
    //        


        if (distance > 0.1f) // Avoid jittering when very close to the target
        {
            transform.position += direction * speed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }


    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Assuming the player moves on the XZ plane
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 worldPosition = ray.GetPoint(rayDistance);
            worldPosition.y = 0; // Explicitly set Y to 0
            return worldPosition;
        }

        return Vector3.zero;
    }
#endregion

#region Effort Control
    
    void OnEffort()
    {
         if(effortPeriod==true)
        {
            float currentTime = Time.time;
            latency = currentTime - lastPressTime;
            Debug.Log("Latency between spacebar presses: " + latency + " seconds");
            lastPressTime = currentTime;
            effort();
        }
    }

    private float currentUIspeed = 0f;
    public float transitionSpeed = 5f; // Adjust this value to control the smoothness

    void effortUI()
    {
        float targetUIspeed = speed / maxSpeed;
        currentUIspeed = Mathf.Lerp(currentUIspeed, targetUIspeed, Time.deltaTime * transitionSpeed);
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(currentUIspeed);
        Debug.Log("current speed"+currentUIspeed);
        HeadsUpDisplay.GetComponent<UIController>().SetEnergyText(Mathf.Clamp((int)speed, 0, 9));

    }

    void drag()
    {
        speed -= 0.5f;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }


    void effort()
    {
        if(latency <=MinPressLatency)
        {
            speed += 0.5f*cookieWeight;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        }
        else if (latency <=MinPressLatency+MinPressLatency*0.5)
        {
            speed += 0.25f*cookieWeight;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        }
        else if(latency <= MinPressLatency+MinPressLatency*0.75)
        {
            speed += 0.1f*cookieWeight;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
    }


    public void enableEffort()
        {
            effortPeriod = true;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
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
#endregion




}

