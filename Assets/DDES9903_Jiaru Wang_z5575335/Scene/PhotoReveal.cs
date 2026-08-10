using System.Collections;
using UnityEngine;

public class PhotoReveal : MonoBehaviour
{
    [Header("Final Rotation")]
    public Vector3 revealedRotation;

    [Header("Reveal Settings")]
    public float revealDuration = 1.5f;

    [Header("Photo Glow")]
    public Light photoGlow;
    public float glowFadeDuration = 1.5f;

    [Header("Door Sequence")]
    public BedroomDoorSequence bedroomDoorSequence;

    private bool revealed = false;

    public void RevealPhoto()
    {
        if (revealed)
            return;

        revealed = true;

        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(revealedRotation);

        float startGlowIntensity = 0f;

        if (photoGlow != null)
        {
            startGlowIntensity = photoGlow.intensity;
        }

        float timer = 0f;

        while (timer < revealDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / revealDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 相框缓慢扶正
            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    smoothT
                );

            // 相框扶正的同时，Photo Glow 慢慢熄灭
            if (photoGlow != null)
            {
                float glowT = Mathf.Clamp01(
                    timer / glowFadeDuration
                );

                photoGlow.intensity =
                    Mathf.Lerp(
                        startGlowIntensity,
                        0f,
                        glowT
                    );
            }

            yield return null;
        }

        // 确保最后完全摆正
        transform.localRotation = targetRotation;

        // 确保灯完全熄灭
        if (photoGlow != null)
        {
            photoGlow.intensity = 0f;
        }

        Debug.Log("Sisters photo revealed!");

        // 相框完成后，启动关门流程
        if (bedroomDoorSequence != null)
        {
            bedroomDoorSequence.StartDoorClose();
        }
    }
}