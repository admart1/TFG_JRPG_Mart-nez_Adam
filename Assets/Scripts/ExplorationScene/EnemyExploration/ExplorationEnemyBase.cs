using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplorationEnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] public int maxHealth = 3;
    [HideInInspector] public float currentHealth;


    [Header("Referencias")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider2D collider;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerFacing playerFacing;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public EnemyAnimationController animController;
    [HideInInspector] public EnemyStateMachine stateMachine;
    [SerializeField] public EnemyGroupData enemyGroupData;
    [SerializeField] private Transform visualsRoot;


    [Header("Estados")]
    [HideInInspector] public EnemyState IdleState;
    [HideInInspector] public EnemyState ChaseState;
    [HideInInspector] public EnemyState AttackState;
    [HideInInspector] public EnemyState RecoveryState;
    [HideInInspector] public EnemyState KnockbackState;
    [HideInInspector] public EnemyState StunState;

    [Header("Afinidades")]
    [SerializeField] public bool isWeakToFire;
    [SerializeField] public bool isWeakToIce;
    [SerializeField] public bool isWeakToStandard;
    [SerializeField] public bool isResistantToFire;
    [SerializeField] public bool isResistantToStandard;
    [SerializeField] public bool isResistantToIce;



    [Header("Percepción del jugaodr")]
    [HideInInspector] public float chaseMemoryTime = 1.5f; // +tiempo = +chaseo
    [HideInInspector] public float chaseMemoryTimer;
    [HideInInspector] public Vector2 lastSeenPlayerPosition;
    [HideInInspector] public LayerMask wallLayerMask;     // fijo en ground_0 luego sistema de alturas..
    [HideInInspector] public float maxSightDistance = 5f;

    [Header("HUD")]
    [SerializeField] private GameObject enemyHUD;
    [SerializeField] private UnityEngine.UI.Image healthBarFill;
    [SerializeField] private GameObject selectionIcon;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private bool isKnockback = false;

    [Header("Orientation")]
    [SerializeField] protected FacingDirection facingDirection = FacingDirection.Right;
    public enum FacingDirection
    {
        Left = 1,
        Right = -1
    }

    #region INICIALIZACIÓN
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        animController = GetComponentInChildren<EnemyAnimationController>();

        stateMachine = new EnemyStateMachine();

        if (rb == null)
            Debug.LogError($"{name}: EnemyBase sin Rigidbody2D.");

        if (collider == null)
            Debug.LogError($"{name}: EnemyBase sin Collider2D.");

        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        // cache del playr
        var playerTargetObj = GameObject.FindGameObjectWithTag("PlayerEnemiesTarget");
        if (playerTargetObj != null)
            player = playerTargetObj.transform;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        playerFacing = playerObj.GetComponent<PlayerFacing>();
        playerController = playerObj.GetComponent<PlayerController>();

        // DATOS
        wallLayerMask = LayerMask.GetMask("EnemyGround");
    }
    #endregion


    #region UPDATES
    protected void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }

    protected void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion


    #region DAMAGE SYSTEM

    public virtual void TakeDamage(int damage, Vector2 hitPos, GameObject source)
    {
        if (stateMachine.CurrentState == StunState)
            return;

        if (stateMachine.isStunned)
            return;

        ShowHealthBar(true);


        float actualDamage = damage;
        currentHealth -= actualDamage * CalculateMultiplier();

        if (currentHealth <= 0)
        {
            stateMachine.ChangeState(StunState);
            UpdateHealthBar();
            return;
        }
        else
        {
            OnDamage(damage, hitPos, source);

            UpdateHealthBar();
        }
    }

    protected virtual void OnDamage(int damage, Vector2 hitPos, GameObject source)
    {
        animController?.PlayHurt();
        stateMachine.ChangeState(KnockbackState);
    }

    public float CalculateMultiplier()
    {
        SwordType activeSwordType = SwordType.Standard;

        if (playerController.character.activeSword == ActiveSword.Slot1)
            activeSwordType = playerController.character.SwordSlot1.swordType;
        else if (playerController.character.activeSword == ActiveSword.Slot2)
            activeSwordType = playerController.character.SwordSlot2.swordType;

        float multiplier = 1f;

        // Debilidad
        if ((activeSwordType == SwordType.Fire && isWeakToFire) ||
            (activeSwordType == SwordType.Ice && isWeakToIce))
        {
            multiplier = 2f;
        }

        // Resistencia
        if ((activeSwordType == SwordType.Fire && isResistantToFire) ||
            (activeSwordType == SwordType.Ice && isResistantToIce) ||
            (activeSwordType == SwordType.Standard && isResistantToStandard))
        {
            multiplier = 0.5f;
        }

        return multiplier;
    }

    #endregion

    #region ENEMY HUD

    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    public void ShowSelectionIcon(bool show)
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(show);
    }

    #endregion

    #region FUNCIONES UTILIDAD

    public void SetVelocity(Vector2 vel)
    {
        rb.linearVelocity = vel;
    }

    public bool HasLineOfSight()
    {
        if (player == null)
            return false;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > maxSightDistance)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, wallLayerMask);
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    public void ShowHealthBar(bool show)
    {
        if (enemyHUD != null)
            enemyHUD.SetActive(show);
    }

    public void HandleFlip(Vector2 movementDir)
    {
        if (movementDir.x == 0)
            return;

        FacingDirection newDir = movementDir.x > 0
            ? FacingDirection.Right
            : FacingDirection.Left;

        if (newDir == facingDirection)
            return;

        facingDirection = newDir;

        Vector3 scale = visualsRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (int)facingDirection;
        visualsRoot.localScale = scale;
    }

    public CombatAdvantage GetCombatAdvantageAgainstPlayer()
    {
        if (stateMachine.CurrentState is EnemyStunState)
            return CombatAdvantage.PlayerAdvantage;

        return CombatAdvantage.Neutral;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        stateMachine.CurrentState?.OnCollisionEnter(collision);
    }

    #endregion
}
