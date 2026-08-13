using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WristbandInteraction : MonoBehaviour
{
    [Header("Wristband Glow")]
    public Light wristbandGlow;

    [Header("Timing")]
    public float glowFadeDuration = 0.8f;

    [Header("Clue State")]
    public UnityEvent onClueComplete;

    private bool hasBeenTriggered = false;

    private float originalGlowIntensity = 0f;

    private void Start()
    {
        if (wristbandGlow != null)
        {
            originalGlowIntensity = wristbandGlow.intensity;
        }
    }

    public void InspectWristband()
    {
        if (hasBeenTriggered)
            return;

        hasBeenTriggered = true;

        StartCoroutine(WristbandSequence());
    }

    private IEnumerator WristbandSequence()
    {
        // Fade out the guidance glow
        if (wristbandGlow != null)
        {
            float startIntensity = wristbandGlow.intensity;
            float timer = 0f;

            while (timer < glowFadeDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / glowFadeDuration);

                wristbandGlow.intensity =
                    Mathf.Lerp(startIntensity, 0f, t);

                yield return null;
            }

            wristbandGlow.intensity = 0f;
            wristbandGlow.enabled = false;
        }

        Debug.Log("WRISTBAND CLUE COMPLETE");

        onClueComplete?.Invoke();
    }
}