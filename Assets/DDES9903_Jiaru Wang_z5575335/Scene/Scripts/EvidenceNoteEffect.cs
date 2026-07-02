using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EvidenceNoteEffect : MonoBehaviour
{
    [Header("References")]
    public TMP_Text noteText;
    public MeshRenderer paperRenderer;

    [Header("Fade")]
    public float fadeDuration = 0.4f;
    public float paperMaxAlpha = 0.75f;

    [Header("Float")]
    public float floatAmplitude = 0.02f;
    public float floatSpeed = 1.5f;

    [Header("Line Reveal Timing")]
    public float afterPaperFadeDelay = 0.3f;

    // 每一行出现的时间，单位：秒
    // 从“纸张淡入完成之后”开始算
    public float[] lineRevealTimes;

    private Color originalTextColor;
    private Color originalPaperColor;
    private Vector3 startLocalPos;
    private Coroutine routine;
    private string fullText;

    void Awake()
    {
        startLocalPos = transform.localPosition;

        if (noteText != null)
        {
            originalTextColor = noteText.color;
            fullText = noteText.text;
            noteText.text = "";
        }

        if (paperRenderer != null)
            originalPaperColor = paperRenderer.material.color;

        SetAlpha(0f);
    }

    void OnEnable()
    {
        startLocalPos = transform.localPosition;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine());
    }

    void Update()
    {
        transform.localPosition =
            startLocalPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(HideRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (noteText != null)
            noteText.text = "";

        SetAlpha(0f);

        // 先让纸张淡入
        yield return StartCoroutine(FadeTo(1f));

        // 纸张完全出现后，再等一下
        yield return new WaitForSeconds(afterPaperFadeDelay);

        if (noteText == null || string.IsNullOrEmpty(fullText))
            yield break;

        List<string> lines = new List<string>();
        string[] rawLines = fullText.Split('\n');

        foreach (string rawLine in rawLines)
        {
            string cleanLine = rawLine.Trim();

            if (!string.IsNullOrEmpty(cleanLine))
                lines.Add(cleanLine);
        }

        string currentText = "";
        float previousTime = 0f;

        for (int i = 0; i < lines.Count; i++)
        {
            float targetTime = 0f;

            if (lineRevealTimes != null && i < lineRevealTimes.Length)
                targetTime = lineRevealTimes[i];
            else
                targetTime = i * 1.0f;

            float waitTime = Mathf.Max(0f, targetTime - previousTime);
            yield return new WaitForSeconds(waitTime);
            previousTime = targetTime;

            if (i == 0)
            {
                currentText = lines[i];
            }
            else if (i == 1)
            {
                currentText += "\n\n" + lines[i];
            }
            else if (i == 2)
            {
                currentText += "\n\n" + lines[i];
            }
            else
            {
                currentText += "\n" + lines[i];
            }

            noteText.text = currentText;
        }
    }

    private IEnumerator HideRoutine()
    {
        yield return StartCoroutine(FadeTo(0f));

        if (noteText != null)
            noteText.text = "";
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = noteText != null ? noteText.color.a : 0f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float a)
    {
        if (noteText != null)
        {
            Color c = originalTextColor;
            c.a = a;
            noteText.color = c;
        }

        if (paperRenderer != null)
        {
            Color c = originalPaperColor;
            c.a = a * paperMaxAlpha;
            paperRenderer.material.color = c;
        }
    }
}