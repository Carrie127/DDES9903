using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalTeddyInteraction : MonoBehaviour
{
    [Header("Teddy Glow")]
    public Light teddyGlow;
    public ClueLightPulse clueLightPulse;

    [Header("Teddy Audio")]
    public AudioSource teddyAudioSource;

    [Header("Timing")]
    public float glowFadeDuration = 0.8f;

    [Header("Clue State")]
    public UnityEvent onClueComplete;

    private bool hasBeenTriggered = false;


    public void InspectTeddy()
    {
        if (hasBeenTriggered)
            return;

        hasBeenTriggered = true;

        StartCoroutine(
            TeddySequence()
        );
    }


    private IEnumerator TeddySequence()
    {
        Debug.Log(
            "HOSPITAL TEDDY CLUE TRIGGERED"
        );

        // -------------------------------------------------
        // 1. Stop clue light pulse
        // -------------------------------------------------

        if (clueLightPulse != null)
        {
            clueLightPulse.StopPulse();
        }

        // -------------------------------------------------
        // 2. Start glow fade
        // -------------------------------------------------

        Coroutine glowCoroutine = null;

        if (teddyGlow != null)
        {
            glowCoroutine = StartCoroutine(
                FadeGlowOut()
            );
        }

        // -------------------------------------------------
        // 3. Play Teddy clue audio
        // -------------------------------------------------

        if (teddyAudioSource != null &&
            teddyAudioSource.clip != null)
        {
            teddyAudioSource.Stop();
            teddyAudioSource.loop = false;
            teddyAudioSource.Play();

            yield return new WaitWhile(
                () => teddyAudioSource.isPlaying
            );
        }

        // Make sure glow has also finished fading
        if (glowCoroutine != null)
        {
            yield return glowCoroutine;
        }

        // -------------------------------------------------
        // 4. Clue complete only after audio finishes
        // -------------------------------------------------

        Debug.Log(
            "HOSPITAL TEDDY CLUE COMPLETE"
        );

        onClueComplete?.Invoke();
    }


    private IEnumerator FadeGlowOut()
    {
        if (teddyGlow == null)
            yield break;

        float startIntensity =
            teddyGlow.intensity;

        if (glowFadeDuration <= 0f)
        {
            teddyGlow.intensity = 0f;
            teddyGlow.enabled = false;
            yield break;
        }

        float timer = 0f;

        while (timer < glowFadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / glowFadeDuration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            teddyGlow.intensity = Mathf.Lerp(
                startIntensity,
                0f,
                smoothT
            );

            yield return null;
        }

        teddyGlow.intensity = 0f;
        teddyGlow.enabled = false;
    }
}