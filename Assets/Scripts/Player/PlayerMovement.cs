using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public GameObject player;
    public GameObject HeadsUpDisplay;
    public float speed = 5;
    public int pressLimit;

    public static int defaultPressLimit = 2;
    public static int defaultStepSize = 1;

    PlayerInput playerInput;
    InputAction moveAction;
    Animator animator;
    private bool isMoving = false;
    public bool effortPeriod;
    public bool clickingPeriod;
    public int numPress;
     Vector3 movementDirection;
    public GameObject centerPt;
    public float radius;
    public float debugLineWidth;
    public float stepSize = 1f; 


    public float resistance;
    public float rotationSpeed;
    public float theta;
    public Vector3 driftVector;
    public Vector3 driftdrawVector;

    float lastPress;
    float t1;
    float latency =10f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        // moveAction = playerInput.actions.FindAction("Move");
    }


    void move()
    {
        //  if(effortPeriod==true)
        //  {
        //     float horizontalInput = Input.GetAxis("Horizontal");
        //     float verticalInput = Input.GetAxis("Vertical");

        //      movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        //      transform.Translate(movementDirection * 2f * Time.deltaTime);
            
        //      animator.SetBool("IsRunning", true);
           
        // }
                if (effortPeriod == true)
        {
            float verticalInput = Input.GetAxis("Vertical"); // Get input for forward movement along the y-axis
            float horizontalInput = Input.GetAxis("Horizontal"); // Get input for rotation along the x-axis

            Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized; // Use vertical input for forward movement
            Vector3 mover = new Vector3(0,0, Mathf.Abs(verticalInput)); // Use horizontal input for rotation
            Vector3 mover2 = new Vector3(0,0,Mathf.Abs(horizontalInput)); // Use horizontal input for rotation



            if (mover != Vector3.zero)
            {
                // Calculate the target rotation based on the movement direction
                            // Move the player forward based on movement direction
                animator.SetBool("IsRunning", true);
                transform.Translate(mover * speed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);

            }
          
            else if(mover2 != Vector3.zero)
            {
                Debug.Log("mover2");
                animator.SetBool("IsRunning", true);
                transform.Translate(mover2 * speed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 1f);
            }


            else
            {
                animator.SetBool("IsRunning", false);
            }

        }
    }



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

    void OnEffort()
    {
         if(effortPeriod==true)
        {
          t1 =Time.time;
          latency = t1-lastPress;
          Debug.Log(latency);
          lastPress = t1;
        }
    }
    void effort()
    {
        if(player.GetComponent<PlayerManager>().cookieState == "light")
        {
                 if(latency<=0.22f)
          {
            speed = 7f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.85f);
          }
          else if (latency<=0.5f)
          {
            speed = 4f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.5f);

          }
          else if (latency<=3f)
          {
            speed = 2.5f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.25f);
          }
          else
          {
            speed=2f; 
          }
          latency+=0.001f;
          HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy(0.1f);

        }

        else  if(player.GetComponent<PlayerManager>().cookieState == "heavy")
        {
            if(latency<=0.18f)
          {
            speed = 7f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.85f);
          }
          else if (latency<=0.3f)
          {
            speed = 4f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.5f);

          }
          else if (latency<=2f)
          {
            speed = 2.5f;
            HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0.25f);
          }
          else
          {
            speed=1f; 
          }
          latency+=0.001f;
          HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy(0.1f);

        }
       

    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(1f);
        resetEnergy();
    }

    public void resetEnergy()
    {
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
        numPress=0;
    }

    public void enableEffort()
    {
        effortPeriod = true;
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(0f);
        HeadsUpDisplay.GetComponent<UIController>().windSignal.SetActive(true);
        HeadsUpDisplay.SetActive(true);

    }
    public void resetEffort()
    {
        pressLimit = defaultPressLimit;
        stepSize = defaultStepSize;
        numPress=0;
        HeadsUpDisplay.GetComponent<UIController>().windSignal.SetActive(false);
        HeadsUpDisplay.SetActive(false);

    }


    void Update()
    {
        if(effortPeriod==true)
        {          
            move();
            drift();
            effort();
            // RotatePlayer();
        }
    }

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
            if (player.GetComponent<PlayerManager>().carrying)
            {
                Debug.Log("arrived");
                animator.SetBool("IsRunning", false);
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }


    void drift()
        {
            // Calculate the direction vector from the player to the target
            Vector3 targetDirection = centerPt.transform.position - transform.position;

            Debug.DrawRay(transform.position, targetDirection, Color.blue, debugLineWidth);

            // Apply the offset angle
            Quaternion targetRotation = Quaternion.Euler(0f, theta, 0f);                
        
            // Calculate the drift vector (theta degrees away from the target direction)
            driftVector =targetRotation * targetDirection;
            // Normalize the drift vector and scale it by the drift speed
            driftVector = new Vector3(driftVector.x, 0f, driftVector.z).normalized * resistance;
            Debug.DrawRay(transform.position, driftVector, Color.red, debugLineWidth);
            // Apply the drift vector to the player's position
            driftdrawVector = transform.position+driftVector;
            
            
            Vector3 newPosition = transform.position + driftVector * Time.deltaTime;
            
            
            float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
            if (distanceFromCenter <= radius)
            {
                // Apply the drift vector to the player's position
                transform.position = newPosition;
            }
        }

}

