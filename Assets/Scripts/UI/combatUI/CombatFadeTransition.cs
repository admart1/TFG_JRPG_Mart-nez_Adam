using UnityEngine;
using System.Collections;

public class CombatFadeTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public float fadeDuration = 1f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeRoutine(1f, 0f)); 
    }

    public void StartFadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(0f, 1f, onComplete));
    }

    private IEnumerator FadeRoutine(float from, float to, System.Action onComplete = null)
    {
        float timer = 0f;
        canvasGroup.alpha = from;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        onComplete?.Invoke();
    }
}
