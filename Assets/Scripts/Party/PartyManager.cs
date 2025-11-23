using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("Base de datos personajes")]
    public CharacterDatabase characterDatabase; // so con todos los characterdefinition

    [Header("Personaje activo")]
    public CharacterModel activeCharacter;

    private List<CharacterModel> runtimeCharacters = new List<CharacterModel>(); // lista de charactermodel creados, uno por cada characterdefinition de la db

    private void Awake()
    {
        InitializeCharacters();
    }

    private void InitializeCharacters()
    {
        if (characterDatabase == null || characterDatabase.characters.Length == 0)
        {
            Debug.LogWarning("no hay db o esta vacia");
            return;
        }

        runtimeCharacters.Clear();

        foreach (var characterDefinition in characterDatabase.characters)
        {
            CharacterModel model = new CharacterModel();
            model.InitializeCharacter(characterDefinition);
            runtimeCharacters.Add(model);
        }

        if (runtimeCharacters.Count > 0)
        {
            activeCharacter = runtimeCharacters[0];
        }
    }

    public void SetActiveCharacter(int index)
    {
        if (index < 0 || index >= runtimeCharacters.Count)
        {
            Debug.LogWarning("indice de personaje incorrecto");
            return;
        }

        activeCharacter = runtimeCharacters[index];
    }

    public void NextCharacter()
    {
        if (runtimeCharacters == null || runtimeCharacters.Count == 0) return;

        int currentIndex = runtimeCharacters.IndexOf(activeCharacter);

        int nextIndex = (currentIndex + 1) % runtimeCharacters.Count;

        activeCharacter = runtimeCharacters[nextIndex];
    }

    public List<CharacterModel> GetAllCharacters()
    {
        return runtimeCharacters;
    }
}