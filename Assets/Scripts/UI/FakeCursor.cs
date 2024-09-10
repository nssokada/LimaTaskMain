using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    // Variables for the simulated cursor position
    private Vector2 simulatedCursorPos;

    // Screen boundaries (optional, to keep the cursor within bounds)
    private float screenMinX, screenMaxX, screenMinY, screenMaxY;

    void Start()
    {
        // Hide the system cursor
        Cursor.visible = false;

        // Set the simulated cursor initially at the center of the screen
        simulatedCursorPos = new Vector2(Screen.width / 2, Screen.height / 2);

        // Optional: Set screen boundaries to keep the fake cursor within the screen
        screenMinX = 0f;
        screenMaxX = Screen.width;
        screenMinY = 0f;
        screenMaxY = Screen.height;
    }

    void Update()
    {
        // Get mouse movement delta from the actual mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update the position of the fake cursor
        simulatedCursorPos.x += mouseX * 10f;  // Multiply by a factor to control movement speed
        simulatedCursorPos.y += mouseY * 10f;

        // Clamp the position so it stays within screen bounds (optional)
        simulatedCursorPos.x = Mathf.Clamp(simulatedCursorPos.x, screenMinX, screenMaxX);
        simulatedCursorPos.y = Mathf.Clamp(simulatedCursorPos.y, screenMinY, screenMaxY);

        // Simulate the cursor’s behavior here
        // For example, you could perform raycasts or trigger UI events at simulatedCursorPos
        HandleCursorClick();
    }

    // Example of handling clicks at the simulated cursor position
    private void HandleCursorClick()
    {
        if (Input.GetMouseButtonDown(0))  // Left mouse click
        {
            // Perform actions based on the simulated cursor position
            // You can raycast from the simulated cursor's position for UI or object interaction
            Ray ray = Camera.main.ScreenPointToRay(simulatedCursorPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked on: " + hit.collider.name);
                // Do something with the object that was clicked on
            }
        }
    }

    void OnGUI()
    {
        // Optional: Draw the simulated cursor (if you want some visual feedback)
        // GUI.Label(new Rect(simulatedCursorPos.x, simulatedCursorPos.y, 20, 20), "*");
    }
}
