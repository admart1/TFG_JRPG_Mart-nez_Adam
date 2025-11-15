using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public PlayerController playerController;
    public float zOffset = -10f;
    public float yOffset = 0.7f;
    public float smoothTime = 0.2f;
    public float lookAheadDistance = 1f;
    public float lookAheadSmoothTime = 0.1f;

    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 lookAheadPos = Vector3.zero;
    private Vector3 lookAheadVelocity = Vector3.zero;

    private void Start()
    {
        if (playerController != null)
        {
            Vector2 initialDir = playerController.MoveDirection;
            if (initialDir.magnitude > 0.01f)
            {
                lookAheadPos = new Vector3(initialDir.x, initialDir.y, 0f).normalized * lookAheadDistance;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null || playerController == null) return;

        Vector2 moveDir = playerController.MoveDirection;

        Vector3 basePosition = new Vector3(target.position.x, target.position.y + yOffset, zOffset);

        Vector3 targetLookAhead = new Vector3(moveDir.x, moveDir.y, 0f).normalized * lookAheadDistance;
          lookAheadPos = Vector3.SmoothDamp(lookAheadPos, targetLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);


        Vector3 finalTarget = basePosition + lookAheadPos;
        transform.position = Vector3.SmoothDamp(transform.position, finalTarget, ref smoothVelocity, smoothTime);
    }
}




/*
 * using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;    
    public Rigidbody2D playerRb;     

    [Header("Camera Settings")]
    public float zOffset = -10f;    
    public float yOffset = 0.7f;     
    public float smoothTime = 0.2f;   

    [Header("Look-Ahead Settings")]
    public float lookAheadDistance = 1f;   
    public float lookAheadSmoothTime = 0.2f;  

    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 lookAheadPos = Vector3.zero;
    private Vector3 lookAheadVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null || playerRb == null) return;

        Vector2 moveDir = playerRb.linearVelocity;

        Vector3 targetLookAhead = Vector3.zero;
        if (moveDir.magnitude > 0.01f)
        {
            targetLookAhead = new Vector3(moveDir.x, moveDir.y, 0f).normalized * lookAheadDistance;
        }

        lookAheadPos = Vector3.SmoothDamp(lookAheadPos, targetLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + yOffset, zOffset) + lookAheadPos;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, smoothTime);
    }
}
*/