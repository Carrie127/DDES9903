using System.Collections;
using UnityEngine;

public class BedroomDoorSequence : MonoBehaviour
{
    [Header("Door")]
    public Transform doorTransform;

    [Header("Rotation")]
    public float closedYRotation = 0f;

    [Header("Timing")]
    public float delayBeforeClose = 0.8f;
    public float closeDuration = 1.2f;

    [Header("Clock Glow")]
    public Light clockGlow;
    public float delayBeforeClockGlow = 0.5f;
    public float clockGlowFadeDuration = 1.5f;

    private bool hasClosed = false;
    private float clockGlowTargetIntensity = 1f;

    private void Start()
    {
        // 记住 Clock 最终应该亮到多亮
        if (clockGlow != null)
        {
            clockGlowTargetIntensity = clockGlow.intensity;

            // 游戏开始时先隐藏 Clock Glow
            clockGlow.intensity = 0f;
        }
    }

    public void StartDoorClose()
    {
        if (hasClosed)
            return;

        hasClosed = true;

        StartCoroutine(CloseDoorSequence());
    }

    private IEnumerator CloseDoorSequence()
    {
        // 相框结束后稍微停顿
        yield return new WaitForSeconds(delayBeforeClose);

        if (doorTransform == null)
        {
            Debug.LogWarning(
                "BedroomDoorSequence: Door Transform is missing!"
            );

            yield break;
        }

        Quaternion startRotation = doorTransform.localRotation;

        Vector3 targetEuler = doorTransform.localEulerAngles;
        targetEuler.y = closedYRotation;

        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        float timer = 0f;

        while (timer < closeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / closeDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            doorTransform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        // 确保门完全关闭
        doorTransform.localRotation = targetRotation;

        Debug.Log("Bedroom door closed!");

        // 门完全关闭后稍微停一下
        yield return new WaitForSeconds(delayBeforeClockGlow);

        // Clock Glow 慢慢亮起
        if (clockGlow != null)
        {
            yield return StartCoroutine(FadeClockGlowIn());
        }
    }

    private IEnumerator FadeClockGlowIn()
    {
        float timer = 0f;

        while (timer < clockGlowFadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / clockGlowFadeDuration
            );

            clockGlow.intensity =
                Mathf.Lerp(
                    0f,
                    clockGlowTargetIntensity,
                    t
                );

            yield return null;
        }

        clockGlow.intensity = clockGlowTargetIntensity;

        Debug.Log("Clock glow activated!");
    }
}