using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WristbandInteraction : MonoBehaviour
{
    [Header("Wristband Glow")]
    public Light wristbandGlow;
    public ClueLightPulse clueLightPulse;

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

        StartCoroutine(WristbandSequence());
    }

    private IEnumerator WristbandSequence()
    {
        Debug.Log("WRISTBAND CLUE TRIGGERED");

        // ---------------------------------------------
        // 1. Stop pulse first
        // ---------------------------------------------
        if (clueLightPulse != null)
        {
            clueLightPulse.StopPulse();
        }

        // ---------------------------------------------
        // 2. Fade glow from its current intensity
        // ---------------------------------------------
        if (wristbandGlow != null)
        {
            float startIntensity = wristbandGlow.intensity;
            float timer = 0f;

            if (glowFadeDuration <= 0f)
            {
                wristbandGlow.intensity = 0f;
            }
            else
            {
                while (timer < glowFadeDuration)
                {
                    timer += Time.deltaTime;

                    float t =
                        Mathf.Clamp01(
                            timer / glowFadeDuration
                        );

                    float smoothT =
                        Mathf.SmoothStep(
                            0f,
                            1f,
                            t
                        );

                    wristbandGlow.intensity =
                        Mathf.Lerp(
                            startIntensity,
                            0f,
                            smoothT
                        );

                    yield return null;
                }

                wristbandGlow.intensity = 0f;
            }

            wristbandGlow.enabled = false;
        }

        // ---------------------------------------------
        // 3. Clue complete
        // ---------------------------------------------
        Debug.Log("WRISTBAND CLUE COMPLETE");

        onClueComplete?.Invoke();
    }
}