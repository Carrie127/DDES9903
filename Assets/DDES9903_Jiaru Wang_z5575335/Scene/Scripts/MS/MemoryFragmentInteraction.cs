using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MemoryFragmentInteraction : MonoBehaviour
{
    [Header("Fragment Identity")]
    [Tooltip(
        "Fragment_01 = 0, Fragment_02 = 1, Fragment_03 = 2, Fragment_04 = 3, Fragment_05 = 4"
    )]
    [SerializeField] private int fragmentIndex = 0;

    [SerializeField] private MemoryCorridorManager corridorManager;


    [Header("Truth Content - Audio Optional")]
    [Tooltip(
        "2D AudioSource used to play this Fragment's truth memory."
    )]
    [SerializeField] private AudioSource memoryAudioSource;

    [Tooltip(
        "The truth memory AudioClip for this Fragment. Leave empty while testing."
    )]
    [SerializeField] private AudioClip memoryClip;


    [Header("Fragment Light")]
    [Tooltip(
        "The local Point Light illuminating this Fragment."
    )]
    [SerializeField] private Light fragmentLight;

    [Tooltip(
        "Light intensity after this Fragment has been completed."
    )]
    [SerializeField] private float completedLightIntensity = 0.2f;

    [Tooltip(
        "How long the Fragment light takes to fade to its completed intensity."
    )]
    [SerializeField] private float lightFadeDuration = 0.6f;


    [Header("Temporary Testing")]
    [Tooltip(
        "If no AudioClip is assigned, wait this long before completing the Fragment."
    )]
    [SerializeField] private float fallbackDurationWithoutAudio = 1.0f;


    [Header("Optional Content Events")]
    [Tooltip(
        "Called when this Fragment's truth sequence begins."
    )]
    public UnityEvent onFragmentStarted;

    [Tooltip(
        "Called when this Fragment's truth sequence has completely finished."
    )]
    public UnityEvent onFragmentCompleted;


    private bool hasTriggered = false;
    private bool hasCompleted = false;


    // =====================================================
    // PLAYER INTERACTION
    // =====================================================

    public void InspectFragment()
    {
        // Prevent repeated interaction.
        if (hasTriggered || hasCompleted)
            return;


        if (corridorManager == null)
        {
            Debug.LogWarning(
                name + ": MemoryCorridorManager is missing."
            );

            return;
        }


        // Only the Fragment currently selected by
        // MemoryCorridorManager may be activated.
        if (!corridorManager.IsCurrentFragment(fragmentIndex))
        {
            Debug.Log(
                name +
                ": This Fragment is not currently active."
            );

            return;
        }


        hasTriggered = true;

        StartCoroutine(
            FragmentSequence()
        );
    }


    // =====================================================
    // MAIN FRAGMENT SEQUENCE
    // =====================================================

    private IEnumerator FragmentSequence()
    {
        Debug.Log(
            "MEMORY FRAGMENT " +
            (fragmentIndex + 1) +
            " TRIGGERED"
        );


        // -------------------------------------------------
        // 1. Start optional additional content.
        //
        // Later this can trigger:
        // slideshow / animation / particles / etc.
        // -------------------------------------------------

        onFragmentStarted?.Invoke();


        // -------------------------------------------------
        // 2. Play Truth Memory audio
        // -------------------------------------------------

        if (
            memoryAudioSource != null &&
            memoryClip != null
        )
        {
            memoryAudioSource.Stop();

            memoryAudioSource.clip =
                memoryClip;

            memoryAudioSource.loop =
                false;

            memoryAudioSource.Play();


            yield return new WaitWhile(
                () => memoryAudioSource.isPlaying
            );
        }

        // -------------------------------------------------
        // TEMPORARY TEST MODE
        //
        // Until the real Truth Memory audio exists,
        // wait briefly so the progression can be tested.
        // -------------------------------------------------

        else if (fallbackDurationWithoutAudio > 0f)
        {
            yield return new WaitForSeconds(
                fallbackDurationWithoutAudio
            );
        }


        // -------------------------------------------------
        // 3. Completed Fragment visual state
        //
        // IMPORTANT:
        // We DO NOT hide the white Frame.
        // We DO NOT hide the evidence.
        // We DO NOT hide the Back.
        //
        // The complete Fragment remains visible.
        //
        // Only its local light becomes dimmer.
        // -------------------------------------------------

        if (fragmentLight != null)
        {
            yield return StartCoroutine(
                FadeLightToCompletedState()
            );
        }


        // -------------------------------------------------
        // 4. Mark Fragment as completed
        // -------------------------------------------------

        hasCompleted = true;


        onFragmentCompleted?.Invoke();


        Debug.Log(
            "MEMORY FRAGMENT " +
            (fragmentIndex + 1) +
            " COMPLETE"
        );


        // -------------------------------------------------
        // 5. Tell MemoryCorridorManager
        //    to reveal the next Fragment.
        // -------------------------------------------------

        corridorManager.CompleteFragment(
            fragmentIndex
        );
    }


    // =====================================================
    // DIM COMPLETED FRAGMENT LIGHT
    // =====================================================

    private IEnumerator FadeLightToCompletedState()
    {
        if (fragmentLight == null)
            yield break;


        float startIntensity =
            fragmentLight.intensity;


        // Instant change if duration is zero.
        if (lightFadeDuration <= 0f)
        {
            fragmentLight.intensity =
                completedLightIntensity;

            yield break;
        }


        float timer = 0f;


        while (timer < lightFadeDuration)
        {
            timer += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    timer / lightFadeDuration
                );


            // Smooth fade instead of linear fade.
            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            fragmentLight.intensity =
                Mathf.Lerp(
                    startIntensity,
                    completedLightIntensity,
                    smoothT
                );


            yield return null;
        }


        fragmentLight.intensity =
            completedLightIntensity;
    }


    // =====================================================
    // OPTIONAL FUTURE EXTERNAL CONTENT SUPPORT
    //
    // If later a slideshow / Timeline / animation controls
    // the Fragment duration instead of the AudioClip,
    // that system can call this function when finished.
    // =====================================================

    public void FinishFragmentFromExternalContent()
    {
        if (!hasTriggered || hasCompleted)
            return;


        StopAllCoroutines();


        if (
            memoryAudioSource != null &&
            memoryAudioSource.isPlaying
        )
        {
            memoryAudioSource.Stop();
        }


        StartCoroutine(
            FinishExternalContentSequence()
        );
    }


    private IEnumerator FinishExternalContentSequence()
    {
        // Keep the entire Fragment visible.
        // Only dim its local light.

        if (fragmentLight != null)
        {
            yield return StartCoroutine(
                FadeLightToCompletedState()
            );
        }


        hasCompleted = true;


        onFragmentCompleted?.Invoke();


        Debug.Log(
            "MEMORY FRAGMENT " +
            (fragmentIndex + 1) +
            " COMPLETE"
        );


        if (corridorManager != null)
        {
            corridorManager.CompleteFragment(
                fragmentIndex
            );
        }
    }
}