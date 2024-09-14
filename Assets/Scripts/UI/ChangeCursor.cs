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
        Cursor.SetCursor(cursorTexture, CalculateHotSpot(cursorTexture), cursorMode);
    }

    void OnDisable()
    {
        // Reset the cursor when the script or object is disabled
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void setTargetCursor()
    {
        Cursor.SetCursor(targetCursorTexture, CalculateHotSpot(targetCursorTexture), cursorMode);
    }

    public void setMoveCursor()
    {
        Cursor.SetCursor(moveCursorTexture, CalculateHotSpot(moveCursorTexture), cursorMode);
    }

    public void setDefaultCursor()
    {
        Cursor.SetCursor(cursorTexture, CalculateHotSpot(cursorTexture), cursorMode);
    }

    // Call this method to lock the cursor at the center of the screen
    public void MoveCursorToCenterAndUnlock()
    {
        #if UNITY_WEBGL
        // For WebGL builds, lock the cursor to capture mouse delta movements, but cursor remains visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        #else
        // For non-WebGL builds, lock the cursor to the center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    // Calculate the hotspot relative to the texture size
    private Vector2 CalculateHotSpot(Texture2D texture)
    {
        // Calculate the hotspot as the center of the texture
        return new Vector2(texture.width / 2, texture.height / 2);
    }
}
