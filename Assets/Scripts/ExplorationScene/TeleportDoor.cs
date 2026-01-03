using UnityEngine;
public class TeleportDoor : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private Transform destination;
    [SerializeField] private GameObject player;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("activation 1");
        if (!other.CompareTag("PlayerTriggerMovement")) return;

        Debug.Log("activation 2");
        Teleport(player);
    }

    private void Teleport(GameObject player)
    {
        player.transform.position = destination.position;

            SnapCamera();
    }

    private void SnapCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;

        cam.transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            cam.transform.position.z
        );
    }
}
