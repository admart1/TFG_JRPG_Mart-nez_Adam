using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void AnimationEvent(string eventName)
    {
        player.stateMachine.CurrentState.OnAnimationEvent(eventName);
    }
}