using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRFinalChoiceController : MonoBehaviour
{
    [Header("Memory Images")]
    [SerializeField] private GameObject[] memoryImages;

    [SerializeField]
    private GameObject[] memoryInteractAreas;

    [Header("Final Question")]
    [SerializeField] private CanvasGroup finalQuestionCanvasGroup;

    [Header("Ending Portals")]
    [SerializeField]
    private TRPortalRevealGroup[] portalGroups;

    [Header("Timing - Memory")]
    [SerializeField] private float startDelay = 0.25f;
    [SerializeField] private float memoryFadeDuration = 1.2f;
    [SerializeField] private float afterMemoryFadeDelay = 0.35f;

    [Header("Timing - Question")]
    [SerializeField] private float questionFadeDuration = 0.8f;
    [SerializeField] private float questionHoldBeforePortals = 1.0f;

    [Header("Timing - Portals")]
    [SerializeField] private float portalStagger = 0.15f;

    private bool hasStarted = false;

    private class MaterialFadeData
    {
        public Material material;

        public bool hasBaseColor;
        public bool hasColor;
        public bool hasTintColor;

        public Color baseColor;
        public Color color;
        public Color tintColor;
    }

    private void Awake()
    {
        // ---------------------------------------------
        // Question hidden at the beginning
        // ---------------------------------------------

        if (finalQuestionCanvasGroup != null)
        {
            finalQuestionCanvasGroup.alpha = 0f;
            finalQuestionCanvasGroup.interactable = false;
            finalQuestionCanvasGroup.blocksRaycasts = false;
        }

        // ---------------------------------------------
        // Ending portals hidden at the beginning
        // ---------------------------------------------

        if (portalGroups != null)
        {
            foreach (TRPortalRevealGroup portal in portalGroups)
            {
                if (portal != null)
                    portal.PrepareHiddenState();
            }
        }
    }

    public void BeginFinalChoiceSequence()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        StartCoroutine(FinalChoiceSequence());
    }

    private IEnumerator FinalChoiceSequence()
    {
        yield return new WaitForSeconds(startDelay);

        // =============================================
        // 1. Disable all Memory interactions
        // =============================================

        if (memoryInteractAreas != null)
        {
            foreach (GameObject area in memoryInteractAreas)
            {
                if (area != null)
                    area.SetActive(false);
            }
        }

        // =============================================
        // 2. Collect Memory Image materials
        // =============================================

        List<MaterialFadeData> materials =
            CollectMemoryMaterials();

        // =============================================
        // 3. Fade out all four Memory Images
        // =============================================

        float elapsed = 0f;

        while (elapsed < memoryFadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / memoryFadeDuration
                );

            float alpha = 1f - t;

            foreach (MaterialFadeData data in materials)
            {
                SetMaterialAlpha(data, alpha);
            }

            yield return null;
        }

        foreach (MaterialFadeData data in materials)
        {
            SetMaterialAlpha(data, 0f);
        }

        // Fully disable images
        if (memoryImages != null)
        {
            foreach (GameObject image in memoryImages)
            {
                if (image != null)
                    image.SetActive(false);
            }
        }

        Debug.Log(
            "FINAL CHOICE → MEMORIES FADED"
        );

        // =============================================
        // 4. Short pause
        // =============================================

        yield return new WaitForSeconds(
            afterMemoryFadeDelay
        );

        // =============================================
        // 5. Reveal final question
        // =============================================

        if (finalQuestionCanvasGroup != null)
        {
            elapsed = 0f;

            while (elapsed < questionFadeDuration)
            {
                elapsed += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed / questionFadeDuration
                    );

                float smoothT =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        t
                    );

                finalQuestionCanvasGroup.alpha =
                    smoothT;

                yield return null;
            }

            finalQuestionCanvasGroup.alpha = 1f;
        }

        Debug.Log(
            "FINAL CHOICE → QUESTION REVEALED"
        );

        // =============================================
        // 6. Let player read the question
        // =============================================

        yield return new WaitForSeconds(
            questionHoldBeforePortals
        );

        // =============================================
        // 7. Reveal three Ending portals
        // =============================================

        if (portalGroups != null)
        {
            for (int i = 0;
                 i < portalGroups.Length;
                 i++)
            {
                if (portalGroups[i] != null)
                    portalGroups[i].Reveal();

                if (i < portalGroups.Length - 1)
                {
                    yield return new WaitForSeconds(
                        portalStagger
                    );
                }
            }
        }

        Debug.Log(
            "FINAL CHOICE → PORTALS REVEALED"
        );
    }

    // =====================================================
    // MEMORY MATERIAL COLLECTION
    // =====================================================

    private List<MaterialFadeData>
        CollectMemoryMaterials()
    {
        List<MaterialFadeData> result =
            new List<MaterialFadeData>();

        if (memoryImages == null)
            return result;

        foreach (GameObject image in memoryImages)
        {
            if (image == null)
                continue;

            Renderer renderer =
                image.GetComponent<Renderer>();

            if (renderer == null)
                continue;

            Material[] runtimeMaterials =
                renderer.materials;

            foreach (Material mat in runtimeMaterials)
            {
                if (mat == null)
                    continue;

                MaterialFadeData data =
                    new MaterialFadeData();

                data.material = mat;

                data.hasBaseColor =
                    mat.HasProperty("_BaseColor");

                data.hasColor =
                    mat.HasProperty("_Color");

                data.hasTintColor =
                    mat.HasProperty("_TintColor");

                if (data.hasBaseColor)
                {
                    data.baseColor =
                        mat.GetColor("_BaseColor");
                }

                if (data.hasColor)
                {
                    data.color =
                        mat.GetColor("_Color");
                }

                if (data.hasTintColor)
                {
                    data.tintColor =
                        mat.GetColor("_TintColor");
                }

                result.Add(data);
            }
        }

        return result;
    }

    // =====================================================
    // MEMORY MATERIAL FADE
    // =====================================================

    private void SetMaterialAlpha(
        MaterialFadeData data,
        float multiplier)
    {
        if (data == null ||
            data.material == null)
        {
            return;
        }

        if (data.hasBaseColor)
        {
            Color c = data.baseColor;
            c.a *= multiplier;

            data.material.SetColor(
                "_BaseColor",
                c
            );
        }

        if (data.hasColor)
        {
            Color c = data.color;
            c.a *= multiplier;

            data.material.SetColor(
                "_Color",
                c
            );
        }

        if (data.hasTintColor)
        {
            Color c = data.tintColor;
            c.a *= multiplier;

            data.material.SetColor(
                "_TintColor",
                c
            );
        }
    }
}