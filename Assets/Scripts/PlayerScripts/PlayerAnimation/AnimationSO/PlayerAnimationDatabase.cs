using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimationDatabase", menuName = "Animations/PlayerAnimationDatabase")]
public class PlayerAnimationDatabase : ScriptableObject
{
    public enum PlayerState
    {
        Idle,
        Movement,
        Attack,
        Attack2,
        Attack3
    }

    [System.Serializable]
    public class AnimationEntry : DBDisplayName
    {
        public PlayerState state;
        public PlayerFacing.FacingDirection direction;
        public string animationName;                    // nombre del clip
    }

    public AnimationEntry[] animations;


    public string GetAnimationName(PlayerState state, PlayerFacing.FacingDirection direction)
    {
        foreach (var entry in animations)
        {
            if (entry.state == state && entry.direction == direction)
            {
                return entry.animationName;
            }
        }
        return null;
    }

    // crea el nombre automaticamente a partir de state+direccion
    private void OnValidate()
    {
        if (animations == null) return;

        foreach (var entry in animations)
        {
            // nombre generado con state + direction
            entry.animationName = entry.state.ToString() + entry.direction.ToString();
        }
    }
}

