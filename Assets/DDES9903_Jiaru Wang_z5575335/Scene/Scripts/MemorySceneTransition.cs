using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemorySceneTransition : MonoBehaviour
{
    [Header("White Screen")]
    public CanvasGroup whiteScreenCanvasGroup;

    [Header("Default Scene")]
    [Tooltip("Fallback scene name if StartTransition() is used.")]
    public string targetSceneName;

    [Header("Fade Timing")]
    public float delayBeforeFade = 0f;
    public float fadeInDuration = 1.5f;
    public float holdWhiteDuration = 0.2f;
    public float fadeOutDuration = 1.5f;

    [Header("Destination Intro Hold")]
    [Tooltip("Only this destination scene will hold on full white after loading.")]
    public string postLoadHoldSceneName = "Scene_Hospital";

    [Tooltip("How long to remain fully white after the destination scene loads.")]
    public float postLoadWhiteHoldDuration = 2.2f;

    private bool transitionStarted = false;
    private string sceneToLoad;

    private void Start()
    {
        if (whiteScreenCanvasGroup != null)
        {
            whiteScreenCanvasGroup.alpha = 0f;
            whiteScreenCanvasGroup.interactable = false;
            whiteScreenCanvasGroup.blocksRaycasts = false;
        }
    }

    // =====================================================
    // DEFAULT TRANSITION
    // =====================================================

    public void StartTransition()
    {
        StartTransitionTo(targetSceneName, 0f);
    }

    // =====================================================
    // TRANSITION TO A SPECIFIC SCENE
    // =====================================================

    public void StartTransitionTo(string newSceneName)
    {
        StartTransitionTo(newSceneName, 0f);
    }

    // =====================================================
    // TRANSITION WHILE WAITING FOR AUDIO TO FINISH
    // =====================================================

    public void StartTransitionTo(
        string newSceneName,
        float minimumTimeBeforeLoad
    )
    {
        if (transitionStarted)
            return;

        if (string.IsNullOrEmpty(newSceneName))
        {
            Debug.LogWarning(
                "MemorySceneTransition: Scene name is empty!"
            );

            return;
        }

        sceneToLoad = newSceneName;
        transitionStarted = true;

        StartCoroutine(
            TransitionSequence(
                minimumTimeBeforeLoad
            )
        );
    }

    // =====================================================
    // MAIN TRANSITION
    // =====================================================

    private IEnumerator TransitionSequence(
        float minimumTimeBeforeLoad
    )
    {
        Debug.Log(
            "MEMORY SCENE TRANSITION STARTED → "
            + sceneToLoad
        );

        DontDestroyOnLoad(gameObject);

        GameObject canvasRoot = null;

        if (whiteScreenCanvasGroup != null)
        {
            canvasRoot =
                whiteScreenCanvasGroup
                .transform
                .root
                .gameObject;

            if (canvasRoot != gameObject)
            {
                DontDestroyOnLoad(canvasRoot);
            }

            whiteScreenCanvasGroup.blocksRaycasts = true;
        }

        float transitionStartTime = Time.time;

        // -------------------------------------------------
        // 1. Optional delay before white begins
        // -------------------------------------------------

        if (delayBeforeFade > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeFade
            );
        }

        // -------------------------------------------------
        // 2. Fade INTO white
        // -------------------------------------------------

        if (whiteScreenCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    whiteScreenCanvasGroup.alpha,
                    1f,
                    fadeInDuration
                )
            );
        }

        // -------------------------------------------------
        // 3. Wait for current-scene dialogue if necessary
        // -------------------------------------------------

        float elapsed =
            Time.time - transitionStartTime;

        float extraWait =
            minimumTimeBeforeLoad - elapsed;

        if (extraWait > 0f)
        {
            yield return new WaitForSeconds(
                extraWait
            );
        }

        // -------------------------------------------------
        // 4. Brief full-white hold BEFORE load
        // -------------------------------------------------

        if (holdWhiteDuration > 0f)
        {
            yield return new WaitForSeconds(
                holdWhiteDuration
            );
        }

        // -------------------------------------------------
        // 5. Load next scene
        // -------------------------------------------------

        Debug.Log(
            "LOADING SCENE → "
            + sceneToLoad
        );

        SceneManager.LoadScene(
            sceneToLoad
        );

        // Let destination scene initialise.
        // Hospital Intro AudioSource can now Play On Awake.
        yield return null;

        // -------------------------------------------------
        // 6. SPECIAL POST-LOAD WHITE HOLD
        //
        // Hospital is already loaded and its intro audio
        // is playing, but the screen remains fully white.
        // -------------------------------------------------

        if (
            postLoadWhiteHoldDuration > 0f &&
            sceneToLoad == postLoadHoldSceneName
        )
        {
            Debug.Log(
                "HOLDING WHITE FOR DESTINATION INTRO → "
                + sceneToLoad
            );

            yield return new WaitForSeconds(
                postLoadWhiteHoldDuration
            );
        }

        // -------------------------------------------------
        // 7. Fade OUT from white in the new scene
        // -------------------------------------------------

        if (whiteScreenCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    1f,
                    0f,
                    fadeOutDuration
                )
            );

            whiteScreenCanvasGroup.blocksRaycasts = false;
        }

        Debug.Log(
            "MEMORY SCENE TRANSITION COMPLETE"
        );

        // -------------------------------------------------
        // 8. Clean up
        // -------------------------------------------------

        if (canvasRoot != null &&
            canvasRoot != gameObject)
        {
            Destroy(canvasRoot);
        }

        Destroy(gameObject);
    }

    // =====================================================
    // GENERIC CANVAS FADE
    // =====================================================

    private IEnumerator FadeCanvas(
        float from,
        float to,
        float duration
    )
    {
        if (whiteScreenCanvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            whiteScreenCanvasGroup.alpha = to;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            whiteScreenCanvasGroup.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    smoothT
                );

            yield return null;
        }

        whiteScreenCanvasGroup.alpha = to;
    }
}