using System.Collections;
using UnityEngine;

public class BedroomDoorSequence : MonoBehaviour
{
    [Header("Door")]
    public Transform doorTransform;

    [Header("Door Audio")]
    public AudioSource doorAudioSource;
    public AudioClip doorCloseClip;

    [Header("Rotation")]
    public float closedYRotation = 0f;

    [Header("Door Timing")]
    public float delayBeforeClose = 0f;
    public float closeDuration = 1.2f;

    [Header("Memory Atmosphere")]
    public OrphanageMemoryAmbience warmMemoryAmbience;
    public AudioSource childrenBackgroundSource;
    public float atmosphereFadeDuration = 2.0f;

    [Header("Clock")]
    public Light clockGlow;
    public AudioSource clockTickingSource;

    public float delayBeforeClock = 0.5f;
    public float clockGlowFadeDuration = 1.5f;

    private bool hasClosed = false;

    private float clockGlowTargetIntensity = 1f;
    private float childrenOriginalVolume = 1f;

    private void Start()
    {
        // ---------------------------------------------
        // Clock starts hidden
        // ---------------------------------------------

        if (clockGlow != null)
        {
            clockGlowTargetIntensity =
                clockGlow.intensity;

            clockGlow.intensity = 0f;
        }

        // ---------------------------------------------
        // Remember children ambience volume
        // ---------------------------------------------

        if (childrenBackgroundSource != null)
        {
            childrenOriginalVolume =
                childrenBackgroundSource.volume;
        }

        // ---------------------------------------------
        // Clock ticking must NOT start automatically
        // ---------------------------------------------

        if (clockTickingSource != null)
        {
            clockTickingSource.Stop();
        }
    }

    // =================================================
    // PUBLIC START
    // =================================================

    public void StartDoorClose()
    {
        if (hasClosed)
            return;

        hasClosed = true;

        StartCoroutine(
            CloseDoorSequence()
        );
    }

    // =================================================
    // MAIN SEQUENCE
    // =================================================

    private IEnumerator CloseDoorSequence()
    {
        // ---------------------------------------------
        // 1. Optional pause before door closes
        // ---------------------------------------------

        if (delayBeforeClose > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeClose
            );
        }

        if (doorTransform == null)
        {
            Debug.LogWarning(
                "BedroomDoorSequence: Door Transform is missing!"
            );

            yield break;
        }

        // ---------------------------------------------
        // 2. Door sound + door movement begin together
        // ---------------------------------------------

        if (doorAudioSource != null &&
            doorCloseClip != null)
        {
            doorAudioSource.clip =
                doorCloseClip;

            doorAudioSource.Play();
        }

        Quaternion startRotation =
            doorTransform.localRotation;

        Vector3 targetEuler =
            doorTransform.localEulerAngles;

        targetEuler.y =
            closedYRotation;

        Quaternion targetRotation =
            Quaternion.Euler(
                targetEuler
            );

        float timer = 0f;

        while (timer < closeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / closeDuration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            doorTransform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    smoothT
                );

            yield return null;
        }

        doorTransform.localRotation =
            targetRotation;

        Debug.Log("BEDROOM DOOR CLOSED");

        // ---------------------------------------------
        // 3. Old warm atmosphere begins disappearing
        // ---------------------------------------------

        if (warmMemoryAmbience != null)
        {
            warmMemoryAmbience.FadeOut();
        }

        if (childrenBackgroundSource != null)
        {
            StartCoroutine(
                FadeAudioSource(
                    childrenBackgroundSource,
                    childrenBackgroundSource.volume,
                    0f,
                    atmosphereFadeDuration
                )
            );
        }

        // ---------------------------------------------
        // 4. Short unsettling pause
        // ---------------------------------------------

        if (delayBeforeClock > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeClock
            );
        }

        // ---------------------------------------------
        // 5. Clock ticking begins
        // ---------------------------------------------

        if (clockTickingSource != null)
        {
            clockTickingSource.Play();
        }

        // ---------------------------------------------
        // 6. Clock Glow appears
        // ---------------------------------------------

        if (clockGlow != null)
        {
            yield return StartCoroutine(
                FadeClockGlowIn()
            );
        }

        Debug.Log(
            "CLOCK GUIDANCE STARTED"
        );
    }

    // =================================================
    // CLOCK GLOW
    // =================================================

    private IEnumerator FadeClockGlowIn()
    {
        if (clockGlow == null)
            yield break;

        if (clockGlowFadeDuration <= 0f)
        {
            clockGlow.intensity =
                clockGlowTargetIntensity;

            yield break;
        }

        float timer = 0f;

        while (timer < clockGlowFadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / clockGlowFadeDuration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            clockGlow.intensity =
                Mathf.Lerp(
                    0f,
                    clockGlowTargetIntensity,
                    smoothT
                );

            yield return null;
        }

        clockGlow.intensity =
            clockGlowTargetIntensity;
    }

    // =================================================
    // AUDIO FADE
    // =================================================

    private IEnumerator FadeAudioSource(
        AudioSource source,
        float from,
        float to,
        float duration
    )
    {
        if (source == null)
            yield break;

        if (duration <= 0f)
        {
            source.volume = to;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            source.volume =
                Mathf.Lerp(
                    from,
                    to,
                    smoothT
                );

            yield return null;
        }

        source.volume = to;
    }
}