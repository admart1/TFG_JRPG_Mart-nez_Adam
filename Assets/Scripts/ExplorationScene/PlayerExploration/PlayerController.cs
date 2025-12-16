using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody2D Rigidbody;
    [HideInInspector] public PlayerFacing playerFacing;
    [HideInInspector] public PlayerAnimationController animationController;
    [HideInInspector] public InputReader input;
    [HideInInspector] public ExplorationAttackHitbox attackHitbox;
    [HideInInspector] public PlayerHeightSystem heightSystem;
    [HideInInspector] public PlayerTriggerDetector triggerDetector;
    [HideInInspector] public InventoryController inventoryController;
    [HideInInspector] public InventoryUIController inventoryUI;
    [HideInInspector] public PlayerCombatDetector combatDetector;

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
    [HideInInspector] public PlayerFallState FallState;
    [HideInInspector] public PlayerSwordMenuState SwordMenuState;


    [Header("Nivel del dash")]
    [SerializeField] public DashLevel currentDashLevel = DashLevel.Level1;

    [Header("Sistema de Tiles/Altura")]
    public int initialHeight = 0;

    public PlayerStateMachine stateMachine;

    void Awake()
    {        
        // referencias
        playerFacing = GetComponent<PlayerFacing>();
        animationController = GetComponent<PlayerAnimationController>();
        input = GetComponent<InputReader>();
        attackHitbox = GetComponentInChildren<ExplorationAttackHitbox>();
        heightSystem = GetComponent<PlayerHeightSystem>();
        triggerDetector = GetComponentInChildren<PlayerTriggerDetector>();
        inventoryController = Object.FindAnyObjectByType<InventoryController>();
        inventoryUI = Object.FindAnyObjectByType<InventoryUIController>();
        combatDetector = Object.FindAnyObjectByType<PlayerCombatDetector>();

        // crear estados
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState();
        MovementState = new PlayerMovementState();
        AttackState = new PlayerAttackState();
        DashState = new PlayerDashState();
        FallState = new PlayerFallState();
        SwordMenuState = new PlayerSwordMenuState();

        // inicializar estados
        IdleState.Initialize(this, stateMachine);
        MovementState.Initialize(this, stateMachine);
        AttackState.Initialize(this, stateMachine);
        DashState.Initialize(this, stateMachine);
        FallState.Initialize(this, stateMachine);
        SwordMenuState.Initialize(this, stateMachine);
    }

    void Start()
    {
        stateMachine.Initialize(IdleState);
        character = partyManager.activeCharacter;
        heightSystem.SetHeight(initialHeight);
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

    // Funciones de utilidad
    public void SetVelocity(Vector2 vel)
    {
        Rigidbody.linearVelocity = vel;
    }

    // FUNCIONES TEMPORALES, A SER COLOCADAS EN SCRIPTS DE FORMA MAS ORDENADA
    public void EquipSwordToActiveCharacter(EquipableSword sword, int slotIndex)
    {
        if (character == null)
        {
            Debug.LogWarning("No hay character activo para equipar espada.");
            return;
        }

        if (inventoryController == null)
        {
            Debug.LogWarning("No hay InventoryController asignado.");
            return;
        }

        if (!inventoryController.GetOwnedSwords().Contains(sword))
        {
            Debug.LogWarning($"La espada '{sword.displayName}' no está en el inventario.");
            return;
        }

        character.EquipSword(sword, slotIndex);
        Debug.Log($"Espada '{sword.displayName}' equipada en el slot {slotIndex} de {character.definition.displayName}.");
    }
}