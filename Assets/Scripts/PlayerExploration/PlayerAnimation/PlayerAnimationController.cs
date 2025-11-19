using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerAnimationDatabase animationDatabase;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerFacing playerFacing;


    private string currentAnimation;
    private float currentAnimationTime;

    public void Awake()
    {
       animator = GetComponent<Animator>();
       playerFacing = GetComponent<PlayerFacing>();
    }

    public void PlayAnimation(PlayerAnimationDatabase.PlayerState state, PlayerFacing.FacingDirection direction)
    {

        // consulta la DB
        string clipName = animationDatabase.GetAnimationName(state, direction);
        if (currentAnimation == clipName) return;

        float normalizedTime = 0f;

        // Si cambiamos de dirección mientras nos movemos, mantenemos el progreso
        if (state == PlayerAnimationDatabase.PlayerState.Movement)
        {
            normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        }

        animator.Play(clipName, 0, normalizedTime);
        currentAnimation = clipName;
    }
}
