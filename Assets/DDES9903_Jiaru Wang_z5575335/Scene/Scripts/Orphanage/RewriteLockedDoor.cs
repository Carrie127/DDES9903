using System.Collections;
using UnityEngine;

public class RewriteLockedDoor : MonoBehaviour
{
    // =====================================================
    // 1 - FIRST DOOR ATTEMPT
    // =====================================================

    [Header("1 - First Door Attempt")]
    [SerializeField] private AudioSource firstDoorAttemptSFX;

    [Tooltip("Evie: No... no, come on. Open!")]
    [SerializeField] private AudioSource evieFirstReactionAudio;


    // =====================================================
    // 2 - FIRST DIALOGUE
    // =====================================================

    [Header("2 - First Evie / Mia Dialogue")]
    [Tooltip("Dialogue ending with Mia: You can't, Evie.")]
    [SerializeField] private AudioSource firstDialogueAudio;


    // =====================================================
    // 3 - SECOND DOOR ATTEMPT
    // =====================================================

    [Header("3 - Second Door Attempt")]
    [SerializeField] private AudioSource secondDoorAttemptSFX;


    // =====================================================
    // 4 - CORE DIALOGUE
    // =====================================================

    [Header("4 - Core Evie / Mia Dialogue")]
    [Tooltip("Dialogue beginning: No! I came back for you...")]
    [SerializeField] private AudioSource coreDialogueAudio;


    // =====================================================
    // 5 - MIA FINAL LINE
    // =====================================================

    [Header("5 - Mia Final Line")]
    [Tooltip("Mia: You don't have to change what happened to still love me.")]
    [SerializeField] private AudioSource miaFinalAudio;


    // =====================================================
    // AUDIO TIMING
    // =====================================================

    [Header("Sequence Timing")]
    [SerializeField] private float firstReactionDelay = 0.15f;
    [SerializeField] private float firstDialogueDelay = 0.6f;
    [SerializeField] private float secondDoorDelay = 0.7f;
    [SerializeField] private float coreDialogueDelay = 0.45f;
    [SerializeField] private float miaFinalDelay = 1.2f;


    // =====================================================
    // FINAL REWRITE VISUAL
    // =====================================================

    [Header("6 - Final Rewrite Visual")]

    [Tooltip("Only drag in lights that should fade to 0 at the end.")]
    [SerializeField] private Light[] endingLightsToFade;

    [SerializeField] private CanvasGroup rewriteEndingCanvas;


    [Header("Final Visual Timing")]

    [Tooltip("Silence after Mia's final line before the scene begins fading.")]
    [SerializeField] private float finalVisualDelay = 1.2f;

    [Tooltip("How long selected lights take to fade down.")]
    [SerializeField] private float finalLightFadeDuration = 2.0f;

    [Tooltip("How long the semi-transparent ending screen takes to appear.")]
    [SerializeField] private float endingCanvasFadeDuration = 1.6f;


    // =====================================================
    // STATE
    // =====================================================

    private bool sequenceStarted = false;
    private Coroutine sequenceCoroutine;

    private float[] originalEndingLightIntensities;


    // =====================================================
    // INITIAL SETUP
    // =====================================================

    private void Awake()
    {
        CacheEndingLightIntensities();

        if (rewriteEndingCanvas != null)
        {
            rewriteEndingCanvas.alpha = 0f;
            rewriteEndingCanvas.interactable = false;
            rewriteEndingCanvas.blocksRaycasts = false;
        }
    }


    // =====================================================
    // INTERACTION
    // =====================================================

    public void TryOpenDoor()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;

