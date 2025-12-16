using System.Collections.Generic;

[System.Serializable]
public class InventoryModel
{
    // Listas de items (SO) poseídos (slo espadas ahrao) 
    public List<EquipableSword> ownedSwords = new List<EquipableSword>();
}