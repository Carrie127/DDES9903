using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArchiveMemoryTransition : MonoBehaviour
{
    [Header("Transition Screens")]
    public CanvasGroup blackScreen;
    public CanvasGroup whiteScreen;

    [Header("Timing")]
    public float blackFadeDuration = 1.2f;
    public float holdBlackDuration = 0.35f;
    public float blackToWhiteDuration = 1.0f;
    public float holdWhiteDuration = 0.2f;
    public float whiteFadeOutDuration = 1.5f;

    [Header("Destination Intro Hold")]
    [Tooltip("Only this destination scene will hold on full white after loading.")]
    public string postLoadHoldSceneName = "Scene_Hospital";

    [Tooltip("How long to remain fully white after the destination scene loads.")]
    public float postLoadWhiteHoldDuration = 2.2f;

    private bool transitionStarted = false;
    private string sceneToLoad;

    private void Start()
    {
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.blocksRaycasts = false;
            blackScreen.interactable = false;
        }

        if (whiteScreen != null)
        {
            whiteScreen.alpha = 0f;
            whiteScreen.blocksRaycasts = false;
            whiteScreen.interactable = false;
        }
    }

    public void StartTransitionTo(string targetScene)
    {
        if (transitionStarted)
            return;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning(
                "ArchiveMemoryTransition: Target scene name is empty!"
            );

            return;
        }

        sceneToLoad = targetScene;
        transitionStarted = true;

        StartCoroutine(
            TransitionSequence()
        );
    }

    private IEnumerator TransitionSequence()
    {
        DontDestroyOnLoad(gameObject);

        GameObject canvasRoot = null;

        if (whiteScreen != null)
        {
            canvasRoot =
                whiteScreen.transform.root.gameObject;

            if (canvasRoot != gameObject)
            {
                DontDestroyOnLoad(canvasRoot);
            }
        }

        if (blackScreen != null)
        {
            blackScreen.blocksRaycasts = true;
        }

        if (whiteScreen != null)
        {
            whiteScreen.blocksRaycasts = true;
        }

        // -------------------------------------------------
        // 1. Archive fades to black
        // -------------------------------------------------

        if (blackScreen != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    blackScreen,
                    blackScreen.alpha,
                    1f,
                    blackFadeDuration
                )
            );
        }

        // -------------------------------------------------
        // 2. Brief full-black pause
        // -------------------------------------------------

        if (holdBlackDuration > 0f)
        {
            yield return new WaitForSeconds(
                holdBlackDuration
            );
        }

        // -------------------------------------------------
        // 3. White gradually covers the black
        // -------------------------------------------------

        if (whiteScreen != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    whiteScreen,
                    whiteScreen.alpha,
                    1f,
                    blackToWhiteDuration
                )
            );
        }

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
        }

        // -------------------------------------------------
        // 4. Brief full-white pause BEFORE load
        // -------------------------------------------------

        if (holdWhiteDuration > 0f)
        {
            yield return new WaitForSeconds(
                holdWhiteDuration
            );
        }

        // -------------------------------------------------
        // 5. Load first Memory Scene
        // -------------------------------------------------

        SceneManager.LoadScene(
            sceneToLoad
        );

        // Destination scene initialises here.
        // Hospital Intro can begin with Play On Awake.
        yield return null;

        // -------------------------------------------------
        // 6. SPECIAL POST-LOAD WHITE HOLD
        // -------------------------------------------------

        if (
            postLoadWhiteHoldDuration > 0f &&
            sceneToLoad == postLoadHoldSceneName
        )
        {
            Debug.Log(
                "ARCHIVE TRANSITION HOLDING WHITE FOR INTRO → "
                + sceneToLoad
            );

            yield return new WaitForSeconds(
                postLoadWhiteHoldDuration
            );
        }

        // -------------------------------------------------
        // 7. White fades away in destination scene
        // -------------------------------------------------

        if (whiteScreen != null)
        {
            yield return StartCoroutine(
                FadeCanvas(
                    whiteScreen,
                    1f,
                    0f,
                    whiteFadeOutDuration
                )
            );

            whiteScreen.blocksRaycasts = false;
        }

        if (blackScreen != null)
        {
            blackScreen.blocksRaycasts = false;
        }

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

    private IEnumerator FadeCanvas(
        CanvasGroup canvasGroup,
        float from,
        float to,
        float duration
    )
    {
        if (canvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            canvasGroup.alpha = to;
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