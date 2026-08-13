using UnityEngine;

public class ClueLightPulse : MonoBehaviour
{
    [Header("Light")]
    public Light clueLight;

    [Header("Pulse Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 2.2f;
    public float pulseSpeed = 2.8f;

    private bool isPulsing = true;

    private void Update()
    {
        if (!isPulsing)
            return;

        if (clueLight == null || !clueLight.enabled)
            return;

        float t =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        clueLight.intensity =
            Mathf.Lerp(
                minIntensity,
                maxIntensity,
                t
            );
    }

    // =====================================================
    // Stop pulse but keep current light intensity
    // =====================================================

    public void StopPulse()
    {
        isPulsing = false;
    }

    // =====================================================
    // Resume pulse if needed
    // =====================================================

    public void StartPulse()
    {
        if (clueLight == null)
            return;

        clueLight.enabled = true;
        isPulsing = true;
    }

    // =====================================================
    // Completely stop and turn light off
    // =====================================================

    public void StopPulseAndTurnOff()
    {
        isPulsing = false;

        if (clueLight != null)
        {
            clueLight.intensity = 0f;
            clueLight.enabled = false;
        }
    }
}