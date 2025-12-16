using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public List<CharacterModel> PlayerParty { get; private set; }

    public EnemyGroupData CurrentEnemyGroup { get; private set; }

    [SerializeField] private GameObject explorationRoot;

    private bool combatSceneLoaded = false;

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

    // REGISTRAR JUGADOR
    public void RegisterPlayer(CharacterModel model)
    {
        if (model == null)
        {
            Debug.LogError("GameSession: Intentando registrar CharacterModel null");
            return;
        }

        if (!PlayerParty.Contains(model))
        {
            PlayerParty.Add(model);
        }
    }

    // ENTRAR EN COMBATE
    public void StartCombat(EnemyGroupData enemyGroup)
    {
        if (combatSceneLoaded)
        {
            Debug.LogWarning("GameSession: Combate ya activo");
            return;
        }

        if (enemyGroup == null)
        {
            Debug.LogError("GameSession: EnemyGroupData es null");
            return;
        }

        CurrentEnemyGroup = enemyGroup;

        // desactivar explo
        if (explorationRoot != null)
            explorationRoot.SetActive(false);
        else
            Debug.LogWarning("GameSession: explorationRoot no asignado");

        // cargar escena de combate
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

        combatSceneLoaded = false;
    }
}
