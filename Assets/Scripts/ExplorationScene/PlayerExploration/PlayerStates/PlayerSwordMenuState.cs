using UnityEngine;

public class PlayerSwordMenuState : PlayerState
{
    private PlayerState previousState;

    public override void Enter()
    {
        previousState = stateMachine.PreviousState;

        base.Enter();

        player.inventoryUI.ToggleMenu();
        Time.timeScale = 0f;
    }

    public override void HandleInput()
    {
        if (player.input.SwordMenuPressed)
        {
            stateMachine.ChangeState(previousState);
        }

        if (player.input.SwapSwordsPressed)
        {
            player.character.SwitchActiveSword();
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.inventoryUI.ToggleMenu();

        Time.timeScale = 1f;
    }
}
