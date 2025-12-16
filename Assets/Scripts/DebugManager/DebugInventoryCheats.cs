using UnityEngine;

public class DebugInventoryCheats : MonoBehaviour
{
    public InventoryController inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.AddSwordById("sw_yellow");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.AddSwordById("sw_green");
        }
    }
}
