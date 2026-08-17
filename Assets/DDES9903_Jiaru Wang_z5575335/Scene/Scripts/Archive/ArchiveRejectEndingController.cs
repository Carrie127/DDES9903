using System.Collections;
using UnityEngine;

public class ArchiveRejectEndingController : MonoBehaviour
{
    [Header("Mia Reject Audio")]
    [SerializeField] private AudioSource miaRejectAudio;

    [Header("Archive Lights")]
    [Tooltip("Only drag in the lights you want to fade completely to 0.")]
    [SerializeField] private Light[] lightsToFade;

    [Header("Sequence Timing")]
    [Tooltip("Wait after entering Archive before Mia begins speaking.")]
    [SerializeField] private float startDelay = 1.8f;

    [Tooltip("Wait after Mia starts speaking before the first light begins fading.")]
    [SerializeField] private float lightSequenceDelay = 1.5f;

    [Tooltip("How long each individual light takes to fade to 0.")]
    [SerializeField] private float lightFadeDuration = 1.0f;

    [Tooltip("Delay between one light finishing and the next light beginning.")]
    [SerializeField] private float delayBetweenLights = 0.35f;

    [Header("Reject Ending Overlay")]
    [SerializeField] private CanvasGroup rejectEndingCanvas;

    [Header("Ending Timing")]
    [Tooltip("How long the dim Archive remains visible after Mia and the light sequence finish.")]
    [SerializeField] private float finalHoldBeforeOverlay = 1.2f;

    [Tooltip("How long the semi-transparent ending overlay takes to fade in.")]
    [SerializeField] private float overlayFadeDuration = 1.5f;

    private float[] originalLightIntensities;

    private Coroutine endingCoroutine;
    private bool hasStarted = false;

    // =====================================================
    // INITIAL SETUP
    // =====================================================

    private void Awake()
    {
        CacheLightIntensities();

        if (miaRejectAudio != null)
        {
            miaRejectAudio.Stop();
        }

        if (rejectEndingCanvas != null)
        {
            rejectEndingCanvas.alpha = 0f;
            rejectEndingCanvas.interactable = false;
            rejectEndingCanvas.blocksRaycasts = false;
        }
    }

    private void OnEnable()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        endingCoroutine =
            StartCoroutine(RejectEndingSequence());
    }

    // =====================================================
    // CACHE ORIGINAL LIGHT INTENSITIES
    // =====================================================

    private void CacheLightIntensities()
    {
        if (lightsToFade == null)
            return;

        originalLightIntensities =
            new float[lightsToFade.Length];

        for (int i = 0; i < lightsToFade.Length; i++)
        {
            if (lightsToFade[i] == null)
                continue;

            originalLightIntensities[i] =
                lightsToFade[i].intensity;
        }
    }

    // =====================================================
    // REJECT ENDING SEQUENCE
    // =====================================================

    private IEnumerator RejectEndingSequence()
    {
        Debug.Log(
            "REJECT ARCHIVE → ENDING SEQUENCE STARTED"
        );

        // -------------------------------------------------
        // 1. Wait for the Memory Space transition to settle
        // -------------------------------------------------

        yield return new WaitForSeconds(
            startDelay
        );

        // -------------------------------------------------
        // 2. Mia begins speaking
        // -------------------------------------------------

        if (miaRejectAudio != null)
        {
            miaRejectAudio.Play();

            Debug.Log(
                "REJECT ARCHIVE → MIA AUDIO STARTED"
            );
        }

        // -------------------------------------------------
        // 3. Wait before starting the light sequence
        // -------------------------------------------------

        yield return new WaitForSeconds(
            lightSequenceDelay
        );

        // -------------------------------------------------
        // 4. Fade selected lights one by one
        // -------------------------------------------------

        if (lightsToFade != null &&
            originalLightIntensities != null)
        {
            for (int i = 0; i < lightsToFade.Length; i++)
            {
                Light targetLight =
                    lightsToFade[i];

                if (targetLight == null)
                    continue;

                yield return StartCoroutine(
                    FadeLight(
                        targetLight,
                        originalLightIntensities[i],
                        0f,
                        lightFadeDuration
                    )
                );

                /*
                 * IMPORTANT:
                 *
                 * Do NOT set:
                 * targetLight.enabled = false;
                 *
                 * The light stays enabled,
                 * but its intensity is now 0.
                 *
                 * Visually it is completely off.
                 */

                Debug.Log(
                    "REJECT ARCHIVE → LIGHT " +
                    (i + 1) +
                    " FADED TO 0"
                );

                if (i < lightsToFade.Length - 1)
                {
                    yield return new WaitForSeconds(
                        delayBetweenLights
                    );
                }
            }
        }

        // -------------------------------------------------
        // 5. Wait for Mia to finish if she is still speaking
        // -------------------------------------------------

        if (miaRejectAudio != null)
        {
            while (miaRejectAudio.isPlaying)
            {
                yield return null;
            }
        }

        // -------------------------------------------------
        // 6. Leave the darkened Archive visible briefly
        // -------------------------------------------------

        yield return new WaitForSeconds(
            finalHoldBeforeOverlay
        );

        // -------------------------------------------------
        // 7. Fade in semi-transparent black overlay
        //    + final ending text
        // -------------------------------------------------

        if (rejectEndingCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    rejectEndingCanvas,
                    0f,
                    1f,
                    overlayFadeDuration
                )
            );
        }

        Debug.Log(
            "REJECT ARCHIVE → ENDING COMPLETE"
        );

        endingCoroutine = null;
    }

    // =====================================================
    // LIGHT FADE
    // =====================================================

    private IEnumerator FadeLight(
        Light targetLight,
        float from,
        float to,
        float duration)
    {
        if (targetLight == null)
            yield break;

        /*
         * Keep the Light component enabled.
         * Only intensity changes.
         */
        targetLight.enabled = true;

        if (duration <= 0f)
        {
            targetLight.intensity = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            targetLight.intensity =
                Mathf.Lerp(
                    from,
                    to,
                    smoothT
                );

            yield return null;
        }

        targetLight.intensity = to;
    }

    // =====================================================
    // ENDING OVERLAY FADE
    // =====================================================

    private IEnumerator FadeCanvasGroup(
        CanvasGroup canvasGroup,
        float from,
        float to,
        float duration)
    {
        if (canvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            canvasGroup.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    smoothT
                );

            yield return null;
        }

        canvasGroup.alpha = to;
    }

    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDisable()
    {
        if (endingCoroutine != null)
        {
            StopCoroutine(endingCoroutine);
            endingCoroutine = null;
        }
    }
}