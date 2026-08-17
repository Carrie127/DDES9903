using System.Collections;
using UnityEngine;

public class TRFinalEndingController : MonoBehaviour
{
    // =====================================================
    // FINAL CHOICE
    // =====================================================

    [Header("Final Choice")]
    [SerializeField] private CanvasGroup finalQuestionCanvas;

    [Header("Ending Portals")]
    [SerializeField] private TRPortalRevealGroup acceptPortal;
    [SerializeField] private TRPortalRevealGroup rejectPortal;
    [SerializeField] private TRPortalRevealGroup rewritePortal;


    // =====================================================
    // ENDING A — ACCEPT
    // =====================================================

    [Header("Accept Ending - Mia Audio")]
    [SerializeField] private AudioSource miaAcceptAudio;

    [Header("Accept Ending - Final Visuals")]
    [SerializeField] private GameObject truthCore;

    [Header("Accept Ending - Final Text")]
    [SerializeField] private CanvasGroup acceptEndingCanvas;

    [Header("Accept Timing")]
    [SerializeField] private float questionFadeDuration = 0.6f;
    [SerializeField] private float miaStartDelay = 0.4f;
    [SerializeField] private float acceptPortalHideDelay = 1.0f;
    [SerializeField] private float finalDisappearDelay = 0.8f;

    [Header("Accept Ending Text Timing")]
    [SerializeField] private float endingTextDelay = 1.4f;
    [SerializeField] private float endingTextFadeDuration = 1.5f;


    // =====================================================
    // SCENE TRANSITION
    // =====================================================

    [Header("Ending Scene Transition")]
    [SerializeField] private MemorySceneTransition sceneTransition;


    // =====================================================
    // ENDING B — REJECT
    // =====================================================

    [Header("Reject Ending")]
    [SerializeField] private string archiveSceneName = "SceneA_Archive";
    [SerializeField] private float rejectTransitionDelay = 0.8f;


    // =====================================================
    // ENDING C — REWRITE
    // =====================================================

    [Header("Rewrite Ending")]
    [SerializeField] private string orphanageSceneName = "Scene_Orphanage";
    [SerializeField] private float rewriteTransitionDelay = 0.8f;


    // =====================================================
    // STATE
    // =====================================================

    private bool endingStarted = false;


    // =====================================================
    // INITIAL SETUP
    // =====================================================

    private void Awake()
    {
        if (acceptEndingCanvas != null)
        {
            acceptEndingCanvas.alpha = 0f;
            acceptEndingCanvas.interactable = false;
            acceptEndingCanvas.blocksRaycasts = false;
        }
    }


    // =====================================================
    // ENDING ENTRY
    // =====================================================

    public void BeginEnding(
        TRFinalEndingTrigger.EndingType endingType)
    {
        if (endingStarted)
            return;

        endingStarted = true;

        switch (endingType)
        {
            case TRFinalEndingTrigger.EndingType.Accept:
                StartCoroutine(AcceptEndingSequence());
                break;

            case TRFinalEndingTrigger.EndingType.Reject:
                StartCoroutine(RejectEndingSequence());
                break;

            case TRFinalEndingTrigger.EndingType.Rewrite:
                StartCoroutine(RewriteEndingSequence());
                break;
        }
    }


    // =====================================================
    // ENDING A — ACCEPT
    // =====================================================

