using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MemoryPlacementTarget : MonoBehaviour
{
    [Header("Snap Settings")]
    public Transform snapPoint;

    [Header("Current Guidance Lights")]
    public Light objectGlow;
    public Light targetGlow;

    [Header("Current Stage Audio")]
    [Tooltip("Add all AudioSources that belong to the current memory stage.")]
    public AudioSource[] waitForAudioSources;

    [Tooltip(
        "All listed AudioSources must remain silent for this long " +
        "before the current memory stage is considered complete."
    )]
    public float audioSilenceConfirmDuration = 0.8f;

    [Header("Fade Settings")]
    public float fadeOutDuration = 1.5f;

    [Header("Transition Timing")]
    [Tooltip("Extra pause after the current memory audio is fully complete.")]
    public float delayBeforeNextStage = 0.5f;

    [Header("Next Memory Stage")]
    [Tooltip(
        "Called only after the object is correctly placed " +
        "and the current memory audio has fully finished."
    )]
    public UnityEvent onPlacementComplete;

    private bool placed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (placed)
            return;

        Holdable holdable = other.GetComponent<Holdable>();

        if (holdable == null)
        {
            holdable =
                other.GetComponentInParent<Holdable>();
        }

        if (holdable != null &&
            holdable.CompareTag("MemoryObject"))
        {
            SnapObject(holdable);
        }
    }

    private void SnapObject(Holdable holdable)
    {
        placed = true;

        // =================================================
        // 1. Release object from EZPZ hold system
        // =================================================

        holdable.ForceDrop();

        Rigidbody rb =
            holdable.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // =================================================
        // 2. Snap object into correct position
        // =================================================

        if (snapPoint != null)
        {
            holdable.transform.position =
                snapPoint.position;

            holdable.transform.rotation =
                snapPoint.rotation;
        }
        else
        {
            Debug.LogWarning(
                "MemoryPlacementTarget: Snap Point is missing!"
            );
        }

        Debug.Log(
            "Memory object placed correctly: "
            + holdable.gameObject.name
        );

        StartCoroutine(
            PlacementSequence()
        );
    }

    private IEnumerator PlacementSequence()
    {
        // =================================================
        // 1. Fade current guidance lights away
        // =================================================

        yield return StartCoroutine(
            FadeCurrentLightsOut()
        );

        // =================================================
        // 2. Wait until current-stage audio is TRULY over
        // =================================================

        yield return StartCoroutine(
            WaitForCurrentStageAudio()
        );

        Debug.Log(
            "CURRENT MEMORY AUDIO FULLY COMPLETE"
        );

        // =================================================
        // 3. Small breathing space
        // =================================================

        if (delayBeforeNextStage > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeNextStage
            );
        }

        // =================================================
        // 4. Start next stage
        // =================================================

        onPlacementComplete?.Invoke();

        Debug.Log(
            "PLACEMENT COMPLETE - NEXT MEMORY STAGE"
        );
    }

    // =====================================================
    // AUDIO COMPLETION CHECK
    // =====================================================

    private IEnumerator WaitForCurrentStageAudio()
    {
        // If there are no AudioSources assigned,
        // there is nothing to wait for.
        if (waitForAudioSources == null ||
            waitForAudioSources.Length == 0)
        {
            yield break;
        }

        float silentTimer = 0f;

        while (silentTimer < audioSilenceConfirmDuration)
        {
            bool anyAudioPlaying = false;

            foreach (AudioSource source in waitForAudioSources)
            {
                if (source != null &&
                    source.isPlaying)
                {
                    anyAudioPlaying = true;
                    break;
                }
            }

            if (anyAudioPlaying)
            {
                // Any new clip starts:
                // silence confirmation must begin again.
                silentTimer = 0f;
            }
            else
            {
                // All sources are currently silent.
                silentTimer += Time.deltaTime;
            }

            yield return null;
        }
    }

    // =====================================================
    // LIGHT FADE
    // =====================================================

    private IEnumerator FadeCurrentLightsOut()
    {
        float objectStartIntensity =
            objectGlow != null
                ? objectGlow.intensity
                : 0f;

        float targetStartIntensity =
            targetGlow != null
                ? targetGlow.intensity
                : 0f;

        if (fadeOutDuration <= 0f)
        {
            if (objectGlow != null)
                objectGlow.intensity = 0f;

            if (targetGlow != null)
                targetGlow.intensity = 0f;

            yield break;
        }

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / fadeOutDuration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            if (objectGlow != null)
            {
                objectGlow.intensity =
                    Mathf.Lerp(
                        objectStartIntensity,
                        0f,
                        smoothT
                    );
            }

            if (targetGlow != null)
            {
                targetGlow.intensity =
                    Mathf.Lerp(
                        targetStartIntensity,
                        0f,
                        smoothT
                    );
            }

            yield return null;
        }

        if (objectGlow != null)
            objectGlow.intensity = 0f;

        if (targetGlow != null)
            targetGlow.intensity = 0f;
    }
}