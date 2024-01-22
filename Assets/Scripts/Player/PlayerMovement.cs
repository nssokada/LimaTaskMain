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

    public GameObject centerPt;
    public float radius;
    public float debugLineWidth;
    public float stepSize = 1f; // The distance the player moves with each step
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
    }



    void OnEffort()
    {
        if(effortPeriod==true && numPress<pressLimit)
        {
            Vector3 newPosition = transform.position + (transform.forward * stepSize);

            // Calculate the distance from the center of the circle
            float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);

            // Check if the new position is within the specified radius
            if (distanceFromCenter <= radius)
            {
                transform.position = newPosition;
            }
            animator.Play("Running");
            HeadsUpDisplay.GetComponent<UIController>().DecreaseEnergy();
            numPress++;

            if(numPress==pressLimit)  StartCoroutine(CoolDown());
        }
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(1f);
        resetEnergy();
    }

    public void resetEnergy()
    {
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(pressLimit/10f);
        numPress=0;
    }

    public void enableEffort()
    {
        effortPeriod = true;
        HeadsUpDisplay.GetComponent<UIController>().SetEnergy(pressLimit/10f);
        HeadsUpDisplay.SetActive(true);
    }
    public void resetEffort()
    {
        pressLimit = defaultPressLimit;
        stepSize = defaultStepSize;
        numPress=0;
        HeadsUpDisplay.SetActive(false);

    }


    void Update()
    {
        if(effortPeriod==true)
        {          
            RotatePlayer();
        }
    }

     public void RotatePlayer()
    {
        float rotationSpeed = 150f;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on input
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (movementDirection != Vector3.zero)
        {
            // Calculate the target rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Smoothly interpolate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

  

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
}


//UNNEEDED OLD CODE: Remove when the time comes:
/**

#DRIFT
    void Update()
    {
        if(effortPeriod==true)
        {          
            drift();
            RotatePlayer();
        }
    }

    void drift()
    {
        float driftSpeed= 1f;
        float rotationSpeed= 200f;

        float offsetAngle =135f;
        
         // Calculate the direction vector from the player to the target
        Vector3 targetDirection = centerPt.transform.position - transform.position;

        Debug.DrawRay(transform.position, targetDirection, Color.blue, debugLineWidth);

        // Apply the offset angle
        Quaternion targetRotation = Quaternion.Euler(0, offsetAngle, 0);                
    
        // Calculate the drift vector (45 degrees away from the target direction)
        Vector3 driftVector =targetRotation * targetDirection;

        // Normalize the drift vector and scale it by the drift speed
        driftVector = driftVector.normalized * driftSpeed;
        Debug.DrawRay(transform.position, driftVector, Color.red, debugLineWidth);
        // Apply the drift vector to the player's position
        Vector3 newPosition = transform.position + driftVector * Time.deltaTime;
        float distanceFromCenter = Vector3.Distance(centerPt.transform.position, newPosition);
        if (distanceFromCenter <= radius)
        {
            // Apply the drift vector to the player's position
            transform.position = newPosition;
        }
    }
    **/