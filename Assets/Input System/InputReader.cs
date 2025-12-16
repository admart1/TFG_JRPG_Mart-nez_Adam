using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private GameInputActions inputActions;

    public Vector2 Move { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool MenuPressed { get; private set; }
    public bool ChangeCharacterPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool SwordMenuPressed { get; private set; }
    public bool SwapSwordsPressed { get; private set; }
    public bool ToBattlePressed { get; private set; }

    private void Awake()
    {
        inputActions = new GameInputActions();

        inputActions.PlayerExploration.Enable();
    }

    private void Update()
    {
        Move = inputActions.PlayerExploration.Move.ReadValue<Vector2>();
        AttackPressed = inputActions.PlayerExploration.Attack.WasPressedThisFrame();
        DashPressed = inputActions.PlayerExploration.Dash.WasPressedThisFrame();
        MenuPressed = inputActions.PlayerExploration.Menu.WasPressedThisFrame();
        ChangeCharacterPressed = inputActions.PlayerExploration.ChangeCharacter.WasPressedThisFrame();
        SwordMenuPressed = inputActions.PlayerExploration.SwordMenu.WasPressedThisFrame();
        SwapSwordsPressed = inputActions.PlayerExploration.SwapActiveSword.WasPressedThisFrame();
        ToBattlePressed = inputActions.PlayerExploration.TransitionToBattle.WasPressedThisFrame();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }
}
