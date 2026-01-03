using UnityEngine;

public class PlayerTriggerDetector : MonoBehaviour
{
    public WorldTrigger currentTrigger { get; private set; }

    public BoxCollider2D movementCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out WorldTrigger trigger) && trigger.CompareTag("MovementTrigger"))
        {
            currentTrigger = trigger;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out WorldTrigger trigger) && trigger.CompareTag("MovementTrigger"))
        {
            if (currentTrigger == trigger)
            {
                currentTrigger = null;

                //Debug.Log($"Trigger EXIT: {trigger.gameObject.name}");
            }
        }
    }
}
