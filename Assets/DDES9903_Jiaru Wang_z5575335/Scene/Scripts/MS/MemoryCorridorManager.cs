using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MemoryCorridorManager : MonoBehaviour
{
    [Header("Memory Space Intro")]
    [Tooltip("AudioSource used to play Present Evie's Memory Space intro.")]
    [SerializeField] private AudioSource introAudioSource;

    [Tooltip(
        "How long to wait after Scene_MemorySpace loads before the intro begins. " +
        "This allows the existing white transition to mostly fade away first."
    )]
    [SerializeField] private float introStartDelay = 1.2f;

    [Tooltip(
        "Short pause after the intro finishes before Fragment 01 appears."
    )]
    [SerializeField] private float delayBeforeFirstFragment = 0.4f;


    [Header("Memory Fragments")]
    [Tooltip(
        "Drag Fragment_01 to Fragment_05 here IN ORDER."
    )]
    [SerializeField] private GameObject[] fragments;


    [Header("Fragment Progression")]
    [Tooltip(
        "Short pause after one Fragment finishes before the next one appears."
    )]
    [SerializeField] private float delayBetweenFragments = 0.35f;

    [Tooltip(
        "Short pause after Fragment 05 finishes before Truth Reconstruction begins."
    )]
    [SerializeField] private float delayAfterFinalFragment = 0.5f;


    [Header("Corridor Complete")]
    [Tooltip(
        "Invoked after all five Fragments have been completed. " +
        "Later we will connect this to the Truth Reconstruction reveal."
    )]
    public UnityEvent onAllFragmentsCompleted;


    private int currentFragmentIndex = -1;

    private bool introFinished = false;
    private bool waitingForNextFragment = false;
    private bool allFragmentsCompleted = false;


    // =====================================================
    // INITIAL SETUP
    // =====================================================

    private void Awake()
    {
        // Hide every Fragment BEFORE the first rendered frame.
        // This prevents their glowing frames from being visible
        // while the player is still entering Memory Space.
        HideAllFragments();
    }


    private void Start()
    {
        StartCoroutine(
            MemorySpaceIntroSequence()
        );
    }


    // =====================================================
    // MEMORY SPACE INTRO
    // =====================================================

    private IEnumerator MemorySpaceIntroSequence()
    {
        Debug.Log(
            "MEMORY SPACE CORRIDOR → INTRO SEQUENCE STARTED"
        );


        // -------------------------------------------------
        // 1. Allow the existing white transition
        //    to mostly fade away first.
        //
        // IMPORTANT:
        // This script does NOT control the transition itself.
        // -------------------------------------------------

        if (introStartDelay > 0f)
        {
            yield return new WaitForSeconds(
                introStartDelay
            );
        }


        // -------------------------------------------------
        // 2. Play Present Evie's intro
        // -------------------------------------------------

        if (
            introAudioSource != null &&
            introAudioSource.clip != null
        )
        {
            introAudioSource.Stop();

            introAudioSource.Play();

            yield return new WaitWhile(
                () => introAudioSource.isPlaying
            );
        }
        else
        {
            Debug.LogWarning(
                "MemoryCorridorManager: Intro AudioSource or AudioClip is missing."
            );
        }


        introFinished = true;

        Debug.Log(
            "MEMORY SPACE CORRIDOR → INTRO FINISHED"
        );


        // -------------------------------------------------
        // 3. Small pause before the first memory appears
        // -------------------------------------------------

        if (delayBeforeFirstFragment > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforeFirstFragment
            );
        }


        // -------------------------------------------------
        // 4. Reveal Fragment 01
        // -------------------------------------------------

        ActivateFragment(0);
    }


    // =====================================================
    // COMPLETE CURRENT FRAGMENT
    //
    // Fragment numbers in Unity:
    //
    // Fragment 01 → index 0
    // Fragment 02 → index 1
    // Fragment 03 → index 2
    // Fragment 04 → index 3
    // Fragment 05 → index 4
    //
    // The Fragment interaction script will call this
    // AFTER its truth content has completely finished.
    // =====================================================

    public void CompleteFragment(
        int fragmentIndex
    )
    {
        if (!introFinished)
        {
            Debug.LogWarning(
                "MemoryCorridorManager: Fragment attempted to complete before intro finished."
            );

            return;
        }


        if (allFragmentsCompleted)
            return;


        if (waitingForNextFragment)
            return;


        // Prevent out-of-order progression.
        if (fragmentIndex != currentFragmentIndex)
        {
            Debug.LogWarning(
                "MemoryCorridorManager: Wrong Fragment attempted to complete. " +
                "Expected index "
                + currentFragmentIndex
                + ", received "
                + fragmentIndex
            );

            return;
        }


        Debug.Log(
            "MEMORY FRAGMENT COMPLETED → "
            + (fragmentIndex + 1)
        );


        StartCoroutine(
            ProgressAfterFragment()
        );
    }


    // =====================================================
    // MOVE TO NEXT FRAGMENT
    // =====================================================

    private IEnumerator ProgressAfterFragment()
    {
        waitingForNextFragment = true;


        int nextFragmentIndex =
            currentFragmentIndex + 1;


        // -------------------------------------------------
        // Another Fragment still remains
        // -------------------------------------------------

        if (
            fragments != null &&
            nextFragmentIndex < fragments.Length
        )
        {
            if (delayBetweenFragments > 0f)
            {
                yield return new WaitForSeconds(
                    delayBetweenFragments
                );
            }


            ActivateFragment(
                nextFragmentIndex
            );


            waitingForNextFragment = false;

            yield break;
        }


        // -------------------------------------------------
        // Fragment 05 was the final Fragment
        // -------------------------------------------------

        allFragmentsCompleted = true;


        if (delayAfterFinalFragment > 0f)
        {
            yield return new WaitForSeconds(
                delayAfterFinalFragment
            );
        }


        Debug.Log(
            "MEMORY CORRIDOR → ALL FRAGMENTS COMPLETED"
        );


        onAllFragmentsCompleted?.Invoke();

        waitingForNextFragment = false;
    }


    // =====================================================
    // ACTIVATE ONE FRAGMENT
    // =====================================================

    private void ActivateFragment(
        int fragmentIndex
    )
    {
        if (
            fragments == null ||
            fragments.Length == 0
        )
        {
            Debug.LogWarning(
                "MemoryCorridorManager: No Fragments assigned."
            );

            return;
        }


        if (
            fragmentIndex < 0 ||
            fragmentIndex >= fragments.Length
        )
        {
            Debug.LogWarning(
                "MemoryCorridorManager: Invalid Fragment index."
            );

            return;
        }


        currentFragmentIndex =
            fragmentIndex;


        GameObject fragment =
            fragments[fragmentIndex];


        if (fragment != null)
        {
            fragment.SetActive(true);

            Debug.Log(
                "MEMORY FRAGMENT ACTIVATED → "
                + (fragmentIndex + 1)
            );
        }
        else
        {
            Debug.LogWarning(
                "MemoryCorridorManager: Fragment "
                + (fragmentIndex + 1)
                + " is missing."
            );
        }
    }


    // =====================================================
    // HIDE ALL FRAGMENTS
    // =====================================================

    private void HideAllFragments()
    {
        if (fragments == null)
            return;


        foreach (GameObject fragment in fragments)
        {
            if (fragment != null)
            {
                fragment.SetActive(false);
            }
        }
    }


    // =====================================================
    // OPTIONAL HELPERS FOR FUTURE INTERACTION SCRIPTS
    // =====================================================

    public bool IsCurrentFragment(
        int fragmentIndex
    )
    {
        return (
            introFinished &&
            !allFragmentsCompleted &&
            !waitingForNextFragment &&
            fragmentIndex == currentFragmentIndex
        );
    }


    public int GetCurrentFragmentIndex()
    {
        return currentFragmentIndex;
    }


    public bool AreAllFragmentsCompleted()
    {
        return allFragmentsCompleted;
    }
}