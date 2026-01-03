using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CombatManager;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public List<CharacterModel> PlayerParty { get; private set; }

    public EnemyGroupData CurrentEnemyGroup { get; private set; }
    public ExplorationEnemyBase CurrentEnemy { get; private set; }
    [SerializeField] public PlayerController playerController;

    [SerializeField] private GameObject explorationRoot;

    private bool combatSceneLoaded = false;
    public PartyManager partyManager;

    public CombatAdvantage CurrentCombatAdvantage { get; private set; }
    public Vector3 LastSpawnPoint;
    public Vector3 LastEnterCombatPoint;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerParty = new List<CharacterModel>();
    }

    void Start()
    {
        if (partyManager != null)
        {
            InitializeParty(partyManager);
        }
        else
        {
            Debug.LogWarning("GameSession: PartyManager no asignado");
        }
    }

    // REGISTRAR JUGADOR
    public void InitializeParty(PartyManager partyManager)
    {
        PlayerParty = new List<CharacterModel>(partyManager.GetAllCharacters());
    }

    // ENTRAR EN COMBATE
    public void StartCombat(ExplorationEnemyBase enemy, CombatAdvantage advantage)
    {
        if (combatSceneLoaded) return;
        if (enemy.enemyGroupData == null) return;

        LastEnterCombatPoint = playerController.transform.position;

        CurrentEnemy = enemy;
        CurrentEnemyGroup = enemy.enemyGroupData;
        CurrentCombatAdvantage = advantage;

        if (explorationRoot != null)
            explorationRoot.SetActive(false);

        SceneManager.LoadScene("CombatScene", LoadSceneMode.Additive);
        combatSceneLoaded = true;
    }

    // SALIR COMBATE
    public void EndCombat()
    {
        if (!combatSceneLoaded)
        {
            Debug.LogWarning("GameSession: No hay combate activo");
            return;
        }

        CurrentEnemyGroup = null;

        // descargar escena combate
        SceneManager.UnloadSceneAsync("CombatScene");

        // reactivar explo
        if (explorationRoot != null)
            explorationRoot.SetActive(true);

        // solucion temporal a que el menu de espadas aparezca abierto a salir de cobmate
        InventoryUIController inventoryUI = FindFirstObjectByType<InventoryUIController>();
        if (inventoryUI != null)
            inventoryUI.CloseMenu();

        PlayerHUDController hudUI = FindFirstObjectByType<PlayerHUDController>();
        hudUI.Refresh();

        StartCoroutine(UnloadCombatScene());
    }

    private IEnumerator UnloadCombatScene()
    {
        yield return SceneManager.UnloadSceneAsync("CombatScene");
        combatSceneLoaded = false;
    }

    public void ResolveCombatResult(CombatResult result)
    {
        switch (result)
        {
            case CombatResult.Victory:
                HandleVictory();
                break;

            case CombatResult.Defeat:
                HandleDefeat();
                break;

            case CombatResult.Escape:
                HandleEscape();
                break;
        }
    }

    private void HandleVictory()
    {
        CurrentEnemy.gameObject.SetActive(false);
        Debug.Log("victory");
    }

    private void HandleDefeat()
    {
        foreach (var member in PlayerParty)
        {
            member.currentHP = member.GetFinalStats().maxHP;
            member.currentMana = member.GetFinalStats().maxMana;
        }
        if (playerController != null)
        {
            Vector3 spawn = LastSpawnPoint != Vector3.zero ? LastSpawnPoint : Vector3.zero;
            playerController.transform.position = spawn;
        }
    }

    private void HandleEscape()
    {
        CurrentEnemy.stateMachine.ChangeState(CurrentEnemy.RecoveryState);

        if (playerController != null)
        {
            Vector3 spawn = LastEnterCombatPoint != Vector3.zero ? LastEnterCombatPoint : Vector3.zero;
            playerController.transform.position = spawn;
        }
    }
}
