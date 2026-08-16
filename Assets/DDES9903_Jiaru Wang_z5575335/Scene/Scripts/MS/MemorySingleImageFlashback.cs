using System.Collections;
using UnityEngine;

public class MemorySingleImageFlashback : MonoBehaviour
{
    [Header("Memory Image")]
    [SerializeField] private CanvasGroup memoryImage;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.7f;
    [SerializeField] private float holdDuration = 8.9f;
    [SerializeField] private float fadeOutDuration = 0.7f;

    private Coroutine flashbackCoroutine;

    private void Awake()
    {
        if (memoryImage != null)
        {
            memoryImage.alpha = 0f;
            memoryImage.interactable = false;
            memoryImage.blocksRaycasts = false;
        }
    }

    public void PlayFlashback()
    {
        if (flashbackCoroutine != null)
            StopCoroutine(flashbackCoroutine);

        flashbackCoroutine = StartCoroutine(FlashbackSequence());
    }

    private IEnumerator FlashbackSequence()
    {
        if (memoryImage == null)
            yield break;

        // Fade in
        yield return FadeCanvasGroup(
            memoryImage,
            memoryImage.alpha,
            1f,
            fadeInDuration
        );

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        yield return FadeCanvasGroup(
            memoryImage,
            memoryImage.alpha,
            0f,
            fadeOutDuration
        );

        flashbackCoroutine = null;
    }

    private IEnumerator FadeCanvasGroup(
        CanvasGroup canvasGroup,
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}