using UnityEngine;

public class TRFinalEndingTrigger : MonoBehaviour
{
    public enum EndingType
    {
        Accept,
        Reject,
        Rewrite
    }

    [Header("Ending")]
    [SerializeField] private EndingType endingType;

    [Header("Controller")]
    [SerializeField] private TRFinalEndingController endingController;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        // Works with a normal tagged Player
        // or a CharacterController-based player rig.
        bool isPlayer =
            other.CompareTag("Player") ||
            other.GetComponentInParent<CharacterController>() != null;

        if (!isPlayer)
            return;

        hasTriggered = true;

        if (endingController != null)
        {
            endingController.BeginEnding(endingType);
        }
    }
}