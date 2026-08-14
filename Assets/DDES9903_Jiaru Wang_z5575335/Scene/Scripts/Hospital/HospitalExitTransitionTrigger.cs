using UnityEngine;

public class HospitalExitTransitionTrigger : MonoBehaviour
{
    [Header("Scene Transition")]
    public MemorySceneTransition sceneTransition;

    [Header("Scene Names")]
    public string orphanageSceneName =
        "Scene_Orphanage";

    public string memorySpaceSceneName =
        "Scene_MemorySpace";

    [Header("Editor Testing")]
    [Tooltip("Only used when no real MemoryRouteState has been set.")]
    public bool useEditorFallbackRoute = true;

    [Tooltip(
        "ON = simulate Hospital-first, so Hospital goes to Orphanage.\n" +
        "OFF = simulate Orphanage-first, so Hospital goes to Memory Space."
    )]
    public bool editorFallbackHospitalFirst = true;

    [Header("Debug")]
    [SerializeField]
    private bool hasTriggered = false;


    private void Start()
    {
        ApplyEditorFallbackIfNeeded();
    }


    // =====================================================
    // Editor / direct-scene testing fallback
    // =====================================================

    private void ApplyEditorFallbackIfNeeded()
    {
        // If a real route already exists,
        // do not overwrite it.
        if (MemoryRouteState.CurrentRoute !=
            MemoryRouteState.MemoryRoute.None)
        {
            Debug.Log(
                "HOSPITAL TEST FALLBACK NOT USED - REAL ROUTE ALREADY SET: "
                + MemoryRouteState.CurrentRoute
            );

            return;
        }

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
    // Exit Trigger
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (sceneTransition == null)
        {
            Debug.LogWarning(
                "HOSPITAL EXIT: MemorySceneTransition is missing!"
            );

            return;
        }

        string targetScene = "";

        // -------------------------------------------------
        // Hospital First
        // Archive → Hospital → Orphanage
        // -------------------------------------------------

        if (MemoryRouteState.IsHospitalFirst())
        {
            targetScene =
                orphanageSceneName;

            Debug.Log(
                "HOSPITAL FIRST ROUTE → ORPHANAGE"
            );
        }

        // -------------------------------------------------
        // Orphanage First
        // Archive → Orphanage → Hospital → Memory Space
        // -------------------------------------------------

        else if (MemoryRouteState.IsOrphanageFirst())
        {
            targetScene =
                memorySpaceSceneName;

            Debug.Log(
                "ORPHANAGE FIRST ROUTE → MEMORY SPACE"
            );
        }

        else
        {
            Debug.LogWarning(
                "HOSPITAL EXIT: MEMORY ROUTE HAS NOT BEEN SET!"
            );

            return;
        }

        hasTriggered = true;

        // -------------------------------------------------
        // Use the shared white transition system
        // Fade In → Load Scene → Fade Out
        // -------------------------------------------------

        sceneTransition.StartTransitionTo(
            targetScene
        );
    }
}