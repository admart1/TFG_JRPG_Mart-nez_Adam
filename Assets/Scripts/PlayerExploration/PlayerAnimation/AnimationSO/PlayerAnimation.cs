using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerAnimation",
    menuName = "Player Animation",
    order = 0)]
public class PlayerAnimation : ScriptableObject
{
    public string clipName;                // state + dirección (ej IdleN)
}
