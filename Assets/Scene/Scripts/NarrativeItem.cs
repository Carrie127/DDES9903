using UnityEngine;
using TMPro;
using System.Collections;

public class NarrativeItem : MonoBehaviour
{
    [Header("Narrative Lines")]
    [TextArea(2, 4)]
    public string[] narrationLines;

    public AudioClip[] narrationAudios;

    [Header("Timing")]
    public float defaultLineDuration = 2.5f;

    [Header("Inspect Settings")]
    public bool moveToInspectPoint = false;
    public float inspectScale = 1.5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private bool isInspecting = false;
    private Coroutine narrationCoroutine;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
    }

    public void Inspect(Transform inspectPoint, GameObject panel, TMP_Text textBox, AudioSource audioSource)
    {
        panel.SetActive(true);

        if (moveToInspectPoint && inspectPoint != null)
        {
            transform.position = inspectPoint.position;
            transform.rotation = inspectPoint.rotation;
            transform.localScale = originalScale * inspectScale;
            isInspecting = true;
        }

        if (narrationCoroutine != null)
        {
            StopCoroutine(narrationCoroutine);
        }

        narrationCoroutine = StartCoroutine(PlayNarrationLines(textBox, audioSource));
    }

    private IEnumerator PlayNarrationLines(TMP_Text textBox, AudioSource audioSource)
    {
        for (int i = 0; i < narrationLines.Length; i++)
        {
            textBox.text = narrationLines[i];

            if (audioSource != null)
            {
                audioSource.Stop();

                if (narrationAudios != null && i < narrationAudios.Length && narrationAudios[i] != null)
                {
                    audioSource.clip = narrationAudios[i];
                    audioSource.Play();

                    yield return new WaitForSeconds(narrationAudios[i].length + 0.3f);
                }
                else
                {
                    yield return new WaitForSeconds(defaultLineDuration);
                }
            }
            else
            {
                yield return new WaitForSeconds(defaultLineDuration);
            }
        }
    }

    public void CloseInspect()
    {
        if (narrationCoroutine != null)
        {
            StopCoroutine(narrationCoroutine);
            narrationCoroutine = null;
        }

        if (isInspecting)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            isInspecting = false;
        }
    }
}