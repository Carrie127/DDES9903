using System.Collections;
using UnityEngine;

public class ClockInteraction : MonoBehaviour
{
    [Header("Clock Guidance")]
    public Light clockGlow;
    public AudioSource clockTickingSource;

    [Header("Clock Memory Audio Sources")]
    public AudioSource presentEvieSource;
    public AudioSource memoryVoiceSource;
    public AudioSource memoryEffectSource;

    [Header("7:42 Common Memory Clips")]
    public AudioClip evieClock742;
    public AudioClip memoryChildrenAlarmed;
    public AudioClip mia742Fragment;

    [Header("Route Ending Audio")]
    public AudioSource endingEvieSource;

    [Tooltip("Used when Orphanage is the first memory scene.")]
    public AudioClip orphanageFirstEndingClip;

    [Tooltip("Leave empty until the Hospital-First ending audio is created.")]
    public AudioClip hospitalFirstEndingClip;

    [Header("Scene Transition")]
    public MemorySceneTransition sceneTransition;

    [Tooltip("Temporary or final Hospital scene name.")]
    public string hospitalSceneName =
        "Scene_Hospital";

    [Tooltip("Memory Space scene name.")]
    public string memorySpaceSceneName =
        "Scene_MemorySpace";

    [Header("Clock Timing")]
    public float clockFadeDuration = 0.8f;
    public float gapAfterEvie742 = 0.4f;

    [Header("Children Alarmed Layer")]
    public float childrenInitialVolume = 0.7f;
    public float childrenBackgroundVolume = 0.18f;

    public float childrenFadeToBackgroundDuration =
        0.8f;

    public float childrenFinalFadeOutDuration =
        1.0f;

    public float loudChildrenDuration =
        1.2f;

    [Header("Ending Timing")]
    public float delayBeforeEndingReaction =
        0.5f;

    [Range(0f, 1f)]
    [Tooltip(
        "At what percentage of the final Evie audio " +
        "the white transition should begin. " +
        "0.5 means halfway."
    )]
    public float transitionStartPoint =
        0.5f;

    private bool activated = false;

    // =====================================================
    // PUBLIC INTERACTION
    // =====================================================

    public void ActivateClock()
    {
        if (activated)
            return;

        activated = true;

        Debug.Log(
            "CLOCK 7:42 ACTIVATED!"
        );

        StartCoroutine(
            ClockMemorySequence()
        );
    }

    // =====================================================
    // MAIN CLOCK SEQUENCE
    // =====================================================

    private IEnumerator ClockMemorySequence()
    {
        // -------------------------------------------------
        // 1. Clock Glow fades out
        // -------------------------------------------------

        if (clockGlow != null)
        {
            StartCoroutine(
                FadeLightOut(
                    clockGlow,
                    clockFadeDuration
                )
            );
        }

        // -------------------------------------------------
        // 2. Clock ticking fades out
        // -------------------------------------------------

        if (clockTickingSource != null &&
            clockTickingSource.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudioOut(
                    clockTickingSource,
                    clockFadeDuration,
                    true
                )
            );
        }

        // -------------------------------------------------
        // 3. Present Evie reacts to 7:42
        // -------------------------------------------------

        yield return PlayClip(
            presentEvieSource,
            evieClock742
        );

        if (gapAfterEvie742 > 0f)
        {
            yield return new WaitForSeconds(
                gapAfterEvie742
            );
        }

        // -------------------------------------------------
        // 4. Children Alarmed bursts in
        // -------------------------------------------------

        if (memoryEffectSource != null &&
            memoryChildrenAlarmed != null)
        {
            memoryEffectSource.clip =
                memoryChildrenAlarmed;

            memoryEffectSource.loop = true;

            memoryEffectSource.volume =
                childrenInitialVolume;

            memoryEffectSource.Play();
        }

        if (loudChildrenDuration > 0f)
        {
            yield return new WaitForSeconds(
                loudChildrenDuration
            );
        }

        // -------------------------------------------------
        // 5. Children Alarmed becomes background
        // -------------------------------------------------

        if (memoryEffectSource != null)
        {
            yield return StartCoroutine(
                FadeAudioToVolume(
                    memoryEffectSource,
                    childrenBackgroundVolume,
                    childrenFadeToBackgroundDuration
                )
            );
        }

        // -------------------------------------------------
        // 6. Mia calls Evie
        // -------------------------------------------------

        yield return PlayClip(
            memoryVoiceSource,
            mia742Fragment
        );

        // -------------------------------------------------
        // 7. Children background fades away
        // -------------------------------------------------

