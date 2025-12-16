using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    // RELAY
    public Action<string> OnAnimationEvent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError($"{name}: sin animator en EnemyAnimationController.");
    }

    public void PlayIdle() => animator.Play("idle");
    public void PlayMovement() => animator.Play("walk");
    public void PlayHurt() => animator.Play("hurt");
    public void PlayDeath() => animator.Play("death");
    public void PlayAttack() => animator.Play("attack");
    public void PlayRecovery() => animator.Play("hurt");

    // RELAY
    public void AnimationEvent(string eventName)
    {
        OnAnimationEvent?.Invoke(eventName);
    }
}
