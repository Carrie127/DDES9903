using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalExitTransitionTrigger : MonoBehaviour
{
    [Header("White Screen")]
    public CanvasGroup whiteScreen;

    [Header("Transition Timing")]
    public float whiteFadeDuration = 1.5f;
    public float holdWhiteDuration = 0.3f;

    [Header("Scene Names")]
    public string orphanageSceneName = "Scene_Orphanage";
    public string memorySpaceSceneName = "Memory Space";

    [Header("Editor Testing")]
    [Tooltip("Only used when no real MemoryRouteState has been set.")]
    public bool useEditorFallbackRoute = true;

    [Tooltip(
        "ON = simulate Hospital-first, so Hospital goes to Orphanage.\n" +
        "OFF = simulate Orphanage-first, so Hospital goes to Memory Space."
    )]
    public bool editorFallbackHospitalFirst = true;

    [Header("Debug")]
    [SerializeField] private bool hasTriggered = false;

    private void Start()
    {
        if (whiteScreen != null)
        {
            whiteScreen.alpha = 0f;
        }

        ApplyEditorFallbackIfNeeded();
    }

    // =====================================================
    // Editor / direct-scene testing fallback
    // =====================================================

    private void ApplyEditorFallbackIfNeeded()
    {
        // IMPORTANT:
        // If a real route already exists, do absolutely nothing.
        if (MemoryRouteState.CurrentRoute !=
            MemoryRouteState.MemoryRoute.None)
        {
            Debug.Log(
                "HOSPITAL TEST FALLBACK NOT USED - REAL ROUTE ALREADY SET: "
                + MemoryRouteState.CurrentRoute
            );

            return;
        }

        // No real route exists.
        if (!useEditorFallbackRoute)
        {
            Debug.LogWarning(
                "HOSPITAL: NO ROUTE SET AND EDITOR FALLBACK IS DISABLED."
            );

            return;
        }

        if (editorFallbackHospitalFirst)
        {
            MemoryRouteState.SetHospitalFirst();

            Debug.Log(
                "HOSPITAL EDITOR TEST ROUTE → HOSPITAL FIRST"
            );
        }
        else
        {
            MemoryRouteState.SetOrphanageFirst();

            Debug.Log(
                "HOSPITAL EDITOR TEST ROUTE → ORPHANAGE FIRST"
            );
        }
    }

    // =====================================================
    // Trigger
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        Debug.Log(
            "HOSPITAL EXIT TRANSITION TRIGGERED"
        );

        StartCoroutine(
            TransitionSequence()
        );
    }

    // =====================================================
    // Transition
    // =====================================================

    private IEnumerator TransitionSequence()
    {
        // ---------------------------------------------
        // 1. Fade to white
        // ---------------------------------------------

        if (whiteScreen != null)
        {
            float startAlpha =
                whiteScreen.alpha;

            float timer = 0f;

            if (whiteFadeDuration <= 0f)
            {
                whiteScreen.alpha = 1f;
            }
            else
            {
                while (timer < whiteFadeDuration)
                {
                    timer += Time.deltaTime;

                    float t =
                        Mathf.Clamp01(
                            timer / whiteFadeDuration
                        );

                    float smoothT =
                        Mathf.SmoothStep(
                            0f,
                            1f,
                            t
                        );

                    whiteScreen.alpha =
                        Mathf.Lerp(
                            startAlpha,
                            1f,
                            smoothT
                        );

                    yield return null;
                }

                whiteScreen.alpha = 1f;
            }
        }

        Debug.Log(
            "HOSPITAL WHITE TRANSITION COMPLETE"
        );

        // ---------------------------------------------
        // 2. Hold full white briefly
        // ---------------------------------------------

        if (holdWhiteDuration > 0f)
        {
            yield return new WaitForSeconds(
                holdWhiteDuration
            );
        }

        // ---------------------------------------------
        // 3. Decide destination from route
        // ---------------------------------------------

        if (MemoryRouteState.IsHospitalFirst())
        {
            Debug.Log(
                "HOSPITAL FIRST ROUTE → LOADING ORPHANAGE"
            );

            SceneManager.LoadScene(
                orphanageSceneName
            );
        }
        else if (MemoryRouteState.IsOrphanageFirst())
        {
            Debug.Log(
                "ORPHANAGE FIRST ROUTE → LOADING MEMORY SPACE"
            );

            SceneManager.LoadScene(
                memorySpaceSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "HOSPITAL EXIT: MEMORY ROUTE HAS NOT BEEN SET!"
            );
        }
    }
}