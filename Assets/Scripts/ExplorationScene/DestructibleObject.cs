using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [SerializeField] private SwordType requiredSwordType;
    [SerializeField] private GameObject newObject;

    [Header("Feedback")]
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private Color hitFlashColorEfective = Color.white;
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float destroyFlashDuration = 0.15f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Vector2 hitPos, GameObject source)
    {
        SwordType swordType = GetSwordTypeFromSource(source);

        if (swordType != requiredSwordType)
        {
            PlayHitFlash(hitFlashColor);
            return;
        }

        PlayHitFlash(hitFlashColorEfective);

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private SwordType GetSwordTypeFromSource(GameObject source)
    {
        var player = source.GetComponentInParent<PlayerController>();
        if (player == null)
            return SwordType.Standard;

        var character = player.character;

        if (character.activeSword == ActiveSword.Slot1)
            return character.SwordSlot1.swordType;

        if (character.activeSword == ActiveSword.Slot2)
            return character.SwordSlot2.swordType;

        return SwordType.Standard;
    }

    private void Die()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        StartCoroutine(DestroyFlashAndDie());
    }

    private void PlayHitFlash(Color color)
    {
        if (spriteRenderer == null) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash(color, hitFlashDuration));
    }

    private IEnumerator DestroyFlashAndDie()
    {
        yield return Flash(Color.white, destroyFlashDuration);

        if (newObject != null)
            newObject.SetActive(true);

        gameObject.SetActive(false);
    }

    private IEnumerator Flash(Color flashColor, float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSecondsRealtime(duration);
        spriteRenderer.color = originalColor;
    }
}
