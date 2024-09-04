using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
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
}
