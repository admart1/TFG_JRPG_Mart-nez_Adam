using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody2D Rigidbody;
    [HideInInspector] public PlayerFacing playerFacing;
    [HideInInspector] public PlayerAnimationController animationController;
    [HideInInspector] public InputReader input;
    [HideInInspector] public ExplorationAttackHitbox attackHitbox;

    [Header("Persoanjes")]
    public PartyManager partyManager;
    public CharacterModel character;

    [Header("Movimiento")]
    public float MovementSpeed = 3;
    public float BaseSpeed = 3;
    [HideInInspector] public Vector2 MoveDirection; 

    // Estados
    [HideInInspector] public PlayerIdleState IdleState;
    [HideInInspector] public PlayerMovementState MovementState;
    [HideInInspector] public PlayerAttackState AttackState;
    [HideInInspector] public PlayerDashState DashState;

    // Adicionales
    [SerializeField] public DashLevel currentDashLevel = DashLevel.Level1;

    public PlayerStateMachine stateMachine;

    void Awake()
    {
        // referencias
        playerFacing = GetComponent<PlayerFacing>();
        animationController = GetComponent<PlayerAnimationController>();
        input = GetComponent<InputReader>();
        attackHitbox = GetComponentInChildren<ExplorationAttackHitbox>();

        // crear estados
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState();
        MovementState = new PlayerMovementState();
        AttackState = new PlayerAttackState();
        DashState = new PlayerDashState();

        // inicializar estados
        IdleState.Initialize(this, stateMachine);
        MovementState.Initialize(this, stateMachine);
        AttackState.Initialize(this, stateMachine);
        DashState.Initialize(this, stateMachine);
    }

    void Start()
    {
        stateMachine.Initialize(IdleState);
        character = partyManager.activeCharacter;
    }

    void Update()
    {
        // genericos
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.LogicUpdate();

        // timers
        DashState.DashTimerUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
}