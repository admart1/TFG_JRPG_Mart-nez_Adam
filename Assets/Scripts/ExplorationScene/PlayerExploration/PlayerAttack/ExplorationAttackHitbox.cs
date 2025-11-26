using System.Collections.Generic;
using UnityEngine;

public class ExplorationAttackHitbox : MonoBehaviour
{
    private BoxCollider2D hitbox;
    private PlayerFacing playerFacing;

    [Header("Offset general")]
    [Tooltip("Debido a estar usando un addon para exportar de forma automatica desde aseprite, el pivot de player no es correcto asi que hay que determinar un offset concreto de x0.005 e y0.698")]
    public Vector2 baseCenterOffset = new Vector2(0.005f, 0.698f);


    [Header("Offsets por dirección")]
    public Vector2 North = new Vector2(0f, 0.45f);
    public Vector2 South = new Vector2(0f, -0.45f);
    public Vector2 East = new Vector2(0.45f, 0f);
    public Vector2 West = new Vector2(-0.45f, 0f);
    public Vector2 NorthEast = new Vector2(0.225f, 0.225f);
    public Vector2 SouthEast = new Vector2(0.225f, -0.225f);
    public Vector2 SouthWest = new Vector2(-0.225f, -0.225f);
    public Vector2 NorthWest = new Vector2(-0.225f, 0.225f);

    [Header("Layers golpeables")]
    public LayerMask hitMask;

    [Header("Daño")]
    public int damage = 1;      // mas adelante recibira el daño concreto a través de attackstate, o quiza daño fijo..

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    private bool active = false;

    public void Activate() => EnableHitbox();
    public void Deactivate() => DisableHitbox();

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();

        hitbox.enabled = false;

        playerFacing = GetComponentInParent<PlayerFacing>();
    }

    public void EnableHitbox()
    {
        UpdateHitboxPosition();

        hitTargets.Clear();
        active = true;
        hitbox.enabled = true;
        Debug.Log("Hitbox activada en posición: " + transform.localPosition);
    }

    public void DisableHitbox()
    {
        active = false;
        hitbox.enabled = false;
        Debug.Log("Hitbox desactivada");
    }

    private void UpdateHitboxPosition()
    {
        if (playerFacing == null) return;

        Vector2 finalOffset = baseCenterOffset + GetDirectionalOffset();
        transform.localPosition = new Vector3(finalOffset.x, finalOffset.y, transform.localPosition.z);
    }

    private Vector2 GetDirectionalOffset()
    {
        switch (playerFacing.facingDirection)
        {
            case PlayerFacing.FacingDirection.North: return North;
            case PlayerFacing.FacingDirection.NorthEast: return NorthEast;
            case PlayerFacing.FacingDirection.East: return East;
            case PlayerFacing.FacingDirection.SouthEast: return SouthEast;
            case PlayerFacing.FacingDirection.South: return South;
            case PlayerFacing.FacingDirection.SouthWest: return SouthWest;
            case PlayerFacing.FacingDirection.West: return West;
            case PlayerFacing.FacingDirection.NorthWest: return NorthWest;

            default: return Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !hitTargets.Contains(damageable))
        {
            hitTargets.Add(damageable);
            Vector2 hitPos = other.ClosestPoint(transform.position);
            damageable.TakeDamage(damage, hitPos, gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (hitbox == null) return;

        Gizmos.color = hitbox.enabled ? Color.green : Color.red;

        Gizmos.DrawWireCube(
            transform.position + (Vector3)hitbox.offset,
            hitbox.size) ;
    }
}