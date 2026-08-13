using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalClockInteraction : MonoBehaviour
{
    [Header("Exit Sign Guidance")]
    public Light exitSignGlow;
    public ClueLightPulse exitSignPulse;

    [Header("Timing")]
    public float signFadeDuration = 0.8f;

    [Header("Clock State")]
    public UnityEvent onClockActivated;

    private bool hasBeenActivated = false;

    public void ActivateClock()
    {
        if (hasBeenActivated)
            return;

        hasBeenActivated = true;

        StartCoroutine(ClockSequence());
    }

    private IEnumerator ClockSequence()
    {
        Debug.Log("HOSPITAL CLOCK 7:42 ACTIVATED");

        // Stop the sign pulse first
        if (exitSignPulse != null)
        {
            exitSignPulse.StopPulse();
        }

        // Fade out the Emergency Department sign guidance light
        if (exitSignGlow != null)
        {
            float startIntensity = exitSignGlow.intensity;
            float timer = 0f;

            if (signFadeDuration <= 0f)
            {
                exitSignGlow.intensity = 0f;
            }
            else
            {
                while (timer < signFadeDuration)
                {
                    timer += Time.deltaTime;

                    float t = Mathf.Clamp01(
                        timer / signFadeDuration
                    );

                    float smoothT = Mathf.SmoothStep(
                        0f,
                        1f,
                        t
                    );

                    exitSignGlow.intensity = Mathf.Lerp(
                        startIntensity,
                        0f,
                        smoothT
                    );

                    yield return null;
                }

                exitSignGlow.intensity = 0f;
            }

            exitSignGlow.enabled = false;
        }

        Debug.Log("HOSPITAL CLOCK INTERACTION COMPLETE");

        onClockActivated?.Invoke();
    }
}