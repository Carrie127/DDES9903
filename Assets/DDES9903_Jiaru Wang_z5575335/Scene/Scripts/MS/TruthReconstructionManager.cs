using System.Collections;
using UnityEngine;

public class TruthReconstructionManager : MonoBehaviour
{
    [Header("Memory Sequence")]
    [SerializeField] private GameObject[] memoryImages;
    [SerializeField] private GameObject[] memoryInteractAreas;

    [Header("Memory Audio")]
    [SerializeField] private AudioSource fireAmbience;
    [SerializeField] private AudioSource[] memoryAudios;

    [Header("Memory 04")]
    [SerializeField] private AudioSource memory04Collapse;
    [SerializeField] private float collapseDelay = 2f;

    [Header("Mia Truth")]
    [SerializeField] private AudioSource miaTruth;
    [SerializeField] private float fireFadeDuration = 1.2f;
    [SerializeField] private float miaStartDelay = 0.4f;

    [Header("Present Evie")]
    [SerializeField] private AudioSource presentEvieTruth;
    [SerializeField] private float presentEvieDelayAfterMia = 1.3f;

    [Header("Final Choice")]
    [SerializeField] private TRFinalChoiceController finalChoiceController;
    [SerializeField] private float finalChoiceDelayAfterEvie = 0.8f;

    [Header("Timing")]
    [SerializeField] private float firstMemoryDelay = 0.5f;
    [SerializeField] private float nextMemoryDelay = 0.35f;
    [SerializeField] private float interactionEnableDelay = 1.25f;

    private bool hasStarted = false;
    private bool isRevealing = false;
    private int currentMemoryIndex = -1;

    private void Awake()
    {
        foreach (GameObject image in memoryImages)
        {
            if (image != null)
                image.SetActive(false);
        }

        foreach (GameObject area in memoryInteractAreas)
        {
            if (area != null)
                area.SetActive(false);
        }

        if (fireAmbience != null)
            fireAmbience.Stop();

        if (memory04Collapse != null)
            memory04Collapse.Stop();

        if (miaTruth != null)
            miaTruth.Stop();

        if (presentEvieTruth != null)
            presentEvieTruth.Stop();

        foreach (AudioSource audio in memoryAudios)
        {
            if (audio != null)
                audio.Stop();
        }
    }

    public void BeginTruthReconstruction()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        Debug.Log("TRUTH RECONSTRUCTION → STARTED");

        if (fireAmbience != null)
        {
            fireAmbience.loop = true;
            fireAmbience.Play();
        }

        StartCoroutine(RevealNextMemory(firstMemoryDelay));
    }

    public void AdvanceMemory()
    {
        if (!hasStarted || isRevealing)
            return;

        if (currentMemoryIndex >= 0 &&
            currentMemoryIndex < memoryInteractAreas.Length &&
            memoryInteractAreas[currentMemoryIndex] != null)
        {
            memoryInteractAreas[currentMemoryIndex].SetActive(false);
        }

        if (currentMemoryIndex >= memoryImages.Length - 1)
        {
            Debug.Log(
                "TRUTH RECONSTRUCTION → ALL MISSING MEMORIES REVEALED"
            );

            StartCoroutine(FinishTruthReconstruction());
            return;
        }

        StartCoroutine(RevealNextMemory(nextMemoryDelay));
    }

    private IEnumerator RevealNextMemory(float delay)
    {
        isRevealing = true;

        yield return new WaitForSeconds(delay);

        currentMemoryIndex++;

        if (currentMemoryIndex >= memoryImages.Length)
        {
            isRevealing = false;
            yield break;
        }

        GameObject image = memoryImages[currentMemoryIndex];

        if (image != null)
        {
            image.SetActive(true);

            TRMemoryImageFade fade =
                image.GetComponent<TRMemoryImageFade>();

            if (fade != null)
                fade.PlayReveal();

            Debug.Log(
                "TRUTH RECONSTRUCTION → MEMORY " +
                (currentMemoryIndex + 1) +
                " REVEALED"
            );
        }

        if (currentMemoryIndex < memoryAudios.Length &&
            memoryAudios[currentMemoryIndex] != null)
        {
            memoryAudios[currentMemoryIndex].Play();
        }

        if (currentMemoryIndex == memoryImages.Length - 1 &&
            memory04Collapse != null)
        {
            memory04Collapse.PlayDelayed(collapseDelay);
        }

        float waitTime = GetInteractionDelay(currentMemoryIndex);

        yield return new WaitForSeconds(waitTime);

        if (currentMemoryIndex < memoryInteractAreas.Length &&
            memoryInteractAreas[currentMemoryIndex] != null)
        {
            memoryInteractAreas[currentMemoryIndex].SetActive(true);
        }

        isRevealing = false;
    }

    private float GetInteractionDelay(int index)
    {
        float delay = interactionEnableDelay;

        if (index < memoryAudios.Length &&
            memoryAudios[index] != null &&
            memoryAudios[index].clip != null)
        {
            delay = Mathf.Max(
                delay,
                memoryAudios[index].clip.length
            );
        }

        if (index == memoryImages.Length - 1 &&
            memory04Collapse != null &&
            memory04Collapse.clip != null)
        {
            delay = Mathf.Max(
                delay,
                collapseDelay + memory04Collapse.clip.length
            );
        }

        return delay;
    }

    private IEnumerator FinishTruthReconstruction()
    {
        isRevealing = true;

        // 1. 火场环境声渐弱
        if (fireAmbience != null && fireAmbience.isPlaying)
        {
            float startVolume = fireAmbience.volume;
            float elapsed = 0f;

            while (elapsed < fireFadeDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    elapsed / fireFadeDuration
                );

                fireAmbience.volume =
                    Mathf.Lerp(startVolume, 0f, t);

                yield return null;
            }

            fireAmbience.Stop();
            fireAmbience.volume = startVolume;
        }

        // 2. Mia Truth
        yield return new WaitForSeconds(miaStartDelay);

        if (miaTruth != null)
        {
            miaTruth.Play();

            Debug.Log(
                "TRUTH RECONSTRUCTION → MIA TRUTH"
            );

            while (miaTruth.isPlaying)
                yield return null;
        }

        // 3. Mia 说完后停顿
        yield return new WaitForSeconds(
            presentEvieDelayAfterMia
        );

        // 4. Present Evie 回应
        if (presentEvieTruth != null)
        {
            presentEvieTruth.Play();

            Debug.Log(
                "TRUTH RECONSTRUCTION → PRESENT EVIE TRUTH"
            );

            while (presentEvieTruth.isPlaying)
                yield return null;
        }

        // 5. Evie 说完后再停一下
        yield return new WaitForSeconds(
            finalChoiceDelayAfterEvie
        );

        // 6. Final Choice
        if (finalChoiceController != null)
        {
            finalChoiceController.BeginFinalChoiceSequence();

            Debug.Log(
                "TRUTH RECONSTRUCTION → FINAL CHOICE"
            );
        }

        isRevealing = false;
    }
}