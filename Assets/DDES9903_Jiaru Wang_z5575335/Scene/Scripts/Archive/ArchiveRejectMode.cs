using UnityEngine;

public class ArchiveRejectMode : MonoBehaviour
{
    [Header("Normal Archive Interactions")]
    [Tooltip(
        "Drag the clue / interaction root objects here. " +
        "Their visuals will stay, but interaction components will be disabled."
    )]
    [SerializeField] private GameObject[] normalInteractionObjects;

    [Header("Normal Archive Systems")]
    [Tooltip(
        "Managers used only for the normal Archive narrative / route progression."
    )]
    [SerializeField] private GameObject[] normalSystemsToDisable;

    [Header("Reject Ending Content")]
    [SerializeField] private GameObject rejectEndingRoot;

    private void Awake()
    {
        if (FinalEndingState.IsReject())
        {
            EnterRejectMode();
        }
        else
        {
            EnterNormalMode();
        }
    }

    // =====================================================
    // REJECT MODE
    // =====================================================

    private void EnterRejectMode()
    {
        // Keep clue objects visible,
        // but remove their ability to interact.
        if (normalInteractionObjects != null)
        {
            foreach (GameObject obj in normalInteractionObjects)
            {
                if (obj != null)
                    DisableInteraction(obj);
            }
        }

        // Normal narrative / route systems can be
        // completely disabled because they have no visuals.
        if (normalSystemsToDisable != null)
        {
            foreach (GameObject system in normalSystemsToDisable)
            {
                if (system != null)
                    system.SetActive(false);
            }
        }

        if (rejectEndingRoot != null)
            rejectEndingRoot.SetActive(true);

        Debug.Log(
            "ARCHIVE → REJECT ENDING MODE"
        );
    }

    // =====================================================
    // NORMAL MODE
    // =====================================================

    private void EnterNormalMode()
    {
        if (rejectEndingRoot != null)
            rejectEndingRoot.SetActive(false);

        Debug.Log(
            "ARCHIVE → NORMAL MODE"
        );
    }

    // =====================================================
    // DISABLE INTERACTION WITHOUT HIDING VISUALS
    // =====================================================

    private void DisableInteraction(GameObject root)
    {
        // ---------------------------------------------
        // 1. Disable interaction colliders
        // ---------------------------------------------

        Collider[] colliders =
            root.GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // ---------------------------------------------
        // 2. Disable known interaction behaviours
        //    without touching MeshRenderer / visuals
        // ---------------------------------------------

        MonoBehaviour[] behaviours =
            root.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null)
                continue;

            string typeName =
                behaviour.GetType().Name;

            if (
                typeName == "InteractableGeneral" ||
                typeName == "NarrativeItem"
            )
            {
                behaviour.enabled = false;
            }
        }

        Debug.Log(
            "ARCHIVE REJECT → INTERACTION DISABLED: "
            + root.name
        );
    }
}