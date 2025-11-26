using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    public int health = 3;

    public void TakeDamage(int damage, Vector2 hitPos, GameObject source)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de {source.name}. Salud restante: {health}");

        // efectos visuales??

        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} ha sido destruido.");
            Destroy(gameObject);
        }
    }
}
