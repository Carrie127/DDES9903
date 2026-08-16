using System.Collections;
using UnityEngine;

public class TRMemoryImageFade : MonoBehaviour
{
    [Header("Reveal Settings")]
    [SerializeField] private float fadeDuration = 1.2f;
    [SerializeField] private float startScale = 0.94f;

    private MeshRenderer meshRenderer;
    private Material runtimeMaterial;

    private Vector3 finalScale;
    private Coroutine revealCoroutine;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            runtimeMaterial = meshRenderer.material;

            Color color = runtimeMaterial.color;
            color.a = 0f;
            runtimeMaterial.color = color;
        }

        finalScale = transform.localScale;
        transform.localScale = finalScale * startScale;
    }

    public void PlayReveal()
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        revealCoroutine = StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        if (meshRenderer == null || runtimeMaterial == null)
            yield break;

        float elapsed = 0f;

        Color startColor = runtimeMaterial.color;
        startColor.a = 0f;

        Color endColor = startColor;
        endColor.a = 1f;

        Vector3 smallScale = finalScale * startScale;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Smooth transition instead of linear fade
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            runtimeMaterial.color =
                Color.Lerp(startColor, endColor, smoothT);

            transform.localScale =
                Vector3.Lerp(smallScale, finalScale, smoothT);

            yield return null;
        }

        runtimeMaterial.color = endColor;
        transform.localScale = finalScale;

        revealCoroutine = null;
    }
}