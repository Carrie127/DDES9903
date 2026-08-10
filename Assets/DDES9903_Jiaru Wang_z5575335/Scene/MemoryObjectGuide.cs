using System.Collections;
using UnityEngine;

public class MemoryObjectGuide : MonoBehaviour
{
    [Header("References")]
    public Holdable holdable;
    public Light objectGlow;
    public Light targetGlow;
    public AudioSource pickupHintAudio;

    [Header("Fade Settings")]
    public float fadeDuration = 1.0f;

    private bool wasMoving = false;
    private bool pickupTriggered = false;
    private float targetOriginalIntensity = 1f;

    private void Start()
    {
        if (holdable == null)
        {
            holdable = GetComponent<Holdable>();
        }

        if (targetGlow != null)
        {
            targetOriginalIntensity = targetGlow.intensity;
            targetGlow.intensity = 0f;
        }
    }

    private void Update()
    {
        if (holdable == null) return;

        bool isMovingNow = holdable.moving;

        if (!pickupTriggered && !wasMoving && isMovingNow)
        {
            pickupTriggered = true;
            StartCoroutine(OnFirstPickup());
        }

        wasMoving = isMovingNow;
    }

    private IEnumerator OnFirstPickup()
    {
        if (objectGlow != null)
        {
            yield return StartCoroutine(
                FadeLight(objectGlow, objectGlow.intensity, 0f, fadeDuration)
            );
        }

        if (pickupHintAudio != null)
        {
            pickupHintAudio.Play();
        }

        if (targetGlow != null)
        {
            yield return StartCoroutine(
                FadeLight(targetGlow, 0f, targetOriginalIntensity, fadeDuration)
            );
        }
    }

    private IEnumerator FadeLight(
        Light lightSource,
        float from,
        float to,
        float duration
    )
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            lightSource.intensity = Mathf.Lerp(from, to, t);

            yield return null;
        }

        lightSource.intensity = to;
    }
}