using System.Collections;
using UnityEngine;

public class OrphanageOpeningController : MonoBehaviour
{
    [Header("Evie Opening Audio")]
    public AudioSource evieOpeningSource;

    public AudioClip evieOpeningOrphanageFirst;
    public AudioClip evieOpeningHospitalFirst;

    [Header("Mia Hide and Seek")]
    public AudioSource miaHideAndSeekSource;

    [Header("Bedroom Guide")]
    public BedroomGuideLight bedroomGuideLight;

    [Header("Timing")]
    public float delayBeforeMia = 0.5f;

    [Header("Editor Testing")]
    [Tooltip(
        "Only used when Scene_Orphanage is played directly " +
        "and no route has been set by Archive."
    )]
    public bool useEditorTestRoute = true;

    public enum EditorTestRoute
    {
        OrphanageFirst,
        HospitalFirst
    }

    [Tooltip(
        "Choose which route to simulate when testing Orphanage directly."
    )]
    public EditorTestRoute editorTestRoute =
        EditorTestRoute.OrphanageFirst;

    private bool sequenceStarted = false;

    private void Start()
    {
        SetupRouteForEditorTesting();

        StartOpeningSequence();
    }

    // =====================================================
    // EDITOR TEST ROUTE
    // =====================================================

    private void SetupRouteForEditorTesting()
    {
        // A real route already exists.
        // This means the player came from Archive,
        // so do NOT overwrite it.
        if (MemoryRouteState.IsOrphanageFirst() ||
            MemoryRouteState.IsHospitalFirst())
        {
            Debug.Log(
                "REAL ROUTE DETECTED - Editor test route ignored."
            );

            return;
        }

        // No real route exists.
        if (!useEditorTestRoute)
        {
            Debug.LogWarning(
                "No Memory Route is set and Editor Test Route is disabled."
            );

            return;
        }

        // Simulate the selected route.
        if (editorTestRoute ==
            EditorTestRoute.OrphanageFirst)
        {
            MemoryRouteState.SetOrphanageFirst();

            Debug.Log(
                "EDITOR TEST ROUTE SET: Orphanage First"
            );
        }
        else
        {
            MemoryRouteState.SetHospitalFirst();

            Debug.Log(
                "EDITOR TEST ROUTE SET: Hospital First"
            );
        }
    }

    // =====================================================
    // OPENING
    // =====================================================

    public void StartOpeningSequence()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;

        StartCoroutine(
            OpeningSequence()
        );
    }

    private IEnumerator OpeningSequence()
    {
        AudioClip selectedOpening = null;

        // -------------------------------------------------
        // 1. Choose opening based on actual current route
        // -------------------------------------------------

        if (MemoryRouteState.IsOrphanageFirst())
        {
            selectedOpening =
                evieOpeningOrphanageFirst;

            Debug.Log(
                "ORPHANAGE OPENING: Orphanage First route"
            );
        }
        else if (MemoryRouteState.IsHospitalFirst())
        {
            selectedOpening =
                evieOpeningHospitalFirst;

            Debug.Log(
                "ORPHANAGE OPENING: Hospital First route"
            );
        }
        else
        {
            Debug.LogWarning(
                "OrphanageOpeningController: No route is currently set."
            );
        }

        // -------------------------------------------------
        // 2. Play Present Evie opening
        // -------------------------------------------------

        if (evieOpeningSource != null &&
            selectedOpening != null)
        {
            evieOpeningSource.clip =
                selectedOpening;

            evieOpeningSource.Play();

            yield return new WaitWhile(
                () => evieOpeningSource.isPlaying
            );
        }

        // -------------------------------------------------
        // 3. Brief pause
        // -------------------------------------------------

        if (delayBeforeMia > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeMia
            );
        }

        // -------------------------------------------------
        // 4. Mia calls from Bedroom
        // -------------------------------------------------

        if (miaHideAndSeekSource != null)
        {
            miaHideAndSeekSource.Play();
        }

        // -------------------------------------------------
        // 5. Bedroom Guide Light appears
        // -------------------------------------------------

        if (bedroomGuideLight != null)
        {
            bedroomGuideLight.FadeInGuideLight();
        }

        Debug.Log(
            "ORPHANAGE OPENING SEQUENCE COMPLETE"
        );
    }
}