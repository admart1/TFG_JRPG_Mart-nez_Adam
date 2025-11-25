using UnityEngine;

// NOMENCLATURA: characterId_state_direction_layer_swordId

public class PlayerAnimationController : MonoBehaviour
{
    [Header("PlayerController")]
    public PlayerController player;

    [Header("Animators")]
    public Animator bodyAnimator;
    public Animator armAnimator;
    public Animator sword1Animator;
    public Animator sword2Animator;

    [Header("SpriteRenderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer armRenderer;
    public SpriteRenderer sword1Renderer;
    public SpriteRenderer sword2Renderer;


    private string currentAnimationNameBody;
    private string currentAnimationNameArm;
    private string currentAnimationNameSword1;
    private string currentAnimationNameSword2;
    private float currentNormalizedTime;

    public void PlayAnimation(string state, PlayerFacing.FacingDirection direction)
    {
        UpdateLayerOrder(direction);

        string characterID = player.partyManager.activeCharacter.definition.characterId;
        string swordID = player.partyManager.activeCharacter.SwordSlot1.swordId;
        string sword2ID = player.partyManager.activeCharacter.SwordSlot2.swordId;
        
        string bodyClip = GenerateClipName(characterID, state, direction, "Body", "0");
        string armClip = GenerateClipName(characterID, state, direction, "Arm", "0");
        string sword1Clip = GenerateClipName(characterID, state, direction, "Sword", swordID);
        string sword2Clip = GenerateClipName(characterID, state, direction, "Sword", sword2ID);

        // se guarda el tiempo en el que estaba la animación si es movement
        float normalizedTime = 0f;
        if (state == "Movement")
        {
            normalizedTime = currentNormalizedTime;
        }

        // si es la misma animación no se reproduce
        if (currentAnimationNameBody != bodyClip ||
            currentAnimationNameArm != armClip ||
            currentAnimationNameSword1 != sword1Clip ||
            currentAnimationNameSword2 != sword2Clip)
        {
            if (bodyAnimator != null) bodyAnimator.Play(bodyClip, 0, normalizedTime);
            if (armAnimator != null) armAnimator.Play(armClip, 0, normalizedTime);

            if (player.partyManager.activeCharacter.SwordSlot1.swordId == "None")
            {
                sword1Renderer.enabled = false;
            }
            else
            {
                sword1Renderer.enabled = true;
                if (sword1Animator != null)
                    sword1Animator.Play(sword1Clip, 0, normalizedTime);
            }

            if (player.partyManager.activeCharacter.SwordSlot2.swordId == "None")
            {
                sword2Renderer.enabled = false;
                // sword2Animator.Play("Empty", 0, normalizedTime);
            }
            else
            {
                sword2Renderer.enabled = true;
                if (sword2Animator != null)
                    sword2Animator.Play(sword2Clip, 0, normalizedTime);
            }

            currentAnimationNameBody = bodyClip;
            currentAnimationNameArm = armClip;
            currentAnimationNameSword1 = sword1Clip;
            currentAnimationNameSword2 = sword2Clip;
            currentNormalizedTime = normalizedTime;
        }
    }

    private string GenerateClipName(string character, string state, PlayerFacing.FacingDirection direction, string layer, string swordID)
    {
        return $"{character}_{state}_{direction}_{layer}_{swordID}";
    }

    public string GetCurrentAnimationName()
    {
        return $"Body: {currentAnimationNameBody}, Arm: {currentAnimationNameArm}, Sword1: {currentAnimationNameSword1}, Sword2: {currentAnimationNameSword2}";
    }

    #region UpdateLayerOrder
    private void UpdateLayerOrder(PlayerFacing.FacingDirection direction)
    {
        switch (direction)
        {
            case PlayerFacing.FacingDirection.North:
                bodyRenderer.sortingOrder = 2;
                armRenderer.sortingOrder = 1;
                sword1Renderer.sortingOrder = 3;
                sword2Renderer.sortingOrder = 4;
                break;

            case PlayerFacing.FacingDirection.NorthEast:
                bodyRenderer.sortingOrder = 1;
                armRenderer.sortingOrder = 2;
                sword1Renderer.sortingOrder = 3;
                sword2Renderer.sortingOrder = 4;
                break;

            case PlayerFacing.FacingDirection.East:
                bodyRenderer.sortingOrder = 3;
                armRenderer.sortingOrder = 4;
                sword1Renderer.sortingOrder = 2;
                sword2Renderer.sortingOrder = 1;
                break;

            case PlayerFacing.FacingDirection.SouthEast:
                bodyRenderer.sortingOrder = 3;
                armRenderer.sortingOrder = 4;
                sword1Renderer.sortingOrder = 2;
                sword2Renderer.sortingOrder = 1;
                break;

            case PlayerFacing.FacingDirection.South:
                bodyRenderer.sortingOrder = 3;
                armRenderer.sortingOrder = 4;
                sword1Renderer.sortingOrder = 2;
                sword2Renderer.sortingOrder = 1;
                break;

            case PlayerFacing.FacingDirection.SouthWest:
                bodyRenderer.sortingOrder = 4;
                armRenderer.sortingOrder = 3;
                sword1Renderer.sortingOrder = 2;
                sword2Renderer.sortingOrder = 1;
                break;

            case PlayerFacing.FacingDirection.West:
                bodyRenderer.sortingOrder = 4;
                armRenderer.sortingOrder = 1;
                sword1Renderer.sortingOrder = 3;
                sword2Renderer.sortingOrder = 2;
                break;

            case PlayerFacing.FacingDirection.NorthWest:
                bodyRenderer.sortingOrder = 1;
                armRenderer.sortingOrder = 2;
                sword1Renderer.sortingOrder = 3;
                sword2Renderer.sortingOrder = 4;
                break;
        }
    }
    #endregion

}

