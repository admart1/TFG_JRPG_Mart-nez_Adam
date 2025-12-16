using UnityEngine;
using System.Collections;

public enum CombatAdvantage
{
    Neutral,   
    PlayerAdvantage,    
    EnemyAdvantage
}

public class CombatTransitionManager : MonoBehaviour
{
    public static CombatTransitionManager Instance { get; private set; }

    [Header("Transición")]
    [SerializeField] private float transitionDuration = 1f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartCombat(GameObject player, ExplorationEnemyBase enemy, CombatAdvantage advantageType)
    {
        if (isTransitioning || player == null || enemy == null)
            return;

        StartCoroutine(TransitionRoutine(player, enemy, advantageType));
    }

    private IEnumerator TransitionRoutine(GameObject player, ExplorationEnemyBase enemy, CombatAdvantage advantageType)
    {
        isTransitioning = true;

        // bloquear controles del jugador

        // animacion al combat.e..
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Iniciar el combate
        Debug.Log($"Iniciando combate con {enemy.name}, ventaja: {advantageType}");
        // CombatManager.Instance.StartCombat(player, enemy, advantage); o algo asi

        isTransitioning = false;
    }
}
