using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private Vector2 detectionOffset = new Vector2(0f, 0.5f);
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("UI")]
    [SerializeField] private GameObject selectionIconPrefab;


    private readonly List<ExplorationEnemyBase> detectedEnemies = new();
    private ExplorationEnemyBase selectedEnemy;
    private GameObject selectionIconInstance;

    public ExplorationEnemyBase SelectedEnemy => selectedEnemy;
    private ExplorationEnemyBase lastSelectedEnemy;

    private void Update()
    {
        DetectEnemies();
        ShowEnemySelection();
        ShowEnemyHealthBars();
    }

    private void DetectEnemies()
    {
        detectedEnemies.Clear();
        Vector2 center = (Vector2)transform.position + detectionOffset;

        Collider2D[] results = Physics2D.OverlapCircleAll(center, detectionRadius, enemyLayerMask);
        foreach (var col in results)
        {
            ExplorationEnemyBase enemy = col.GetComponentInParent<ExplorationEnemyBase>();
            if (enemy != null && !detectedEnemies.Contains(enemy))
            {
                detectedEnemies.Add(enemy);
            }
        }
    }

    private void ShowEnemySelection()
    {
        if (detectedEnemies.Count == 0)
        {
            // desactivar
            if (lastSelectedEnemy != null)
            {
                lastSelectedEnemy.ShowSelectionIcon(false);
                lastSelectedEnemy = null;
            }
            selectedEnemy = null;
            return;
        }

        // enemigo mas cercano
        float minDist = float.MaxValue;
        ExplorationEnemyBase closest = null;

        foreach (var enemy in detectedEnemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        selectedEnemy = closest;

        // desactivar
        if (lastSelectedEnemy != null && lastSelectedEnemy != selectedEnemy)
        {
            lastSelectedEnemy.ShowSelectionIcon(false);
        }

        // activar
        if (selectedEnemy != null)
        {
            selectedEnemy.ShowSelectionIcon(true);
            lastSelectedEnemy = selectedEnemy;
        }
    }

    private void ShowEnemyHealthBars()
    {
        // mostar
       /* foreach (var enemy in detectedEnemies)
        {
            enemy.ShowHealthBar(true);
        }*/

        // ocultar
        var allEnemies = FindObjectsByType<ExplorationEnemyBase>(
            FindObjectsInactive.Exclude,   
            FindObjectsSortMode.None      
        );

        foreach (var enemy in allEnemies)
        {
            if (!detectedEnemies.Contains(enemy))
                enemy.ShowHealthBar(false);
        }
    }

    public ExplorationEnemyBase GetSelectedEnemy()
    {
        return selectedEnemy;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Gizmos.DrawWireSphere(center, detectionRadius);
    }
#endif
}
