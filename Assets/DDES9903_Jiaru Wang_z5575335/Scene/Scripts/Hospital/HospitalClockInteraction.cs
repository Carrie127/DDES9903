using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalClockInteraction : MonoBehaviour
{
    [Header("Exit Sign Guidance")]
    public Light exitSignGlow;
    public ClueLightPulse exitSignPulse;

    [Header("Clock Guidance Audio")]
    [Tooltip("The looping electronic guidance sound played before the player interacts with the clock.")]
    public AudioSource clockGuidanceSource;

    [Header("Clock Memory Audio Source")]
    [Tooltip("One shared AudioSource used to play all Clock memory voice clips in sequence.")]
    public AudioSource memoryAudioSource;

    [Header("Common Memory Clips")]
    [Tooltip("Common Clock memory played for both routes.")]
    public AudioClip commonMemoryClip;

    [Tooltip("Young Evie trying to speak but unable to form words.")]
    public AudioClip youngEvieStruggleClip;

    [Header("Route Ending Clips")]
    [Tooltip("Present Evie response when Hospital was visited first.")]
    public AudioClip hospitalFirstEndingClip;

    [Tooltip("Present Evie response when Orphanage was visited first.")]
    public AudioClip orphanageFirstEndingClip;

    [Header("Timing")]
    public float signFadeDuration = 0.8f;

    [Tooltip("Pause between the common memory and Young Evie's struggle.")]
    public float gapBeforeStruggle = 0.15f;

    [Tooltip("Pause between Young Evie's struggle and Present Evie's response.")]
    public float gapBeforeEndingReaction = 0.25f;

    [Header("Clock State")]
    public UnityEvent onClockActivated;

    private bool hasBeenActivated = false;


    // =====================================================
    // CLOCK INTERACTION
    // =====================================================

    public void ActivateClock()
    {
        if (hasBeenActivated)
            return;

        hasBeenActivated = true;

        StartCoroutine(
            ClockSequence()
        );
    }


    // =====================================================
    // MAIN CLOCK SEQUENCE
    // =====================================================

    private IEnumerator ClockSequence()
    {
        Debug.Log(
            "HOSPITAL CLOCK 7:42 ACTIVATED"
        );

        // -------------------------------------------------
        // 1. Stop the looping Clock guidance sound
        // -------------------------------------------------

        if (clockGuidanceSource != null)
        {
            clockGuidanceSource.Stop();
        }

        // -------------------------------------------------
        // 2. Stop the exit-sign pulse
        // -------------------------------------------------

        if (exitSignPulse != null)
        {
            exitSignPulse.StopPulse();
        }

        // -------------------------------------------------
        // 3. Fade the exit-sign guidance light out
        // -------------------------------------------------

        if (exitSignGlow != null)
        {
            yield return StartCoroutine(
                FadeExitSignOut()
            );
        }

        // -------------------------------------------------
        // 4. Common memory
        // -------------------------------------------------

        yield return PlayClip(
            commonMemoryClip
        );

        // -------------------------------------------------
        // 5. Young Evie struggles to speak
        // -------------------------------------------------

        if (gapBeforeStruggle > 0f)
        {
            yield return new WaitForSeconds(
                gapBeforeStruggle
            );
        }

        yield return PlayClip(
            youngEvieStruggleClip
        );

        // -------------------------------------------------
        // 6. Short pause before Present Evie's reaction
        // -------------------------------------------------

        if (gapBeforeEndingReaction > 0f)
        {
            yield return new WaitForSeconds(
                gapBeforeEndingReaction
            );
        }

        // -------------------------------------------------
        // 7. Route-specific Present Evie response
        // -------------------------------------------------

        AudioClip endingClip = null;

        if (MemoryRouteState.IsHospitalFirst())
        {
            endingClip =
                hospitalFirstEndingClip;

            Debug.Log(
                "HOSPITAL CLOCK ENDING → HOSPITAL FIRST"
            );
        }
        else if (MemoryRouteState.IsOrphanageFirst())
        {
            endingClip =
                orphanageFirstEndingClip;

            Debug.Log(
                "HOSPITAL CLOCK ENDING → ORPHANAGE FIRST"
            );
        }
        else
        {
            Debug.LogWarning(
                "HOSPITAL CLOCK: MEMORY ROUTE HAS NOT BEEN SET."
            );
        }

        yield return PlayClip(
            endingClip
        );

        // -------------------------------------------------
        // 8. Memory sequence complete
        //    Continue the existing door-opening event
        // -------------------------------------------------

        Debug.Log(
            "HOSPITAL CLOCK MEMORY COMPLETE → OPEN EXIT"
        );

        onClockActivated?.Invoke();
    }


    // =====================================================
    // PLAY ONE CLIP USING THE SHARED AUDIO SOURCE
    // =====================================================

    private IEnumerator PlayClip(
        AudioClip clip
    )
    {
        if (memoryAudioSource == null)
        {
            Debug.LogWarning(
                "HOSPITAL CLOCK: Memory Audio Source is missing."
            );

            yield break;
        }

        if (clip == null)
        {
            yield break;
        }

        memoryAudioSource.Stop();

        memoryAudioSource.clip = clip;
        memoryAudioSource.loop = false;

        memoryAudioSource.Play();

        yield return new WaitWhile(
            () => memoryAudioSource.isPlaying
        );
    }


    // =====================================================
    // EXIT SIGN FADE
    // =====================================================

    private IEnumerator FadeExitSignOut()
    {
        if (exitSignGlow == null)
            yield break;

        float startIntensity =
            exitSignGlow.intensity;

        if (signFadeDuration <= 0f)
        {
            exitSignGlow.intensity = 0f;
            exitSignGlow.enabled = false;

            yield break;
        }

        float timer = 0f;

        while (timer < signFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / signFadeDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            exitSignGlow.intensity =
                Mathf.Lerp(
                    startIntensity,
                    0f,
                    smoothT
                );

            yield return null;
        }

        exitSignGlow.intensity = 0f;
        exitSignGlow.enabled = false;
    }
}