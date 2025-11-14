using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F1;
    public bool showDebug = true;
    public Vector2 position = new Vector2(10, 10);


    
    private string facingDirection;
    [SerializeField] private PlayerFacing playerFacing;

    [SerializeField] public PlayerController player;
    private string currentStateName;
    void Update()
    {

        if (Input.GetKeyDown(toggleKey))
            showDebug = !showDebug;

        if (playerFacing != null)
            facingDirection = playerFacing.facingDirection.ToString();
        else facingDirection = "no hay direccion";



        // HACER LA STATEMACHINE PRIVADA DESPUES DE TESTEAR
        if (player.stateMachine != null && player.stateMachine.CurrentState != null)
        {
            currentStateName = player.stateMachine.CurrentState.GetType().Name;
        }
        else
        {
            currentStateName = "no hay state activo";
        }
    }


    void OnGUI()
    {
        if (!showDebug) return;

        GUI.Label(new Rect(position.x, position.y, 300, 20), $"Dirección MC: {facingDirection}");
        GUI.Label(new Rect(position.x, position.y + 20, 300, 20), $"Estado: {currentStateName}");
    }
}