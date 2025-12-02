using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    private float distanceToFall;
    private float distanceCovered;
    private Vector2 fallDirection;
    public int targetHeight;

    public override void Enter()
    {
        player.heightSystem.DisableCollisions();

        distanceCovered = 0f;

        if (!fallDistances.TryGetValue((player.heightSystem.currentHeight, targetHeight), out distanceToFall))
        {
            Debug.LogWarning("Combinación de altura sin distancia definida");
            distanceToFall = 0.32f; // un tile por defecto
        }

        fallDirection = Vector2.down;

        player.Rigidbody.linearVelocity = Vector2.zero;

        player.animationController.PlayAnimation(
            "Dash",
            player.playerFacing.facingDirection
        );
    }

    public override void HandleInput() { }

    public override void PhysicsUpdate()
    {
        player.Rigidbody.linearVelocity = fallDirection * player.MovementSpeed;

        distanceCovered += player.MovementSpeed * Time.fixedDeltaTime;

        if (distanceCovered >= distanceToFall)
        {
            // Al finalizar la caída
            player.heightSystem.SetHeight(targetHeight);
            player.heightSystem.EnableCollisions();
            player.Rigidbody.linearVelocity = Vector2.zero;

            stateMachine.ChangeState(player.IdleState);
        }
    }

    // Distancias (tile = 0.32 unidades) numero de tiles * 0.32 y luego margen dee 16?
    Dictionary<(int, int), float> fallDistances = new Dictionary<(int, int), float>()
    {
        { (1, 0), 0.70f },    // de 1 a 0
        { (2, 1), 0.32f },    // de 2 a 1
        { (2, 0), 0.64f },    // de 2 a 0
        { (5, 0), 1.6f  },    // de 5 a 0
        { (5, 1), 1.28f },    // de 5 a 1
        { (5, 2), 0.96f },    // de 5 a 2
    };
}
