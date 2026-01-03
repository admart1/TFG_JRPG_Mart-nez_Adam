using UnityEngine;

public class RecoveryPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHurt"))
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player == null) return;

            HealPlayer(player);
            SaveSpawnPoint(player.transform.position);
        }
    }

    private void HealPlayer(PlayerController player)
    {
        foreach (var character in GameSession.Instance.PlayerParty)
        {
            character.currentHP = character.GetFinalStats().maxHP;
            character.currentMana = character.GetFinalStats().maxMana;
        }

        var hud = FindFirstObjectByType<PlayerHUDController>();
        hud?.Refresh();

        Debug.Log("spawn guardado");
    }

    private void SaveSpawnPoint(Vector3 position)
    {
        GameSession.Instance.LastSpawnPoint = position;
    }
}