using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    // Define the bounds or boundaries for the player
    public float minX, maxX, minZ, maxZ;

    private void OnTriggerExit(Collider other)
    {

        // Check if the player (or any other object with a collider) exits the trigger area
        if (other.CompareTag("Player"))
        {
            Debug.Log("running bound");
            // Get the player's position
            Vector3 playerPosition = other.transform.position;

            // Clamp the player's position to stay within the defined bounds
            float clampedX = Mathf.Clamp(playerPosition.x, minX, maxX);
            float clampedZ = Mathf.Clamp(playerPosition.z, minZ, maxZ);

            // Update the player's position to stay within bounds
            other.transform.position = new Vector3(clampedX, playerPosition.y, clampedZ);
        }
    }
}