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
        Quaternion startRotation =
            transform.localRotation;

        Quaternion targetRotation =
            Quaternion.Euler(revealedRotation);

        float startGlowIntensity = 0f;

        if (photoGlow != null)
        {
            startGlowIntensity =
                photoGlow.intensity;
        }

        float timer = 0f;

        while (timer < revealDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / revealDuration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            // -----------------------------------------
            // Slowly straighten the photo
            // -----------------------------------------

            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    smoothT
                );

            // -----------------------------------------
            // Fade Photo Glow out during reveal
            // -----------------------------------------

            if (photoGlow != null)
            {
                float glowT =
                    Mathf.Clamp01(
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

        // ---------------------------------------------
        // Ensure final state
        // ---------------------------------------------

        transform.localRotation =
            targetRotation;

        if (photoGlow != null)
        {
            photoGlow.intensity = 0f;
        }

        Debug.Log(
            "Sisters photo revealed!"
        );

        // IMPORTANT:
        // Door closing is NOT controlled here anymore.
        // PhotoMemoryAudio will start the door sequence
        // only after all Photo dialogue has finished.
    }
}