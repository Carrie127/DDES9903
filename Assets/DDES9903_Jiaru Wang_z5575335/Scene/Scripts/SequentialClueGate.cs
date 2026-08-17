using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class SequentialClueGate : MonoBehaviour
{
    [Header("Initial State")]
    [Tooltip("Enable only for the first clue in the sequence.")]
    [SerializeField] private bool unlockedAtStart = false;

    [Header("Interaction Components")]
    [Tooltip("Collider(s) used for player interaction.")]
    [SerializeField] private Collider[] interactionColliders;

    [Tooltip("Scripts that allow interaction, such as Holdable or InteractableGeneral.")]
    [SerializeField] private MonoBehaviour[] interactionScripts;

    [Header("Guide / Highlight Objects")]
    [Tooltip("Optional. Drag guide lights, highlight icons, or visual hints that should be hidden while this clue is locked.")]
    [SerializeField] private GameObject[] guideObjects;

    private bool isUnlocked = false;

    private void Awake()
    {
        if (unlockedAtStart)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    // =====================================================
    // LOCK
    // =====================================================

    public void Lock()
    {
        isUnlocked = false;

        if (interactionColliders != null)
        {
            foreach (Collider col in interactionColliders)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        if (interactionScripts != null)
        {
            foreach (MonoBehaviour script in interactionScripts)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        if (guideObjects != null)
        {
            foreach (GameObject guide in guideObjects)
            {
                if (guide != null)
                    guide.SetActive(false);
            }
        }

        Debug.Log(gameObject.name + " → CLUE LOCKED");
    }

    // =====================================================
    // UNLOCK
    // =====================================================

    public void Unlock()
    {
        isUnlocked = true;

        if (interactionColliders != null)
        {
            foreach (Collider col in interactionColliders)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        if (interactionScripts != null)
        {
            foreach (MonoBehaviour script in interactionScripts)
            {
                if (script != null)
                    script.enabled = true;
            }
        }

        if (guideObjects != null)
        {
            foreach (GameObject guide in guideObjects)
            {
                if (guide != null)
                    guide.SetActive(true);
            }
        }

        Debug.Log(gameObject.name + " → CLUE UNLOCKED");
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}