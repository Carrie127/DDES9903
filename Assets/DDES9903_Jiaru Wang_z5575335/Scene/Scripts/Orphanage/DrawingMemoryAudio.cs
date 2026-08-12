using System.Collections;
using UnityEngine;

public class DrawingMemoryAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource memoryVoiceSource;
    public AudioSource presentEvieSource;

    [Header("Drawing Memory Clips")]
    public AudioClip miaIntroMemory;
    public AudioClip youngEvieMemory;
    public AudioClip miaReplyMemory;
    public AudioClip evieReaction;

    [Header("Drawing Guidance")]
    public MemoryObjectGuide drawingGuide;

    [Header("Timing")]
    public float gapBetweenClips = 0.35f;

    private bool stageStarted = false;
    private bool drawingPickedUp = false;
    private bool pickupSequenceStarted = false;

    public void StartDrawingStage()
    {
        if (stageStarted)
            return;

        stageStarted = true;

        Debug.Log("DRAWING STAGE STARTED");

        // Drawing Glow 开始渐亮
        if (drawingGuide != null)
        {
            drawingGuide.FadeInObjectGlow();
        }

        // 同时播放第一段 Mia memory
        StartCoroutine(PlayDrawingIntro());
    }

    private IEnumerator PlayDrawingIntro()
    {
        yield return PlayMemoryClip(miaIntroMemory);

        Debug.Log("DRAWING INTRO MEMORY COMPLETE");

        // 如果玩家在第一段音频还没结束前就已经拿起 Drawing，
        // 等第一段结束后再自动继续后面的音频。
        if (drawingPickedUp && !pickupSequenceStarted)
        {
            StartPickupSequence();
        }
    }

    public void OnDrawingPickedUp()
    {
        if (drawingPickedUp)
            return;

        drawingPickedUp = true;

        Debug.Log("DRAWING PICKED UP");

        // 如果第一段 memory 还在播，不打断
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

        StartCoroutine(DrawingPickupSequence());
    }

    private IEnumerator DrawingPickupSequence()
    {
        // Young Evie
        yield return PlayMemoryClip(youngEvieMemory);

        // Mia reply
        yield return PlayMemoryClip(miaReplyMemory);

        // Present Evie
        yield return PlayPresentClip(evieReaction);

        Debug.Log("DRAWING MEMORY COMPLETE");
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