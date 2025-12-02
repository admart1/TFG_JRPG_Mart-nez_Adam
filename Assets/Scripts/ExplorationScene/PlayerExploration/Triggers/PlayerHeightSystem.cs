using UnityEngine;

public class PlayerHeightSystem : MonoBehaviour
{
    [Header("Current Height")]
    public int currentHeight = 0;

    [Header("Height Layers")]
    public string[] heightLayerNames;

    [Header("Movement Collider")]
    public GameObject movementCollider;

    public void SetHeight(int newHeight)
    {
        currentHeight = Mathf.Clamp(newHeight, 0, heightLayerNames.Length - 1);
        ApplyHeightLayer();
    }

    private void ApplyHeightLayer()
    {
        if (movementCollider == null) return;
        if (currentHeight < 0 || currentHeight >= heightLayerNames.Length) return;

        int layerIndex = LayerMask.NameToLayer(heightLayerNames[currentHeight]);
        if (layerIndex == -1) return;

        movementCollider.layer = layerIndex;
    }

    public void DisableCollisions()
    {
        movementCollider.layer = LayerMask.NameToLayer("Player_No_Collisions");
    }

    public void EnableCollisions()
    {
        ApplyHeightLayer();
    }
    /*
    public void EnterRamp()
    {
        if (movementCollider == null) return;

        int layerIndex = LayerMask.NameToLayer("Player_Ramp");
        if (layerIndex == -1) return;

        movementCollider.layer = layerIndex;
    }

    public void ExitRamp()
    {
        ApplyHeightLayer();
    }*/
}
