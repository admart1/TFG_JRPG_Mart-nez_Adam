using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Character Definition", fileName = "NewCharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    // ================================
    // DATOS ESTÁTICOS
    // ================================
    [Header("Identidad")]
    public string characterId;          
    public string displayName;       

    [Header("Stats Base")]                  // ambos son SO
    public BaseStats baseStats;         
    public LevelProgression levelProgression; 

    [Header("Visual")]                      // placeholder
    public Sprite portrait;                 
    public RuntimeAnimatorController overworldAnimator;

    [Header("Empty sword TEMPORAL")]
    public EquipableSword emptySword;

    // ================================
    // DATOS DINÁMICOS (a falta de un sistema de guardado)
    // ================================
    [Header("Nivel y estado inicial")]
    public int startingLevel = 1;            
    public int startingHP = 10;
    public int startingMana = 10;


    [Header("Equipo inicial")]
    public EquipableSword startingSword1; 
    public EquipableSword startingSword2;   
    public ActiveSword activeSword = ActiveSword.Slot1;
}