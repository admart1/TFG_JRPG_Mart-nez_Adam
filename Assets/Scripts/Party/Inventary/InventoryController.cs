using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Database")]
    public SwordDatabase swordDatabase;

    [Header("Inventary")]
    [SerializeField] private InventoryModel inventory = new InventoryModel();

    // Evento
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (swordDatabase == null)
            Debug.LogWarning("InventoryController: No hay SwordDatabase asignada.");
    }

    //     MÉTODOS 
    public void AddSword(EquipableSword sword)
    {
        if (sword == null)
        {
            Debug.LogWarning("InventoryController: intento de añadir una espada NULL.");
            return;
        }

        if (!inventory.ownedSwords.Contains(sword))
        {
            inventory.ownedSwords.Add(sword);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.Log($"InventoryController: La espada '{sword.displayName}' ya está en inventario.");
        }
    }

    public void AddSwordById(string swordId)
    {
        EquipableSword sword = swordDatabase?.GetSwordById(swordId);

        if (sword == null)
        {
            Debug.LogWarning($"InventoryController: No existe espada con ID '{swordId}'.");
            return;
        }

        AddSword(sword);
    }

    public void RemoveSword(EquipableSword sword)
    {
        if (inventory.ownedSwords.Remove(sword))
        {
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning("InventoryController: La espada no estaba en inventario.");
        }
    }

    public List<EquipableSword> GetOwnedSwords()
    {
        return inventory.ownedSwords;
    }

    public bool EquipSwordToSlot(EquipableSword sword, CharacterModel character, int slot)
    {
        if (sword == null || character == null)
        {
            Debug.LogWarning("EquipSwordToSlot: sword o character es NULL.");
            return false;
        }

        EquipableSword previousSword = null;

        if (slot == 1)
        {
            previousSword = character.SwordSlot1;
            character.SwordSlot1 = sword;
        }
        else if (slot == 2)
        {
            previousSword = character.SwordSlot2;
            character.SwordSlot2 = sword;
        }
        else
        {
            Debug.LogWarning("EquipSwordToslot: slot inválido (no es 1 o 2).");
            return false;
        }

        /*if (previousSword != null && !inventory.ownedSwords.Contains(previousSword))
        {
            inventory.ownedSwords.Add(previousSword);
        }*/

        OnInventoryChanged?.Invoke();
        return true;
    }
}