    private IEnumerator AcceptEndingSequence()
    {
        Debug.Log("ENDING A → ACCEPT STARTED");

        DisableAllEndingTriggers();

        // -------------------------------------------------
        // Remove the other two choices
        // -------------------------------------------------

        if (rejectPortal != null)
            rejectPortal.Hide();

        if (rewritePortal != null)
            rewritePortal.Hide();


        // -------------------------------------------------
        // Fade final question
        // -------------------------------------------------

        if (finalQuestionCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    finalQuestionCanvas,
                    finalQuestionCanvas.alpha,
                    0f,
                    questionFadeDuration
                )
            );
        }


        // -------------------------------------------------
        // Wait before Mia speaks
        // -------------------------------------------------

        yield return new WaitForSeconds(
            miaStartDelay
        );


        // -------------------------------------------------
        // Mia Accept audio
        // -------------------------------------------------

        if (miaAcceptAudio != null)
        {
            miaAcceptAudio.Play();

            Debug.Log(
                "ENDING A → MIA ACCEPT AUDIO"
            );

            yield return new WaitForSeconds(
                acceptPortalHideDelay
            );

            if (acceptPortal != null)
                acceptPortal.Hide();

            while (miaAcceptAudio.isPlaying)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning(
                "ENDING A → Mia Accept Audio is not assigned!"
            );

            if (acceptPortal != null)
                acceptPortal.Hide();
        }


        // -------------------------------------------------
        // Final disappearance
        // -------------------------------------------------

        yield return new WaitForSeconds(
            finalDisappearDelay
        );

        if (truthCore != null)
        {
            truthCore.SetActive(false);
        }


        // -------------------------------------------------
        // Hold darkness
        // -------------------------------------------------

        yield return new WaitForSeconds(
            endingTextDelay
        );


        // -------------------------------------------------
        // Final Accept sentence
        // -------------------------------------------------

        if (acceptEndingCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    acceptEndingCanvas,
                    0f,
                    1f,
                    endingTextFadeDuration
                )
            );
        }

        Debug.Log(
            "ENDING A → ACCEPT COMPLETE"
        );
    }


    // =====================================================
    // ENDING B — REJECT
    // =====================================================

    private IEnumerator RejectEndingSequence()
    {
        Debug.Log(
            "ENDING B → REJECT STARTED"
        );

        DisableAllEndingTriggers();


        // -------------------------------------------------
        // Save Reject state for Archive
        // -------------------------------------------------

        FinalEndingState.SetReject();


        // -------------------------------------------------
        // Remove ACCEPT + REWRITE
        // -------------------------------------------------

        if (acceptPortal != null)
            acceptPortal.Hide();

        if (rewritePortal != null)
            rewritePortal.Hide();


        // -------------------------------------------------
        // Fade final question
        // -------------------------------------------------

        if (finalQuestionCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    finalQuestionCanvas,
                    finalQuestionCanvas.alpha,
                    0f,
                    questionFadeDuration
                )
            );
        }


        // -------------------------------------------------
        // Leave REJECT alone briefly
        // -------------------------------------------------

        yield return new WaitForSeconds(
            rejectTransitionDelay
        );


        // -------------------------------------------------
        // Return to Archive
        // -------------------------------------------------

        Debug.Log(
            "ENDING B → RETURNING TO ARCHIVE"
        );

        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionTo(
                archiveSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "ENDING B → MemorySceneTransition is not assigned!"
            );
        }
    }


    // =====================================================
    // ENDING C — REWRITE
    // =====================================================

    private IEnumerator RewriteEndingSequence()
    {
        Debug.Log(
            "ENDING C → REWRITE STARTED"
        );

        DisableAllEndingTriggers();


        // -------------------------------------------------
        // Save Rewrite state for Orphanage
        // -------------------------------------------------

        FinalEndingState.SetRewrite();


        // -------------------------------------------------
        // Remove ACCEPT + REJECT
        // -------------------------------------------------

        if (acceptPortal != null)
            acceptPortal.Hide();

        if (rejectPortal != null)
            rejectPortal.Hide();


        // -------------------------------------------------
        // Fade final question
        // -------------------------------------------------

        if (finalQuestionCanvas != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    finalQuestionCanvas,
                    finalQuestionCanvas.alpha,
                    0f,
                    questionFadeDuration
                )
            );
        }


        // -------------------------------------------------
        // Leave REWRITE alone briefly
        // -------------------------------------------------

        yield return new WaitForSeconds(
            rewriteTransitionDelay
        );


        // -------------------------------------------------
        // Return to Orphanage
        // -------------------------------------------------

        Debug.Log(
            "ENDING C → RETURNING TO ORPHANAGE"
        );

        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionTo(
                orphanageSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "ENDING C → MemorySceneTransition is not assigned!"
            );
        }
    }


    // =====================================================
    // DISABLE ALL ENDING TRIGGERS
    // =====================================================

    private void DisableAllEndingTriggers()
    {
        DisablePortalTrigger(acceptPortal);
        DisablePortalTrigger(rejectPortal);
        DisablePortalTrigger(rewritePortal);
    }


    private void DisablePortalTrigger(
        TRPortalRevealGroup portal)
    {
        if (portal == null)
            return;

        Collider[] colliders =
            portal.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider col in colliders)
        {
            if (col != null)
            {
                col.enabled = false;
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
}