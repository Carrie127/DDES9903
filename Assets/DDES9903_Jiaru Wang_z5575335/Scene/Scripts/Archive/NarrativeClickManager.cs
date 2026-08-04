using UnityEngine;

public class NarrativeClickManager : MonoBehaviour
{
    public Camera playerCamera;
    public Transform inspectPoint;
    public AudioSource audioSource;
    public float clickDistance = 5f;

    private bool isInteractionLocked = false;

    void Update()
    {
        if (isInteractionLocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryClickObject();
        }
    }

    void TryClickObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, clickDistance))
        {
            NarrativeItem item = hit.collider.GetComponentInParent<NarrativeItem>();

            if (item != null)
            {
                isInteractionLocked = true;

                item.Inspect(
                    inspectPoint,
                    audioSource,
                    OnItemFinished
                );
            }
        }
    }

    void OnItemFinished()
    {
        isInteractionLocked = false;
    }
}