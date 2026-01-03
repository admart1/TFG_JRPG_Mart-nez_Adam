using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour{ 
    [SerializeField] private string menuSceneName = "MainMenu";

    private void OnTriggerEnter2D(Collider2D other)
    {
    if (!other.CompareTag("PlayerTriggerMovement"))
        return;

        SceneManager.LoadScene(menuSceneName);
    }
}