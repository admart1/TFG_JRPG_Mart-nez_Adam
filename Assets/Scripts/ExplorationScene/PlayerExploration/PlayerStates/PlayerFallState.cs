using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public enum FallOrigin { FromMovement, FromDash }
    public enum FallCardinalDir {  North, South, East, West }

    private FallOrigin fallOrigin;
    private FallCardinalDir fallCardinal;

    private Vector2 initialHorizontalDir;

    private float distanceToFall;
    private float distanceCovered;
    private Vector2 fallDirection;
    public int targetHeight;

    private float fallSpeed = 4f;

    private bool inUpPhase;
    private float upPhaseTimer;

    public void SetupFall(int targetHeight, Vector2 horizontalDir, FallOrigin origin)
    {
        this.targetHeight = targetHeight;
        this.initialHorizontalDir = horizontalDir;
        this.fallOrigin = origin;
    }

    public override void Enter()
    {
        player.heightSystem.DisableCollisions();

        distanceCovered = 0f;

        GetFallCardinal(player.triggerDetector.currentTrigger);

        if (!fallDistances.TryGetValue((player.heightSystem.currentHeight, targetHeight, fallCardinal), out distanceToFall))
        {
            Debug.LogWarning("Combinación de altura sin distancia definida");
            distanceToFall = 0.32f;
        }

        player.Rigidbody.linearVelocity = Vector2.zero;

        if (fallOrigin == FallOrigin.FromDash)
        {
            fallDirection = Vector2.down;
            inUpPhase = false;
        }
        else
        {
            inUpPhase = true; upPhaseTimer = 0.08f;  //tiempo sbuida 
            
            float horizontalFactor = 0.3f; 
            fallDirection = (Vector2.up * 0.35f + initialHorizontalDir * horizontalFactor).normalized;
        }

        player.animationController.PlayAnimation(
            "Dash",
            player.playerFacing.facingDirection
        );
    }


    public override void HandleInput() { }

    public override void PhysicsUpdate()
    {
        // fase 1 de la caída (salto
        if (inUpPhase)
        {
            upPhaseTimer -= Time.fixedDeltaTime;

            player.Rigidbody.linearVelocity = fallDirection * (fallSpeed * 0.4f);

            if (upPhaseTimer <= 0)
            {
                // transición a caída normal
                inUpPhase = false;

                float horizontalFactor;

                Debug.Log("fall origin" + fallOrigin);

                if (targetHeight == 0 && player.heightSystem.currentHeight == 2)
                {
                    horizontalFactor = 0.2f;
                }
                else
                {
                    horizontalFactor = 0.4f;
                }

                fallDirection = (Vector2.down + initialHorizontalDir * horizontalFactor).normalized;
            }

            return;
        }

        // fase 2 de la caída
        
        if (fallCardinal == FallCardinalDir.North || fallCardinal == FallCardinalDir.West)
            player.heightSystem.isFalling = true;

        player.Rigidbody.linearVelocity = fallDirection * fallSpeed;

        distanceCovered += fallSpeed * Time.fixedDeltaTime;    

        if (distanceCovered >= distanceToFall)
        {
            player.heightSystem.SetHeight(targetHeight);
         
            player.heightSystem.EnableCollisions();
            player.Rigidbody.linearVelocity = Vector2.zero;

            stateMachine.ChangeState(player.IdleState);
            player.heightSystem.isFalling = false;
        }
    }

    private void GetFallCardinal(WorldTrigger ledge)
    {
        if (fallOrigin == FallOrigin.FromDash)
        {
            WorldTrigger dashTrigger = player.DashState.bufferedTrigger;
            if (dashTrigger.fallToEast)
                fallCardinal = FallCardinalDir.East;

            if (dashTrigger.fallToWest)
                fallCardinal = FallCardinalDir.West;

            if (dashTrigger.fallToNorth)
                fallCardinal = FallCardinalDir.North;

            if (dashTrigger.fallToSouth)
                fallCardinal = FallCardinalDir.South;
        }
        else
        {
            if (ledge.fallToEast)
                fallCardinal = FallCardinalDir.East;

            if (ledge.fallToWest)
                fallCardinal = FallCardinalDir.West;

            if (ledge.fallToNorth)
                fallCardinal = FallCardinalDir.North;

            if (ledge.fallToSouth)
                fallCardinal = FallCardinalDir.South;
        }
    }

    // Distancias (tile = 0.32 unidades) numero de tiles * 0.32 y luego margen dee 16?
    Dictionary<(int, int, FallCardinalDir), float> fallDistances = new Dictionary<(int, int, FallCardinalDir), float>()
    {
    // 1 → 0
    { (1, 0, FallCardinalDir.East), 1.2f },
    { (1, 0, FallCardinalDir.West), 1.2f },
    { (1, 0, FallCardinalDir.North), 1f },
    { (1, 0, FallCardinalDir.South), 1.7f },

    // 2 → 0
    { (2, 0, FallCardinalDir.East), 2.4f },
    { (2, 0, FallCardinalDir.West), 2.4f },
    { (2, 0, FallCardinalDir.North), 2f },
    { (2, 0, FallCardinalDir.South), 3.2f },

    // 2 → 1
    { (2, 1, FallCardinalDir.East), 1.2f },
    { (2, 1, FallCardinalDir.West), 1.2f },
    { (2, 1, FallCardinalDir.North), 1f },
    { (2, 1, FallCardinalDir.South), 1.7f },

    // 5 → 0
    { (5, 0, FallCardinalDir.East), 1.98f },
    { (5, 0, FallCardinalDir.West), 1.98f },
    { (5, 0, FallCardinalDir.North), 1.40f },
    { (5, 0, FallCardinalDir.South), 1.40f },

    // 5 → 1
    { (5, 1, FallCardinalDir.East), 1.66f },
    { (5, 1, FallCardinalDir.West), 1.66f },
    { (5, 1, FallCardinalDir.North), 1.20f },
    { (5, 1, FallCardinalDir.South), 1.20f },

    // 5 → 2
    { (5, 2, FallCardinalDir.East), 1.34f },
    { (5, 2, FallCardinalDir.West), 1.34f },
    { (5, 2, FallCardinalDir.North), 1.00f },
    { (5, 2, FallCardinalDir.South), 1.00f },
    };
}
