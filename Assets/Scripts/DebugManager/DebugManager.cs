using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F1;
    public bool showDebug = true;
    public Vector2 position = new Vector2(10, 10);

    [SerializeField] private PlayerController player;

    private string facingDirection;
    private string currentStateName;
    private string activeCharacterName;
    private string sword1Name;
    private string sword2Name;
    private string currentAnimation;
    private int currentLayer;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            showDebug = !showDebug;

        // dirección
        if (player != null && player.playerFacing != null)
            facingDirection = player.playerFacing.facingDirection.ToString();
        else
            facingDirection = "No hay dirección";

        // estado actual
        if (player != null && player.stateMachine != null && player.stateMachine.CurrentState != null)
            currentStateName = player.stateMachine.CurrentState.GetType().Name;
        else
            currentStateName = "No hay state activo";

        // personaje activo
        if (player != null && player.partyManager != null && player.partyManager.activeCharacter != null)
        {
            var activeChar = player.partyManager.activeCharacter;

            // personaje
            activeCharacterName = activeChar.definition != null ? activeChar.definition.displayName : "Sin definición";

            // espadas
            sword1Name = activeChar.SwordSlot1 != null ? activeChar.SwordSlot1.displayName : "Vacía";
            sword2Name = activeChar.SwordSlot2 != null ? activeChar.SwordSlot2.displayName : "Vacía";
        }
        else
        {
            activeCharacterName = "No hay character activo";
            sword1Name = "-";
            sword2Name = "-";
        }

        // animación
        if (player != null && player.animationController != null)
        {
            currentAnimation = player.animationController.GetCurrentAnimationName();
        }

        if (player != null && player.animationController != null)
        {
            currentLayer = player.heightSystem.movementCollider.layer;
        }
    }

    void OnGUI()
    {
        if (!showDebug) return;

        float lineHeight = 20f;
        float x = position.x;
        float y = position.y;

        GUI.Label(new Rect(x, y, 400, lineHeight), $"Dirección MC: {facingDirection}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Estado: {currentStateName}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Personaje activo: {activeCharacterName}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Espada Slot1: {sword1Name}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Espada Slot2: {sword2Name}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Animación: {currentAnimation}");
        y += lineHeight;
        GUI.Label(new Rect(x, y, 400, lineHeight), $"Layer player: {currentLayer}");
    }
}
