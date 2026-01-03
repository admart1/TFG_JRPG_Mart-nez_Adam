using System.Collections.Generic;
using UnityEngine;

public class ExplorationAttackHitbox : MonoBehaviour
{
    [Header("Referencias")]
    private BoxCollider2D hitbox;
    private PlayerFacing playerFacing;

    [System.Serializable]
    public struct DirectionalHitbox
    {
        public Vector2 offset;
        public Vector2 size;
    }

    [Header("Hitbox por dirección")]
    public DirectionalHitbox North;
    public DirectionalHitbox South;
    public DirectionalHitbox East;
    public DirectionalHitbox West;
    public DirectionalHitbox NorthEast;
    public DirectionalHitbox NorthWest;
    public DirectionalHitbox SouthEast;
    public DirectionalHitbox SouthWest;

    [Header("Layers golpeables")]
    public LayerMask hitMask;

    [Header("Daño")]
    public int damage = 1;

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    private bool active = false;

    public void Activate() => EnableHitbox();
    public void Deactivate() => DisableHitbox();

    Vector2 lastOffset;


    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.enabled = false;

        playerFacing = GetComponentInParent<PlayerFacing>();
    }

    private void Update()
    {
        lastOffset = hitbox.offset;
    }

    public void EnableHitbox()
    {
        ApplyHitboxData();

        hitTargets.Clear();
        active = true;
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        active = false;
        hitbox.enabled = false;
    }

    private void ApplyHitboxData()
    {
        if (playerFacing == null) return;

        DirectionalHitbox data = GetDirectionalHitbox();

        hitbox.offset = data.offset;
        hitbox.size = data.size;
    }

    private DirectionalHitbox GetDirectionalHitbox()
    {
        switch (playerFacing.facingDirection)
        {
            case PlayerFacing.FacingDirection.North: return North;
            case PlayerFacing.FacingDirection.South: return South;
            case PlayerFacing.FacingDirection.East: return East;
            case PlayerFacing.FacingDirection.West: return West;
            case PlayerFacing.FacingDirection.NorthEast: return NorthEast;
            case PlayerFacing.FacingDirection.NorthWest: return NorthWest;
            case PlayerFacing.FacingDirection.SouthEast: return SouthEast;
            case PlayerFacing.FacingDirection.SouthWest: return SouthWest;
            default: return North;
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
            Vector2 hitPos = other.ClosestPoint(hitbox.bounds.center);
            damageable.TakeDamage(damage, hitPos, gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (hitbox == null) return;

        Gizmos.color = Color.yellow;
        Vector3 center = transform.TransformPoint(lastOffset);
        Gizmos.DrawWireCube(center, hitbox.size);
    }
}
