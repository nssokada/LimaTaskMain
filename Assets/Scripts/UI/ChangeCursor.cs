using UnityEngine;
using System.Collections;

public class ChangeCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Texture2D targetCursorTexture;
    public Texture2D moveCursorTexture;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        // Set the default cursor texture
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnDisable()
    {
        // Reset the cursor when the script or object is disabled
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void setTargetCursor()
    {
        Cursor.SetCursor(targetCursorTexture, hotSpot, cursorMode);
    }

    public void setMoveCursor()
    {
        Cursor.SetCursor(moveCursorTexture, hotSpot, cursorMode);
    }

    public void setDefaultCursor()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Call this method to lock the cursor at the center of the screen
    public void MoveCursorToCenterAndUnlock()
    {
        #if UNITY_WEBGL
        // For WebGL builds, lock the cursor to capture mouse delta movements, but cursor remains visible
        Cursor.lockState = CursorLockMode.Locked; // This will capture mouse delta in WebGL
        Cursor.visible = true;  // You can choose to make the cursor visible or invisible as needed
        #else
        // For non-WebGL builds, lock the cursor to the center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // After one frame, unlock the cursor and make it visible again
        #endif
        StartCoroutine(UnlockCursorNextFrame());

    }

    // Coroutine to unlock the cursor after one frame (for non-WebGL)
    private IEnumerator UnlockCursorNextFrame()
    {
        yield return null;

        // Unlock the cursor and make it visible again
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // void Update()
    // {
    //     // Only in WebGL do we care about mouse delta while locked
    //     if (Cursor.lockState == CursorLockMode.Locked)
    //     {
    //         // Get the mouse delta movement (simulated from center)
    //         float mouseX = Input.GetAxis("Mouse X");
    //         float mouseY = Input.GetAxis("Mouse Y");

    //         // Process the mouseX and mouseY as if the cursor is moving from the center of the screen
    //         // Implement any movement or camera rotation logic here
    //         Debug.Log("Mouse Delta X: " + mouseX + ", Mouse Delta Y: " + mouseY);
    //     }


    // }
}
