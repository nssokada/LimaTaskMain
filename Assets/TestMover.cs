using UnityEngine;
using UnityEngine.InputSystem;

public class TesterMover : MonoBehaviour
{
    public Camera mainCamera;
    public float moveSpeed = 5f;

    private Vector3 targetPosition;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        targetPosition = transform.position;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            targetPosition = GetWorldPosition(mouseScreenPosition);

            MovePlayerTowardsTarget();
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


    void MovePlayerTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > 0.1f) // Avoid jittering when very close to the target
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
