using System.Collections;
using UnityEngine;

public class HospitalClueManager : MonoBehaviour
{
    [Header("Final Exit Reveal")]
    public Light exitSignGlow;
    public ClueLightPulse exitSignPulse;
    public GameObject hospitalClock;

    [Header("Clock Audio")]
    public AudioSource clockRevealSource;
    public AudioSource clockAmbientSource;

    [Header("Reveal Timing")]
    public float clockRevealDelay = 3.5f;

    [Header("Debug - Clue States")]
    [SerializeField] private bool wristbandComplete = false;
    [SerializeField] private bool teddyComplete = false;
    [SerializeField] private bool triageConversationComplete = false;
    [SerializeField] private bool benchConversationComplete = false;

    private bool finalExitRevealed = false;

    private void Start()
    {
        // Final exit guidance hidden at the beginning
        if (exitSignGlow != null)
        {
            exitSignGlow.enabled = false;
        }

        if (hospitalClock != null)
        {
            hospitalClock.SetActive(false);
        }

        // Make sure clock audio is not already playing
        if (clockRevealSource != null)
        {
            clockRevealSource.Stop();
        }

        if (clockAmbientSource != null)
        {
            clockAmbientSource.Stop();
        }

        Debug.Log("HOSPITAL CLUE MANAGER READY");
    }

    // =====================================================
    // Individual clue completion
    // =====================================================

    public void CompleteWristband()
    {
        if (wristbandComplete)
            return;

        wristbandComplete = true;

        Debug.Log("HOSPITAL: WRISTBAND COMPLETE");

        CheckAllClues();
    }

    public void CompleteTeddy()
    {
        if (teddyComplete)
            return;

        teddyComplete = true;

        Debug.Log("HOSPITAL: TEDDY COMPLETE");

        CheckAllClues();
    }

    public void CompleteTriageConversation()
    {
        if (triageConversationComplete)
            return;

        triageConversationComplete = true;

        Debug.Log("HOSPITAL: TRIAGE CONVERSATION COMPLETE");

        CheckAllClues();
    }

    public void CompleteBenchConversation()
    {
        if (benchConversationComplete)
            return;

        benchConversationComplete = true;

        Debug.Log("HOSPITAL: BENCH CONVERSATION COMPLETE");

        CheckAllClues();
    }

    // =====================================================
    // Check whether all Hospital clues are complete
    // =====================================================

    private void CheckAllClues()
    {
        int completedCount = 0;

        if (wristbandComplete) completedCount++;
        if (teddyComplete) completedCount++;
        if (triageConversationComplete) completedCount++;
        if (benchConversationComplete) completedCount++;

        Debug.Log(
            "HOSPITAL CLUES: " +
            completedCount +
            " / 4"
        );

        if (
            wristbandComplete &&
            teddyComplete &&
            triageConversationComplete &&
            benchConversationComplete
        )
        {
            RevealFinalExit();
        }
    }

    // =====================================================
    // Reveal final exit
    // =====================================================

    private void RevealFinalExit()
    {
        if (finalExitRevealed)
            return;

        finalExitRevealed = true;

        Debug.Log("ALL HOSPITAL CLUES COMPLETE");

        StartCoroutine(FinalExitRevealSequence());
    }

    private IEnumerator FinalExitRevealSequence()
    {
        // -------------------------------------------------
        // 1. Emergency Department sign begins glowing
        // -------------------------------------------------

        if (exitSignGlow != null)
        {
            exitSignGlow.enabled = true;
        }

        if (exitSignPulse != null)
        {
            exitSignPulse.StartPulse();
        }

        Debug.Log("HOSPITAL EXIT SIGN ACTIVATED");

        // -------------------------------------------------
        // 2. Give player time to notice the sign
        // -------------------------------------------------

        yield return new WaitForSeconds(clockRevealDelay);

        // -------------------------------------------------
        // 3. Reveal the 7:42 clock
        // -------------------------------------------------

        if (hospitalClock != null)
        {
            hospitalClock.SetActive(true);
        }

        Debug.Log("HOSPITAL 7:42 CLOCK REVEALED");

        // -------------------------------------------------
        // 4. Short clock reveal sound
        // -------------------------------------------------

        if (
            clockRevealSource != null &&
            clockRevealSource.clip != null
        )
        {
            clockRevealSource.Play();
        }

        // -------------------------------------------------
        // 5. Start subtle 3D clock ambience
        // -------------------------------------------------

        if (
            clockAmbientSource != null &&
            clockAmbientSource.clip != null
        )
        {
            clockAmbientSource.Play();
        }
    }
}