        sequenceCoroutine =
            StartCoroutine(
                RewriteDoorSequence()
            );
    }


    // =====================================================
    // FULL REWRITE SEQUENCE
    // =====================================================

    private IEnumerator RewriteDoorSequence()
    {
        Debug.Log(
            "REWRITE → BEDROOM DOOR SEQUENCE STARTED"
        );


        // -------------------------------------------------
        // 1. First door attempt
        // -------------------------------------------------

        if (firstDoorAttemptSFX != null)
        {
            firstDoorAttemptSFX.Stop();
            firstDoorAttemptSFX.Play();

            Debug.Log(
                "REWRITE → FIRST DOOR ATTEMPT"
            );

            while (firstDoorAttemptSFX.isPlaying)
                yield return null;
        }


        // -------------------------------------------------
        // 2. Evie first reaction
        // -------------------------------------------------

        yield return new WaitForSeconds(
            firstReactionDelay
        );

        if (evieFirstReactionAudio != null)
        {
            evieFirstReactionAudio.Stop();
            evieFirstReactionAudio.Play();

            Debug.Log(
                "REWRITE → EVIE FIRST REACTION"
            );

            while (evieFirstReactionAudio.isPlaying)
                yield return null;
        }


        // -------------------------------------------------
        // 3. First Evie / Mia dialogue
        // -------------------------------------------------

        yield return new WaitForSeconds(
            firstDialogueDelay
        );

        if (firstDialogueAudio != null)
        {
            firstDialogueAudio.Stop();
            firstDialogueAudio.Play();

            Debug.Log(
                "REWRITE → FIRST DIALOGUE"
            );

            while (firstDialogueAudio.isPlaying)
                yield return null;
        }


        // -------------------------------------------------
        // 4. Second desperate door attempt
        // -------------------------------------------------

        yield return new WaitForSeconds(
            secondDoorDelay
        );

        if (secondDoorAttemptSFX != null)
        {
            secondDoorAttemptSFX.Stop();
            secondDoorAttemptSFX.Play();

            Debug.Log(
                "REWRITE → SECOND DOOR ATTEMPT"
            );

            while (secondDoorAttemptSFX.isPlaying)
                yield return null;
        }


        // -------------------------------------------------
        // 5. Core dialogue
        // -------------------------------------------------

        yield return new WaitForSeconds(
            coreDialogueDelay
        );

        if (coreDialogueAudio != null)
        {
            coreDialogueAudio.Stop();
            coreDialogueAudio.Play();

            Debug.Log(
                "REWRITE → CORE DIALOGUE"
            );

            while (coreDialogueAudio.isPlaying)
                yield return null;
        }


        // -------------------------------------------------
        // 6. Mia final line
        // -------------------------------------------------

        yield return new WaitForSeconds(
            miaFinalDelay
        );

        if (miaFinalAudio != null)
        {
            miaFinalAudio.Stop();
            miaFinalAudio.Play();

            Debug.Log(
                "REWRITE → MIA FINAL LINE"
            );

            while (miaFinalAudio.isPlaying)
                yield return null;
        }


        // =================================================
        // FINAL VISUAL
        // =================================================

        yield return new WaitForSeconds(
            finalVisualDelay
        );

        Debug.Log(
            "REWRITE → FINAL VISUAL STARTED"
        );


        // -------------------------------------------------
        // Fade selected lights
        // -------------------------------------------------

        yield return StartCoroutine(
            FadeEndingLights()
        );


        // -------------------------------------------------
        // Fade in semi-transparent ending overlay + text
        // -------------------------------------------------

        if (rewriteEndingCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    rewriteEndingCanvas,
                    0f,
                    1f,
                    endingCanvasFadeDuration
                )
            );
        }


        Debug.Log(
            "REWRITE → ENDING COMPLETE"
        );

        sequenceCoroutine = null;
    }


    // =====================================================
    // CACHE LIGHT VALUES
    // =====================================================

    private void CacheEndingLightIntensities()
    {
        if (endingLightsToFade == null)
            return;

        originalEndingLightIntensities =
            new float[endingLightsToFade.Length];

        for (int i = 0; i < endingLightsToFade.Length; i++)
        {
            if (endingLightsToFade[i] != null)
            {
                originalEndingLightIntensities[i] =
                    endingLightsToFade[i].intensity;
            }
        }
    }


    // =====================================================
    // FADE ENDING LIGHTS
    // =====================================================

    private IEnumerator FadeEndingLights()
    {
        if (endingLightsToFade == null ||
            originalEndingLightIntensities == null ||
            endingLightsToFade.Length == 0)
        {
            yield break;
        }

        if (finalLightFadeDuration <= 0f)
        {
            foreach (Light targetLight in endingLightsToFade)
            {
                if (targetLight != null)
                    targetLight.intensity = 0f;
            }

            yield break;
        }

        float elapsed = 0f;

        while (elapsed < finalLightFadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / finalLightFadeDuration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            for (int i = 0; i < endingLightsToFade.Length; i++)
            {
                Light targetLight =
                    endingLightsToFade[i];

                if (targetLight == null)
                    continue;

                targetLight.intensity =
                    Mathf.Lerp(
                        originalEndingLightIntensities[i],
                        0f,
                        smoothT
                    );
            }

            yield return null;
        }

        for (int i = 0; i < endingLightsToFade.Length; i++)
        {
            if (endingLightsToFade[i] != null)
            {
                endingLightsToFade[i].intensity = 0f;
            }
        }
    }


    // =====================================================
    // CANVAS FADE
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
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }
    }
}