using System.Collections;
using UnityEngine;

public class PhotoMemoryAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource memoryVoiceSource;
    public AudioSource presentEvieSource;

    [Header("Photo Memory Clips")]
    public AudioClip miaIntroMemory;
    public AudioClip youngEvieMemory;
    public AudioClip evieReaction;

    [Header("Photo Glow")]
    public Light photoGlow;

    [Header("Photo Reveal")]
    public PhotoReveal photoReveal;

    [Header("Bedroom Door Sequence")]
    public BedroomDoorSequence bedroomDoorSequence;

    [Header("Timing")]
    public float glowFadeDuration = 1.0f;
    public float gapBetweenClips = 0.35f;

    [Tooltip("How long to wait for the photo reveal animation before Young Evie's memory begins.")]
    public float revealWaitDuration = 1.5f;

    private bool stageStarted = false;
    private bool photoClicked = false;

    private float photoGlowOriginalIntensity = 1f;

    private Coroutine glowCoroutine;

    private void Start()
    {
        // Remember Photo Glow's intended visible intensity,
        // then hide it until Drawing is completed.
        if (photoGlow != null)
        {
            photoGlowOriginalIntensity = photoGlow.intensity;
            photoGlow.intensity = 0f;
        }
    }

    // =====================================================
    // Called after Drawing has been placed correctly
    // =====================================================

    public void StartPhotoStage()
    {
        if (stageStarted)
            return;

        stageStarted = true;

        Debug.Log("PHOTO STAGE STARTED");

        // Photo guidance begins.
        FadePhotoGlowIn();

        // Mia's first memory starts at the same time.
        StartCoroutine(PlayPhotoIntro());
    }

    private IEnumerator PlayPhotoIntro()
    {
        yield return PlayMemoryClip(miaIntroMemory);

        Debug.Log("PHOTO INTRO MEMORY COMPLETE");
    }

    // =====================================================
    // Called when player clicks the photo
    // =====================================================

    public void OnPhotoClicked()
    {
        // Prevent the player from triggering Photo
        // before Drawing has actually completed.
        if (!stageStarted)
        {
            Debug.LogWarning(
                "Photo clicked before Photo Stage started."
            );

            return;
        }

        if (photoClicked)
            return;

        photoClicked = true;

        Debug.Log("PHOTO CLICKED");

        StartCoroutine(PhotoClickSequence());
    }

    private IEnumerator PhotoClickSequence()
    {
        // -------------------------------------------------
        // 1. Photo guidance light fades away
        // -------------------------------------------------

        FadePhotoGlowOut();

        // -------------------------------------------------
        // 2. Slowly reveal / straighten the photo frame
        // -------------------------------------------------

        if (photoReveal != null)
        {
            photoReveal.RevealPhoto();
        }
        else
        {
            Debug.LogWarning(
                "PhotoMemoryAudio: PhotoReveal reference is missing!"
            );
        }

        // Allow the reveal animation to finish
        // before the next memory dialogue begins.
        if (revealWaitDuration > 0f)
        {
            yield return new WaitForSeconds(
                revealWaitDuration
            );
        }

        // -------------------------------------------------
        // 3. Young Evie's memory
        // -------------------------------------------------

        yield return PlayMemoryClip(
            youngEvieMemory
        );

        // -------------------------------------------------
        // 4. Present Evie's reaction
        // -------------------------------------------------

        yield return PlayPresentClip(
            evieReaction
        );

        Debug.Log("PHOTO MEMORY COMPLETE");

        // -------------------------------------------------
        // 5. Begin the existing door-closing sequence
        // -------------------------------------------------

        if (bedroomDoorSequence != null)
        {
            bedroomDoorSequence.StartDoorClose();
        }
        else
        {
            Debug.LogWarning(
                "PhotoMemoryAudio: BedroomDoorSequence reference is missing!"
            );
        }
    }

    // =====================================================
    // Photo Glow
    // =====================================================

    private void FadePhotoGlowIn()
    {
        if (photoGlow == null)
            return;

        StartGlowFade(
            photoGlowOriginalIntensity
        );
    }

    private void FadePhotoGlowOut()
    {
        if (photoGlow == null)
            return;

        StartGlowFade(0f);
    }

    private void StartGlowFade(float targetIntensity)
    {
        if (photoGlow == null)
            return;

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }

        glowCoroutine = StartCoroutine(
            FadeGlow(
                photoGlow.intensity,
                targetIntensity,
                glowFadeDuration
            )
        );
    }

    private IEnumerator FadeGlow(
        float from,
        float to,
        float duration
    )
    {
        if (photoGlow == null)
            yield break;

        if (duration <= 0f)
        {
            photoGlow.intensity = to;
            glowCoroutine = null;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            photoGlow.intensity =
                Mathf.Lerp(
                    from,
                    to,
                    smoothT
                );

            yield return null;
        }

        photoGlow.intensity = to;
        glowCoroutine = null;
    }

    // =====================================================
    // Memory Voice
    // =====================================================

    private IEnumerator PlayMemoryClip(
        AudioClip clip
    )
    {
        if (memoryVoiceSource == null ||
            clip == null)
        {
            yield break;
        }

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

    // =====================================================
    // Present Evie Voice
    // =====================================================

    private IEnumerator PlayPresentClip(
        AudioClip clip
    )
    {
        if (presentEvieSource == null ||
            clip == null)
        {
            yield break;
        }

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