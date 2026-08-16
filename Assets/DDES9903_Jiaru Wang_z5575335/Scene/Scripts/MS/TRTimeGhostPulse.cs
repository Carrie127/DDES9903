using UnityEngine;
using TMPro;

public class TRTimeGhostPulse : MonoBehaviour
{
    [Header("Ghost Texts")]
    public TMP_Text ghostLeft;
    public TMP_Text ghostRight;

    [Header("Pulse Settings")]
    [Range(0f, 1f)] public float leftMinAlpha = 0.18f;
    [Range(0f, 1f)] public float leftMaxAlpha = 0.38f;

    [Range(0f, 1f)] public float rightMinAlpha = 0.10f;
    [Range(0f, 1f)] public float rightMaxAlpha = 0.28f;

    public float pulseSpeed = 0.45f;

    void Update()
    {
        float waveA = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
        float waveB = (Mathf.Sin(
            Time.time * pulseSpeed * Mathf.PI * 2f + Mathf.PI
        ) + 1f) * 0.5f;

        SetAlpha(
            ghostLeft,
            Mathf.Lerp(leftMinAlpha, leftMaxAlpha, waveA)
        );

        SetAlpha(
            ghostRight,
            Mathf.Lerp(rightMinAlpha, rightMaxAlpha, waveB)
        );
    }

    private void SetAlpha(TMP_Text text, float alpha)
    {
        if (text == null)
            return;

        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}