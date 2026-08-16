using System.Collections;
using UnityEngine;

public class TruthReconstructionReveal : MonoBehaviour
{
    [Header("Memory Veil")]
    [SerializeField] private GameObject memoryVeil;

    [Header("Truth Reconstruction")]
    [SerializeField] private GameObject timeDisplay;
    [SerializeField] private Light[] truthLights;

    [Header("Reveal Timing")]
    [SerializeField] private float revealDelay = 0.4f;
    [SerializeField] private float veilFadeDuration = 1.5f;

    private MeshRenderer veilRenderer;
    private Collider veilCollider;
    private Material veilMaterial;

    private bool hasRevealed = false;

    private void Awake()
    {
        if (memoryVeil != null)
        {
            veilRenderer = memoryVeil.GetComponent<MeshRenderer>();
            veilCollider = memoryVeil.GetComponent<Collider>();

            if (veilRenderer != null)
                veilMaterial = veilRenderer.material;
        }

        // Safety: make sure TR starts hidden
        if (timeDisplay != null)
            timeDisplay.SetActive(false);

        if (truthLights != null)
        {
            foreach (Light lightSource in truthLights)
            {
                if (lightSource != null)
                    lightSource.enabled = false;
            }
        }
    }

    public void RevealTruthReconstruction()
    {
        if (hasRevealed)
            return;

        hasRevealed = true;
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        yield return new WaitForSeconds(revealDelay);

        // Truth begins to appear behind the veil
        if (timeDisplay != null)
            timeDisplay.SetActive(true);

        if (truthLights != null)
        {
            foreach (Light lightSource in truthLights)
            {
                if (lightSource != null)
                    lightSource.enabled = true;
            }
        }

        // Fade the black veil away
        if (veilMaterial != null)
        {
            Color startColor = veilMaterial.color;
            Color endColor = startColor;
            endColor.a = 0f;

            float elapsed = 0f;

            while (elapsed < veilFadeDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / veilFadeDuration);
                veilMaterial.color = Color.Lerp(startColor, endColor, t);

                yield return null;
            }

            veilMaterial.color = endColor;
        }

        // Only after the veil is visually gone can the player pass through
        if (veilCollider != null)
            veilCollider.enabled = false;

        if (veilRenderer != null)
            veilRenderer.enabled = false;
    }
}