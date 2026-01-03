using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public PlayerController playerController;

    [Header("Offsets")]
    public float zOffset = -10f;
    public float yOffset = 0.7f;

    [Header("Smoothing")]
    public float smoothTime = 0.2f;

    [Header("Look-Ahead")]
    public float lookAheadDistance = 1f;
    public float lookAheadSmoothTime = 0.1f;

    [Header("Bounds")]
    public BoxCollider2D cameraBounds;

    private Camera cam;
    private Vector3 smoothVelocity;
    private Vector3 lookAheadPos;
    private Vector3 lookAheadVelocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null || playerController == null) return;

        Vector2 moveDir = playerController.MoveDirection;

        Vector3 basePosition = new Vector3(
            target.position.x,
            target.position.y + yOffset,
            zOffset
        );

        Vector3 targetLookAhead = Vector3.zero;
        if (moveDir.magnitude > 0.01f)
        {
            targetLookAhead =
                new Vector3(moveDir.x, moveDir.y, 0f).normalized * lookAheadDistance;
        }

        lookAheadPos = Vector3.SmoothDamp(
            lookAheadPos,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        Vector3 desiredPosition = basePosition + lookAheadPos;

        if (cameraBounds != null)
        {
            Bounds bounds = cameraBounds.bounds;

            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            desiredPosition.x = Mathf.Clamp(
                desiredPosition.x,
                bounds.min.x + halfWidth,
                bounds.max.x - halfWidth
            );

            desiredPosition.y = Mathf.Clamp(
                desiredPosition.y,
                bounds.min.y + halfHeight,
                bounds.max.y - halfHeight
            );
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref smoothVelocity,
            smoothTime
        );
    }
}
