using System.Collections;
using UnityEngine;

public class OrphanageMemoryAmbience : MonoBehaviour
{
    [Header("Warm Memory Audio")]
    public AudioSource warmMemorySource;

    [Header("Fade Settings")]
    public float targetVolume = 0.4f;
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (warmMemorySource == null)
        {
            warmMemorySource = GetComponent<AudioSource>();
        }

        if (warmMemorySource != null)
        {
            warmMemorySource.volume = 0f;

            if (!warmMemorySource.isPlaying)
            {
                warmMemorySource.Play();
            }
        }
    }

    public void FadeIn()
    {
        if (warmMemorySource == null)
            return;

        if (!warmMemorySource.isPlaying)
        {
            warmMemorySource.Play();
        }

        StartFade(targetVolume, fadeInDuration);
    }

    public void FadeOut()
    {
        if (warmMemorySource == null)
            return;

        StartFade(0f, fadeOutDuration);
    }

    private void StartFade(float target, float duration)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadeVolume(target, duration)
        );
    }

    private IEnumerator FadeVolume(float target, float duration)
    {
        float startVolume = warmMemorySource.volume;
        float timer = 0f;

        if (duration <= 0f)
        {
            warmMemorySource.volume = target;
            yield break;
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            warmMemorySource.volume =
                Mathf.Lerp(startVolume, target, smoothT);

            yield return null;
        }

        warmMemorySource.volume = target;
        fadeCoroutine = null;
    }
}