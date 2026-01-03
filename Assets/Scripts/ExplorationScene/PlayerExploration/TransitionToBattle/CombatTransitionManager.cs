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

    public void StartCombat(ExplorationEnemyBase enemy, CombatAdvantage advantageType)
    {
        if (isTransitioning || enemy == null)
            return;

        isTransitioning = true;

        GameSession.Instance.StartCombat(enemy, advantageType);

        isTransitioning = false;
    }
}
