using System.Collections;
using UnityEngine;

public class TeddyMemoryAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource memoryVoiceSource;
    public AudioSource presentEvieSource;

    [Header("Teddy Memory Clips")]
    public AudioClip youngEvieMemory;
    public AudioClip miaMemory;
    public AudioClip evieReaction;

    [Header("Warm Memory Ambience")]
    public OrphanageMemoryAmbience memoryAmbience;

    [Header("Timing")]
    public float gapBetweenClips = 0.35f;

    private bool stageStarted = false;
    private bool teddyPickedUp = false;
    private bool pickupSequenceStarted = false;

    public void StartTeddyStage()
    {
        if (stageStarted)
            return;

        stageStarted = true;

        Debug.Log("TEDDY STAGE STARTED");

        if (memoryAmbience != null)
        {
            memoryAmbience.FadeIn();
        }

        StartCoroutine(PlayTeddyIntro());
    }

    private IEnumerator PlayTeddyIntro()
    {
        // Young Evie = memory voice
        yield return PlayMemoryClip(youngEvieMemory);

        Debug.Log("TEDDY INTRO MEMORY COMPLETE");

        if (teddyPickedUp && !pickupSequenceStarted)
        {
            StartPickupSequence();
        }
    }

    public void OnTeddyPickedUp()
    {
        if (teddyPickedUp)
            return;

        teddyPickedUp = true;

        Debug.Log("TEDDY PICKED UP");

        // If memory voice is still speaking,
        // wait until the intro finishes.
        if (memoryVoiceSource != null &&
            memoryVoiceSource.isPlaying)
        {
            return;
        }

        StartPickupSequence();
    }

    private void StartPickupSequence()
    {
        if (pickupSequenceStarted)
            return;

        pickupSequenceStarted = true;

        StartCoroutine(TeddyPickupSequence());
    }

    private IEnumerator TeddyPickupSequence()
    {
        // Mia = memory voice
        yield return PlayMemoryClip(miaMemory);

        // Present Evie = clean present voice
        yield return PlayPresentClip(evieReaction);

        Debug.Log("TEDDY MEMORY COMPLETE");
    }

    private IEnumerator PlayMemoryClip(AudioClip clip)
    {
        if (memoryVoiceSource == null || clip == null)
            yield break;

        memoryVoiceSource.clip = clip;
        memoryVoiceSource.Play();

        yield return new WaitWhile(
            () => memoryVoiceSource.isPlaying
        );

        if (gapBetweenClips > 0f)
        {
            yield return new WaitForSeconds(
                gapBetweenClips
            );
        }
    }

    private IEnumerator PlayPresentClip(AudioClip clip)
    {
        if (presentEvieSource == null || clip == null)
            yield break;

        presentEvieSource.clip = clip;
        presentEvieSource.Play();

        yield return new WaitWhile(
            () => presentEvieSource.isPlaying
        );

        if (gapBetweenClips > 0f)
        {
            yield return new WaitForSeconds(
                gapBetweenClips
            );
        }
    }
}