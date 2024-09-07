using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChangeCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Texture2D targetCursorTexture;
    public Texture2D moveCursorTexture;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        // Change the cursor to the texture defined in cursorTexture
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnDisable()
    {
        // Reset cursor when script or object is disabled
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

         // Call this method to move the cursor to the center of the screen and unlock it
    public void MoveCursorToCenterAndUnlock()
    {
        // Lock the cursor, which moves it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // After one frame, unlock the cursor and make it visible again
        StartCoroutine(UnlockCursorNextFrame());
    }

    // Coroutine to unlock the cursor after one frame
    private IEnumerator UnlockCursorNextFrame()
    {
        // Wait until the end of the current frame
        yield return null;

        // Unlock the cursor and make it visible again
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
