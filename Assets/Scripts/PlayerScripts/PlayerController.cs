using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody2D Rigidbody;
    [HideInInspector] public PlayerFacing playerFacing;
    [HideInInspector] public PlayerAnimationController animationController;

    [Header("Movimiento")]
    public float MovementSpeed = 5f;
    [HideInInspector] public Vector2 MoveDirection; //para el movimiento, normalizado en movementstate

    // Estados
    [HideInInspector] public PlayerIdleState IdleState;
    [HideInInspector] public PlayerMovementState MovementState;
    [HideInInspector] public PlayerAttackState AttackState;


    public PlayerStateMachine stateMachine;

    void Awake()
    {
        // referencias
        playerFacing = GetComponent<PlayerFacing>();
        animationController = GetComponent<PlayerAnimationController>();

        // crear estados
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState();
        MovementState = new PlayerMovementState();
        AttackState = new PlayerAttackState();

        // inicializar estados
        IdleState.Initialize(this, stateMachine);
        MovementState.Initialize(this, stateMachine);
        AttackState.Initialize(this, stateMachine);
    }

    void Start()
    {
        stateMachine.Initialize(IdleState);
    }

    void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }

    public void AnimationEvent(string eventName)
    {
        stateMachine.CurrentState.OnAnimationEvent(eventName);
    }
}