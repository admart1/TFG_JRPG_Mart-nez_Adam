using UnityEngine;

public class FollowCameraDebug : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F1;
    private SpriteRenderer sr;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogWarning("No hay SpriteRenderer, el objeto no será visible");
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && sr != null)
        {
            sr.enabled = !sr.enabled;
        }
    }

    private void LateUpdate()
    {
        if (cam == null || sr == null || !sr.enabled) return;

        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}
