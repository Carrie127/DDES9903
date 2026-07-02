using UnityEngine;
using System;
using System.Collections;

public class NarrativeItem : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip evidenceAudio;

    [Header("Evidence Note")]
    public GameObject evidenceNote;
    public float noteDelay = 0.3f;

    [Header("Timing")]
    public float defaultAudioDuration = 6f;
    public float delayAfterFinished = 0.5f;

    [Header("Inspect Settings")]
    public bool moveToInspectPoint = false;
    public float inspectScale = 1.5f;

    [Header("Scene Progress")]
    public SceneAProgressManager progressManager;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private bool isInspecting = false;
    private bool hasRegistered = false;
    private Coroutine playCoroutine;

    public bool IsPlaying { get; private set; } = false;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        if (evidenceNote != null)
            evidenceNote.SetActive(false);
    }

    public void Inspect(Transform inspectPoint, AudioSource audioSource, Action onFinished)
    {
        if (IsPlaying) return;

        IsPlaying = true;

        if (moveToInspectPoint && inspectPoint != null)
        {
            transform.position = inspectPoint.position;
            transform.rotation = inspectPoint.rotation;
            transform.localScale = originalScale * inspectScale;
            isInspecting = true;
        }

        if (playCoroutine != null)
            StopCoroutine(playCoroutine);

        playCoroutine = StartCoroutine(PlayEvidence(audioSource, onFinished));
    }

    private IEnumerator PlayEvidence(AudioSource audioSource, Action onFinished)
    {
        yield return new WaitForSeconds(noteDelay);

        if (evidenceNote != null)
            evidenceNote.SetActive(true);

        float waitTime = defaultAudioDuration;

        if (audioSource != null && evidenceAudio != null)
        {
            audioSource.Stop();
            audioSource.clip = evidenceAudio;
            audioSource.Play();
            waitTime = evidenceAudio.length;
        }

        yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(delayAfterFinished);

        if (evidenceNote != null)
        {
            EvidenceNoteEffect effect = evidenceNote.GetComponent<EvidenceNoteEffect>();

            if (effect != null)
            {
                effect.Hide();
                yield return new WaitForSeconds(effect.fadeDuration);
            }

            evidenceNote.SetActive(false);
        }

        ReturnToOriginal();

        IsPlaying = false;
        playCoroutine = null;

        if (!hasRegistered && progressManager != null)
        {
            hasRegistered = true;
            progressManager.RegisterItemClicked();
        }

        onFinished?.Invoke();
    }

    private void ReturnToOriginal()
    {
        if (isInspecting)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            isInspecting = false;
        }
    }
}