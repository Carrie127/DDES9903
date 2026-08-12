using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MemoryObjectGuide : MonoBehaviour
{
    [Header("References")]
    public Holdable holdable;
    public Light objectGlow;
    public Light targetGlow;

    [Header("Object Glow Start")]
    [Tooltip("Turn this on if this object's glow should be hidden at scene start and activated later by the previous memory stage.")]
    public bool hideObjectGlowOnStart = false;

    [Header("Target Glow Timing")]
    [Tooltip("How long to wait after the object glow fades before the target glow appears.")]
    public float targetGlowDelay = 1.0f;

    [Header("Fade Settings")]
    public float fadeDuration = 1.0f;

    [Header("First Pickup Event")]
    [Tooltip("Called once when this memory object is picked up for the first time.")]
    public UnityEvent onFirstPickup;

    private bool wasMoving = false;
    private bool pickupTriggered = false;

    private float objectOriginalIntensity = 1f;
    private float targetOriginalIntensity = 1f;

    private Coroutine objectGlowCoroutine;

    private void Start()
    {
        if (holdable == null)
        {
            holdable = GetComponent<Holdable>();
        }

        // Remember Object Glow's intended visible intensity.
        if (objectGlow != null)
        {
            objectOriginalIntensity = objectGlow.intensity;

            // Later-stage objects should begin hidden.
            if (hideObjectGlowOnStart)
            {
                objectGlow.intensity = 0f;
            }
        }

        // Target Glow should always begin hidden.
        if (targetGlow != null)
        {
            targetOriginalIntensity = targetGlow.intensity;
            targetGlow.intensity = 0f;
        }
    }

    private void Update()
    {
        if (holdable == null)
            return;

        bool isMovingNow = holdable.moving;

        // Detect the first real pickup.
        if (!pickupTriggered && !wasMoving && isMovingNow)
        {
            pickupTriggered = true;
            StartCoroutine(OnFirstPickup());
        }

        wasMoving = isMovingNow;
    }

    private IEnumerator OnFirstPickup()
    {
        Debug.Log(gameObject.name + " FIRST PICKUP");

        // 1. Fade current object guidance away.
        if (objectGlow != null)
        {
            yield return StartCoroutine(
                FadeLight(
                    objectGlow,
                    objectGlow.intensity,
                    0f,
                    fadeDuration
                )
            );
        }

        // 2. Trigger pickup audio / memory continuation.
        onFirstPickup?.Invoke();

        // 3. Wait before showing the correct placement target.
        if (targetGlowDelay > 0f)
        {
            yield return new WaitForSeconds(targetGlowDelay);
        }

        // 4. Fade target guidance in.
        if (targetGlow != null)
        {
            yield return StartCoroutine(
                FadeLight(
                    targetGlow,
                    targetGlow.intensity,
                    targetOriginalIntensity,
                    fadeDuration
                )
            );
        }
    }

    public void FadeInObjectGlow()
    {
        if (objectGlow == null)
            return;

        if (objectGlowCoroutine != null)
        {
            StopCoroutine(objectGlowCoroutine);
        }

        objectGlowCoroutine = StartCoroutine(
            FadeObjectGlowIn()
        );
    }

    private IEnumerator FadeObjectGlowIn()
    {
        yield return StartCoroutine(
            FadeLight(
                objectGlow,
                objectGlow.intensity,
                objectOriginalIntensity,
                fadeDuration
            )
        );

        objectGlowCoroutine = null;
    }

    private IEnumerator FadeLight(
        Light lightSource,
        float from,
        float to,
        float duration
    )
    {
        if (lightSource == null)
            yield break;

        if (duration <= 0f)
        {
            lightSource.intensity = to;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            lightSource.intensity = Mathf.Lerp(
                from,
                to,
                smoothT
            );

            yield return null;
        }

        lightSource.intensity = to;
    }
}