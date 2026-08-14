using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WristbandInteraction : MonoBehaviour
{
    [Header("Wristband Glow")]
    public Light wristbandGlow;
    public ClueLightPulse clueLightPulse;

    [Header("Wristband Audio")]
    public AudioSource wristbandAudioSource;

    [Header("Timing")]
    public float glowFadeDuration = 0.8f;

    [Header("Clue State")]
    public UnityEvent onClueComplete;

    private bool hasBeenTriggered = false;


    public void InspectWristband()
    {
        if (hasBeenTriggered)
            return;

        hasBeenTriggered = true;

        StartCoroutine(
            WristbandSequence()
        );
    }


    private IEnumerator WristbandSequence()
    {
        Debug.Log(
            "WRISTBAND CLUE TRIGGERED"
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

        if (wristbandGlow != null)
        {
            glowCoroutine = StartCoroutine(
                FadeGlowOut()
            );
        }

        // -------------------------------------------------
        // 3. Play Wristband clue audio
        // -------------------------------------------------

        if (wristbandAudioSource != null &&
            wristbandAudioSource.clip != null)
        {
            wristbandAudioSource.Stop();
            wristbandAudioSource.loop = false;
            wristbandAudioSource.Play();

            yield return new WaitWhile(
                () => wristbandAudioSource.isPlaying
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
            "WRISTBAND CLUE COMPLETE"
        );

        onClueComplete?.Invoke();
    }


    private IEnumerator FadeGlowOut()
    {
        if (wristbandGlow == null)
            yield break;

        float startIntensity =
            wristbandGlow.intensity;

        if (glowFadeDuration <= 0f)
        {
            wristbandGlow.intensity = 0f;
            wristbandGlow.enabled = false;
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

            wristbandGlow.intensity = Mathf.Lerp(
                startIntensity,
                0f,
                smoothT
            );

            yield return null;
        }

        wristbandGlow.intensity = 0f;
        wristbandGlow.enabled = false;
    }
}