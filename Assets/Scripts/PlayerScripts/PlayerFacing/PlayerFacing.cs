using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    [Header("Facing")]
    public FacingDirection facingDirection;

    public enum FacingDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public void UpdateFacingDirection(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero) return;

        float angle = Vector2.SignedAngle(Vector2.up, moveDirection);
        angle = 360f - angle;
        if (angle >= 360f) angle -= 360f;
        if (angle < 0f) angle += 360f;

        int index = Mathf.RoundToInt(angle / 45f) % 8;
        facingDirection = (FacingDirection)index;
    }
}
