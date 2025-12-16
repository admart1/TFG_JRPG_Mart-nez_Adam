using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplorationEnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 3;
    [HideInInspector] public int currentHealth;


    [Header("Referencias")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider2D collider;
    [HideInInspector] public Transform player;
    [HideInInspector] public EnemyAnimationController animController;
    [HideInInspector] public EnemyStateMachine stateMachine;

    [Header("Estados")]
    [HideInInspector] public EnemyState IdleState;
    [HideInInspector] public EnemyState ChaseState;
    [HideInInspector] public EnemyState AttackState;
    [HideInInspector] public EnemyState RecoveryState;

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
        var playerObj = GameObject.FindGameObjectWithTag("PlayerEnemiesTarget");
        if (playerObj != null)
            player = playerObj.transform;

        // DATOS
        wallLayerMask = LayerMask.GetMask("Ground_0");
    }
    #endregion


    #region UPDATES
    protected void Update()
    {
        stateMachine?.CurrentState?.LogicUpdate();
    }

    protected void FixedUpdate()
    {
        stateMachine?.CurrentState?.PhysicsUpdate();
    }
    #endregion


    #region DAMAGE SYSTEM

    public virtual void TakeDamage(int damage, Vector2 hitPos, GameObject source)
    {
        currentHealth -= damage;

        OnDamage(damage, hitPos, source);

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void OnDamage(int damage, Vector2 hitPos, GameObject source)
    {
        animController?.PlayHurt();
        ApplyKnockback(source.transform.position);
    }

    protected virtual void Die()
    {
        animController?.PlayDeath();

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;

        Destroy(gameObject, 0.5f); //tiempo
    }

    #endregion

    #region ENEMY HUD

    private void UpdateHealthBar()
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

    #region KNOCKBACK
    protected virtual void ApplyKnockback(Vector2 attackerPos)
    {
        if (rb == null) return;

        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;

        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine(dir));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockback = true;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            transform.position += (Vector3)(direction * knockbackForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockback = false;
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

    #endregion
}
