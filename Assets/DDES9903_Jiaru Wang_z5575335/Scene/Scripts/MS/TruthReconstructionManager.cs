using System.Collections;
using UnityEngine;

public class TruthReconstructionManager : MonoBehaviour
{
    [Header("Memory 01")]
    [SerializeField] private GameObject memoryImage01;
    [SerializeField] private GameObject memoryInteractArea01;

    [Header("Memory 02")]
    [SerializeField] private GameObject memoryImage02;
    [SerializeField] private GameObject memoryInteractArea02;

    [Header("Timing")]
    [SerializeField] private float firstMemoryDelay = 0.5f;
    [SerializeField] private float nextMemoryDelay = 0.35f;
    [SerializeField] private float interactionEnableDelay = 1.25f;

    private bool hasStarted = false;
    private bool memory02Revealed = false;

    private void Awake()
    {
        // Memory 01
        if (memoryImage01 != null)
            memoryImage01.SetActive(false);

        if (memoryInteractArea01 != null)
            memoryInteractArea01.SetActive(false);

        // Memory 02
        if (memoryImage02 != null)
            memoryImage02.SetActive(false);

        if (memoryInteractArea02 != null)
            memoryInteractArea02.SetActive(false);
    }

    // Called by Touch 7:42
    public void BeginTruthReconstruction()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        Debug.Log("TRUTH RECONSTRUCTION → STARTED");

        StartCoroutine(RevealMemory01Sequence());
    }

    private IEnumerator RevealMemory01Sequence()
    {
        yield return new WaitForSeconds(firstMemoryDelay);

        if (memoryImage01 != null)
        {
            memoryImage01.SetActive(true);

            TRMemoryImageFade fade =
                memoryImage01.GetComponent<TRMemoryImageFade>();

            if (fade != null)
                fade.PlayReveal();

            Debug.Log("TRUTH RECONSTRUCTION → MEMORY 01 REVEALED");
        }

        // 等淡入基本完成后，才允许 Recall
        yield return new WaitForSeconds(interactionEnableDelay);

        if (memoryInteractArea01 != null)
            memoryInteractArea01.SetActive(true);
    }

    // Called by Recall on Memory 01
    public void RevealMemory02()
    {
        if (!hasStarted || memory02Revealed)
            return;

        memory02Revealed = true;

        // Memory 01 已经被 Recall，不再允许重复点击
        if (memoryInteractArea01 != null)
            memoryInteractArea01.SetActive(false);

        StartCoroutine(RevealMemory02Sequence());
    }

    private IEnumerator RevealMemory02Sequence()
    {
        yield return new WaitForSeconds(nextMemoryDelay);

        if (memoryImage02 != null)
        {
            memoryImage02.SetActive(true);

            TRMemoryImageFade fade =
                memoryImage02.GetComponent<TRMemoryImageFade>();

            if (fade != null)
                fade.PlayReveal();

            Debug.Log("TRUTH RECONSTRUCTION → MEMORY 02 REVEALED");
        }

        // 等 Image 02 浮现完成，再开启它自己的 Recall
        yield return new WaitForSeconds(interactionEnableDelay);

        if (memoryInteractArea02 != null)
            memoryInteractArea02.SetActive(true);
    }
}