        if (memoryEffectSource != null &&
            memoryEffectSource.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudioOut(
                    memoryEffectSource,
                    childrenFinalFadeOutDuration,
                    true
                )
            );
        }

        Debug.Log(
            "CLOCK COMMON MEMORY COMPLETE"
        );

        // -------------------------------------------------
        // 8. Brief pause before Ending Evie
        // -------------------------------------------------

        if (delayBeforeEndingReaction > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeEndingReaction
            );
        }

        // -------------------------------------------------
        // 9. Route-specific Ending
        //    + white transition begins DURING dialogue
        // -------------------------------------------------

        yield return StartCoroutine(
            PlayRouteEndingWithTransition()
        );

        Debug.Log(
            "ORPHANAGE CLOCK ENDING COMPLETE"
        );
    }

    // =====================================================
    // ROUTE-SPECIFIC ENDING + TRANSITION
    // =====================================================

    private IEnumerator PlayRouteEndingWithTransition()
    {
        // -------------------------------------------------
        // ORPHANAGE FIRST
        //
        // Archive
        // → Orphanage
        // → Hospital
        // -------------------------------------------------

        if (MemoryRouteState.IsOrphanageFirst())
        {
            Debug.Log(
                "ORPHANAGE ENDING ROUTE: Orphanage First"
            );

            yield return StartCoroutine(
                PlayEndingAudioWithWhiteTransition(
                    orphanageFirstEndingClip,
                    hospitalSceneName
                )
            );
        }

        // -------------------------------------------------
        // HOSPITAL FIRST
        //
        // Archive
        // → Hospital
        // → Orphanage
        // → Memory Space
        // -------------------------------------------------

        else if (MemoryRouteState.IsHospitalFirst())
        {
            Debug.Log(
                "ORPHANAGE ENDING ROUTE: Hospital First"
            );

            if (hospitalFirstEndingClip != null)
            {
                yield return StartCoroutine(
                    PlayEndingAudioWithWhiteTransition(
                        hospitalFirstEndingClip,
                        memorySpaceSceneName
                    )
                );
            }
            else
            {
                Debug.LogWarning(
                    "Hospital-First Orphanage ending audio " +
                    "has not been created yet."
                );
            }
        }

        else
        {
            Debug.LogWarning(
                "ClockInteraction: No Memory Route is currently set."
            );
        }
    }

    // =====================================================
    // PLAY FINAL EVIE AUDIO
    // WHITE SCREEN BEGINS DURING THE AUDIO
    // =====================================================

    private IEnumerator PlayEndingAudioWithWhiteTransition(
        AudioClip endingClip,
        string targetScene
    )
    {
        if (endingEvieSource == null)
        {
            Debug.LogWarning(
                "ClockInteraction: Ending Evie Source is missing!"
            );

            yield break;
        }

        if (endingClip == null)
        {
            Debug.LogWarning(
                "ClockInteraction: Ending Audio Clip is missing!"
            );

            yield break;
        }

        if (sceneTransition == null)
        {
            Debug.LogWarning(
                "ClockInteraction: MemorySceneTransition is missing!"
            );

            yield break;
        }

        // ---------------------------------------------
        // Start Ending Evie audio
        // ---------------------------------------------

        endingEvieSource.clip =
            endingClip;

        endingEvieSource.loop =
            false;

        endingEvieSource.Play();

        // ---------------------------------------------
        // Calculate when white fade should begin
        // ---------------------------------------------

        float clipLength =
            endingClip.length;

        float transitionStartTime =
            clipLength *
            Mathf.Clamp01(
                transitionStartPoint
            );

        // Wait until selected point in dialogue
        yield return new WaitForSeconds(
            transitionStartTime
        );

        // ---------------------------------------------
        // Calculate how much dialogue remains
        // ---------------------------------------------

        float remainingAudioTime =
            Mathf.Max(
                0f,
                clipLength -
                transitionStartTime
            );

        Debug.Log(
            "WHITE TRANSITION STARTED DURING ENDING AUDIO"
        );

        // ---------------------------------------------
        // Begin white fade now,
        // but don't load the new scene until
        // the remaining dialogue has had time to finish.
        // ---------------------------------------------

        sceneTransition.StartTransitionTo(
            targetScene,
            remainingAudioTime
        );

        // ---------------------------------------------
        // Continue waiting for Evie audio to actually end
        // ---------------------------------------------

        yield return new WaitWhile(
            () => endingEvieSource != null &&
                  endingEvieSource.isPlaying
        );
    }

    // =====================================================
    // NORMAL AUDIO
    // =====================================================

    private IEnumerator PlayClip(
        AudioSource source,
        AudioClip clip
    )
    {
        if (source == null ||
            clip == null)
        {
            yield break;
        }

        source.clip =
            clip;

        source.loop =
            false;

        source.Play();

        yield return new WaitWhile(
            () => source.isPlaying
        );
    }

    // =====================================================
    // AUDIO → TARGET VOLUME
    // =====================================================

    private IEnumerator FadeAudioToVolume(
        AudioSource source,
        float targetVolume,
        float duration
    )
    {
        if (source == null)
            yield break;

        float startVolume =
            source.volume;

        if (duration <= 0f)
        {
            source.volume =
                targetVolume;

            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
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
                    startVolume,
                    targetVolume,
                    smoothT
                );

            yield return null;
        }

        source.volume =
            targetVolume;
    }

    // =====================================================
    // AUDIO FADE OUT
    // =====================================================

    private IEnumerator FadeAudioOut(
        AudioSource source,
        float duration,
        bool stopAfterFade
    )
    {
        if (source == null)
            yield break;

        float startVolume =
            source.volume;

        if (duration <= 0f)
        {
            source.volume = 0f;

            if (stopAfterFade)
            {
                source.Stop();
            }

            source.volume =
                startVolume;

            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
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
                    startVolume,
                    0f,
                    smoothT
                );

            yield return null;
        }

        source.volume = 0f;

        if (stopAfterFade)
        {
            source.Stop();
        }

        source.volume =
            startVolume;
    }

    // =====================================================
    // CLOCK LIGHT FADE OUT
    // =====================================================

    private IEnumerator FadeLightOut(
        Light lightSource,
        float duration
    )
    {
        if (lightSource == null)
            yield break;

        float startIntensity =
            lightSource.intensity;

        if (duration <= 0f)
        {
            lightSource.intensity =
                0f;

            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            lightSource.intensity =
                Mathf.Lerp(
                    startIntensity,
                    0f,
                    smoothT
                );

            yield return null;
        }

        lightSource.intensity =
            0f;
    }
}