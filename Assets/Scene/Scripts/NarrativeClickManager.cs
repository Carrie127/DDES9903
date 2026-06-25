using UnityEngine;
using TMPro;

public class NarrativeClickManager : MonoBehaviour
{
    public Camera playerCamera;
    public Transform inspectPoint;

    public GameObject narrationPanel;
    public TMP_Text narrationText;
    public AudioSource audioSource;

    public float clickDistance = 5f;

    private NarrativeItem currentItem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryClickObject();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrentItem();
        }
    }

    void TryClickObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, clickDistance))
        {
            NarrativeItem item = hit.collider.GetComponent<NarrativeItem>();

            if (item != null)
            {
                CloseCurrentItem();

                currentItem = item;
                currentItem.Inspect(inspectPoint, narrationPanel, narrationText, audioSource);
            }
        }
    }

    void CloseCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem.CloseInspect();
            currentItem = null;
        }

        narrationPanel.SetActive(false);

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}