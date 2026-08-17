using System.Collections;
using UnityEngine;

public class TRPortalRevealGroup : MonoBehaviour
{
    [Header("Portal Parts")]
    [SerializeField] private LineRenderer[] rings;
    [SerializeField] private CanvasGroup labelCanvasGroup;
    [SerializeField] private GameObject interactArea;

    [Header("Reveal Settings")]
    [SerializeField] private float ringRevealDuration = 1.2f;
    [SerializeField] private float labelDelay = 0.75f;
    [SerializeField] private float labelRevealDuration = 0.6f;
    [SerializeField] private float floatUpDistance = 0.18f;

    [Header("Hide Settings")]
    [SerializeField] private float hideDuration = 0.8f;

    [SerializeField]
    private AnimationCurve easeCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 shownLocalPosition;
    private Vector3 hiddenLocalPosition;

    private float[] originalWidths;

    private bool hasCachedState = false;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        CacheOriginalState();
        PrepareHiddenState();
    }

    // =====================================================
    // CACHE ORIGINAL STATE
    // =====================================================

    private void CacheOriginalState()
    {
        if (hasCachedState)
            return;

        shownLocalPosition = transform.localPosition;

        hiddenLocalPosition =
            shownLocalPosition +
            Vector3.down * floatUpDistance;

        // Automatically find CanvasGroup if not assigned
        if (labelCanvasGroup == null)
        {
            Canvas canvas =
                GetComponentInChildren<Canvas>(true);

            if (canvas != null)
            {
                labelCanvasGroup =
                    canvas.GetComponent<CanvasGroup>();

                if (labelCanvasGroup == null)
                {
                    labelCanvasGroup =
                        canvas.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        // Remember original ring widths
        if (rings != null)
        {
            originalWidths =
                new float[rings.Length];

            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null)
                    continue;

                originalWidths[i] =
                    rings[i].widthMultiplier;
            }
        }

        hasCachedState = true;
    }

    // =====================================================
    // INITIAL HIDDEN STATE
    // =====================================================

    public void PrepareHiddenState()
    {
        CacheOriginalState();

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        transform.localPosition =
            hiddenLocalPosition;

        if (rings != null)
        {
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null)
                    continue;

                rings[i].widthMultiplier = 0f;
                rings[i].enabled = false;
            }
        }

        if (labelCanvasGroup != null)
        {
            labelCanvasGroup.alpha = 0f;
            labelCanvasGroup.interactable = false;
            labelCanvasGroup.blocksRaycasts = false;
        }

        if (interactArea != null)
            interactArea.SetActive(false);
    }

    // =====================================================
    // REVEAL
    // =====================================================

    public void Reveal()
    {
        CacheOriginalState();

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine =
            StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        // Turn rings on
        if (rings != null)
        {
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null)
                    continue;

                rings[i].enabled = true;
                rings[i].widthMultiplier = 0f;
            }
        }

        float totalDuration =
            Mathf.Max(
                ringRevealDuration,
                labelDelay + labelRevealDuration
            );

        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            // -----------------------------
            // Ring reveal
            // -----------------------------

            float ringT =
                Mathf.Clamp01(
                    elapsed / ringRevealDuration
                );

            float ringEase =
                easeCurve != null
                    ? easeCurve.Evaluate(ringT)
                    : ringT;

            transform.localPosition =
                Vector3.Lerp(
                    hiddenLocalPosition,
                    shownLocalPosition,
                    ringEase
                );

            if (rings != null)
            {
                for (int i = 0; i < rings.Length; i++)
                {
                    if (rings[i] == null)
                        continue;

                    rings[i].widthMultiplier =
                        Mathf.Lerp(
                            0f,
                            originalWidths[i],
                            ringEase
                        );
                }
            }

            // -----------------------------
            // Label reveal
            // -----------------------------

            float labelT = 0f;

            if (elapsed >= labelDelay)
            {
                labelT =
                    Mathf.Clamp01(
                        (elapsed - labelDelay) /
                        labelRevealDuration
                    );
            }

            if (labelCanvasGroup != null)
                labelCanvasGroup.alpha = labelT;

            yield return null;
        }

        // Final visible state
        transform.localPosition =
            shownLocalPosition;

        if (rings != null)
        {
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null)
                    continue;

                rings[i].enabled = true;
                rings[i].widthMultiplier =
                    originalWidths[i];
            }
        }

        if (labelCanvasGroup != null)
            labelCanvasGroup.alpha = 1f;

        // Only enable ending trigger after reveal finishes
        if (interactArea != null)
            interactArea.SetActive(true);

        activeCoroutine = null;
    }

    // =====================================================
    // HIDE
    // =====================================================

    public void Hide()
    {
        CacheOriginalState();

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine =
            StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        // Disable trigger immediately
        if (interactArea != null)
            interactArea.SetActive(false);

        float elapsed = 0f;

        while (elapsed < hideDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / hideDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            float visibleAmount =
                1f - smoothT;

            // Shrink ring glow away
            if (rings != null)
            {
                for (int i = 0; i < rings.Length; i++)
                {
                    if (rings[i] == null)
                        continue;

                    rings[i].widthMultiplier =
                        originalWidths[i] *
                        visibleAmount;
                }
            }

            // Fade text away
            if (labelCanvasGroup != null)
            {
                labelCanvasGroup.alpha =
                    visibleAmount;
            }

            yield return null;
        }

        // Completely hide rings
        if (rings != null)
        {
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null)
                    continue;

                rings[i].widthMultiplier = 0f;
                rings[i].enabled = false;
            }
        }

        if (labelCanvasGroup != null)
            labelCanvasGroup.alpha = 0f;

        activeCoroutine = null;
    }
}