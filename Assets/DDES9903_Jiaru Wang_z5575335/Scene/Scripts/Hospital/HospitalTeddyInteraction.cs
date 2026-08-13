using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalTeddyInteraction : MonoBehaviour
{
    [Header("Teddy Glow")]
    public Light teddyGlow;
    public ClueLightPulse clueLightPulse;

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

        StartCoroutine(TeddySequence());
    }

    private IEnumerator TeddySequence()
    {
        Debug.Log("HOSPITAL TEDDY CLUE TRIGGERED");

        // 1. Stop pulse
        if (clueLightPulse != null)
        {
            clueLightPulse.StopPulse();
        }

        // 2. Fade glow from current intensity
        if (teddyGlow != null)
        {
            float startIntensity = teddyGlow.intensity;
            float timer = 0f;

            if (glowFadeDuration <= 0f)
            {
                teddyGlow.intensity = 0f;
            }
            else
            {
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
            }

            teddyGlow.enabled = false;
        }

        // 3. Clue complete
        Debug.Log("HOSPITAL TEDDY CLUE COMPLETE");

        onClueComplete?.Invoke();
    }
}