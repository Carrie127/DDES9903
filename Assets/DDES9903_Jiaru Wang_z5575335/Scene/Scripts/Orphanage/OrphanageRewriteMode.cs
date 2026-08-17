using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class OrphanageRewriteMode : MonoBehaviour
{
    // =====================================================
    // REWRITE CONTENT
    // =====================================================

    [Header("Rewrite Ending Content")]
    [SerializeField] private GameObject rewriteEndingRoot;


    // =====================================================
    // REWRITE INTRO
    // =====================================================

    [Header("Rewrite Intro - Evie")]
    [SerializeField] private AudioSource evieRewriteIntroAudio;

    [Tooltip("Delay after entering the burning Orphanage before Evie speaks.")]
    [SerializeField] private float rewriteIntroDelay = 1.2f;


    // =====================================================
    // NORMAL ORPHANAGE INTRO
    // =====================================================

    [Header("Normal Intro - Disable During Rewrite")]

    [Tooltip("Drag the normal Orphanage intro scripts here. They will be disabled ONLY during Rewrite.")]
    [SerializeField] private MonoBehaviour[] normalIntroScriptsToDisable;

    [Tooltip("Drag normal intro AudioSources here so they are explicitly stopped during Rewrite.")]
    [SerializeField] private AudioSource[] normalIntroAudioSourcesToStop;

    [Tooltip("Optional: drag dedicated normal intro Animators here if the intro animation can run by itself.")]
    [SerializeField] private Animator[] normalIntroAnimatorsToDisable;


    // =====================================================
    // LIGHTING
    // =====================================================

    [Header("Lights To Turn Off")]

    [Tooltip("Normal Orphanage lights that should completely turn off during Rewrite.")]
    [SerializeField] private Light[] lightsToDisable;


    [Header("Lights To Dim")]

    [Tooltip("Lights that stay on but become darker during Rewrite.")]
    [SerializeField] private Light[] lightsToDim;

    [Range(0f, 1f)]
    [SerializeField] private float dimMultiplier = 0.3f;


    // =====================================================
    // INTERNAL STATE
    // =====================================================

    private float[] originalDimIntensities;

    private bool[] originalDisabledLightStates;
    private float[] originalDisabledLightIntensities;

    private Coroutine rewriteIntroCoroutine;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        CacheOriginalLightValues();

        if (FinalEndingState.IsRewrite())
        {
            EnterRewriteMode();
        }
        else
        {
            EnterNormalMode();
        }
    }


    // =====================================================
    // CACHE LIGHT VALUES
    // =====================================================

    private void CacheOriginalLightValues()
    {
        // -------------------------
        // Lights that will be dimmed
        // -------------------------

        if (lightsToDim != null)
        {
            originalDimIntensities =
                new float[lightsToDim.Length];

            for (int i = 0; i < lightsToDim.Length; i++)
            {
                if (lightsToDim[i] != null)
                {
                    originalDimIntensities[i] =
                        lightsToDim[i].intensity;
                }
            }
        }


        // -------------------------
        // Lights that will be disabled
        // -------------------------

        if (lightsToDisable != null)
        {
            originalDisabledLightStates =
                new bool[lightsToDisable.Length];

            originalDisabledLightIntensities =
                new float[lightsToDisable.Length];

            for (int i = 0; i < lightsToDisable.Length; i++)
            {
                if (lightsToDisable[i] != null)
                {
                    originalDisabledLightStates[i] =
                        lightsToDisable[i].enabled;

                    originalDisabledLightIntensities[i] =
                        lightsToDisable[i].intensity;
                }
            }
        }
    }


    // =====================================================
    // REWRITE MODE
    // =====================================================

    private void EnterRewriteMode()
    {
        Debug.Log(
            "ORPHANAGE → REWRITE ENDING MODE"
        );


        // -------------------------------------------------
        // 1. Stop normal Orphanage intro
        // -------------------------------------------------

        DisableNormalIntro();


        // -------------------------------------------------
        // 2. Activate Rewrite-only content
        //    Fire / smoke / ending objects
        // -------------------------------------------------

        if (rewriteEndingRoot != null)
        {
            rewriteEndingRoot.SetActive(true);
        }


        // -------------------------------------------------
        // 3. Turn selected normal lights off
        // -------------------------------------------------

        if (lightsToDisable != null)
        {
            foreach (Light targetLight in lightsToDisable)
            {
                if (targetLight != null)
                {
                    targetLight.enabled = false;
                }
            }
        }


        // -------------------------------------------------
        // 4. Dim selected normal lights
        // -------------------------------------------------

        if (lightsToDim != null &&
            originalDimIntensities != null)
        {
            for (int i = 0; i < lightsToDim.Length; i++)
            {
                Light targetLight =
                    lightsToDim[i];

                if (targetLight == null)
                    continue;

                targetLight.enabled = true;

                targetLight.intensity =
                    originalDimIntensities[i] *
                    dimMultiplier;
            }
        }


        // -------------------------------------------------
        // 5. Make sure Rewrite Evie audio is not
        //    already playing
        // -------------------------------------------------

        if (evieRewriteIntroAudio != null)
        {
            evieRewriteIntroAudio.Stop();
        }


        // -------------------------------------------------
        // 6. Play fire-version Evie intro
        // -------------------------------------------------

        rewriteIntroCoroutine =
            StartCoroutine(
                PlayRewriteIntro()
            );
    }


    // =====================================================
    // NORMAL MODE
    // =====================================================

    private void EnterNormalMode()
    {
        Debug.Log(
            "ORPHANAGE → NORMAL MODE"
        );


        // Rewrite fire/smoke stays hidden.
        if (rewriteEndingRoot != null)
        {
            rewriteEndingRoot.SetActive(false);
        }


        // IMPORTANT:
        // Do NOT disable or modify the normal intro here.
        // Normal Orphanage scripts continue exactly as before.
    }


    // =====================================================
    // DISABLE NORMAL INTRO
    // =====================================================

    private void DisableNormalIntro()
    {
        // -------------------------------------------------
        // Disable normal intro scripts
        // -------------------------------------------------

        if (normalIntroScriptsToDisable != null)
        {
            foreach (
                MonoBehaviour introScript
                in normalIntroScriptsToDisable)
            {
                if (introScript != null)
                {
                    introScript.enabled = false;
                }
            }
        }


        // -------------------------------------------------
        // Stop normal intro audio immediately
        // -------------------------------------------------

        if (normalIntroAudioSourcesToStop != null)
        {
            foreach (
                AudioSource introAudio
                in normalIntroAudioSourcesToStop)
            {
                if (introAudio != null)
                {
                    introAudio.Stop();
                }
            }
        }


        // -------------------------------------------------
        // Disable dedicated normal intro Animators
        // -------------------------------------------------

        if (normalIntroAnimatorsToDisable != null)
        {
            foreach (
                Animator introAnimator
                in normalIntroAnimatorsToDisable)
            {
                if (introAnimator != null)
                {
                    introAnimator.enabled = false;
                }
            }
        }

        Debug.Log(
            "ORPHANAGE REWRITE → NORMAL INTRO DISABLED"
        );
    }


    // =====================================================
    // REWRITE INTRO
    // =====================================================

    private IEnumerator PlayRewriteIntro()
    {
        yield return new WaitForSeconds(
            rewriteIntroDelay
        );

        if (evieRewriteIntroAudio != null)
        {
            evieRewriteIntroAudio.Play();

            Debug.Log(
                "ORPHANAGE REWRITE → EVIE INTRO PLAYED"
            );
        }
        else
        {
            Debug.LogWarning(
                "ORPHANAGE REWRITE → Evie Rewrite Intro Audio is not assigned!"
            );
        }

        rewriteIntroCoroutine = null;
    }


    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDisable()
    {
        if (rewriteIntroCoroutine != null)
        {
            StopCoroutine(
                rewriteIntroCoroutine
            );

            rewriteIntroCoroutine = null;
        }
    }
}