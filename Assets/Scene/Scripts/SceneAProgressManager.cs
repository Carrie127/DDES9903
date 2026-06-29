using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneAProgressManager : MonoBehaviour
{
    [Header("Progress")]
    public int totalItems = 5;
    private int clickedItems = 0;
    private bool endingStarted = false;

    [Header("UI")]
    public GameObject narrationPanel;
    public TMP_Text narrationText;
    public Image blackScreen;
    public GameObject tbcText;

    [Header("Audio")]
    public AudioSource miaVoiceAudio;
    public AudioSource rumbleAudio;

    [Header("Light")]
    public Light sceneLight;

    [Header("Timing")]
    public float normalLineDuration = 2.5f;
    public float endingLineDuration = 2.8f;
    public float fadeDuration = 5f;

    public void RegisterItemClicked()
    {
        clickedItems++;

        if (clickedItems == 2)
        {
            StartCoroutine(PlayProgressDialogue(new string[]
            {
                "Wait...",
                "These things...",
                "They feel connected somehow.",
                "But I don't understand why..."
            }));
        }

        if (clickedItems == 4)
        {
            StartCoroutine(PlayProgressDialogue(new string[]
            {
                "No...",
                "Something is wrong.",
                "This doesn't feel like someone else's story..."
            }));
        }

        if (clickedItems >= totalItems && !endingStarted)
        {
            endingStarted = true;
            StartCoroutine(EndingSequence());
        }
    }

    private IEnumerator PlayProgressDialogue(string[] lines)
    {
        yield return new WaitForSeconds(1f);

        if (narrationPanel != null)
            narrationPanel.SetActive(true);

        foreach (string line in lines)
        {
            if (narrationText != null)
                narrationText.text = line;

            yield return new WaitForSeconds(normalLineDuration);
        }

        if (narrationPanel != null)
            narrationPanel.SetActive(false);
    }

    private IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(2f);

        if (narrationPanel != null)
            narrationPanel.SetActive(true);

        string[] endingLines =
        {
            "I think...",
            "I know what happened here.",
            "The girl...",
            "...never made it out.",
            "So...",
            "this is the truth...",
            "Wait...",
            "What's happening...?"
        };

        foreach (string line in endingLines)
        {
            if (narrationText != null)
                narrationText.text = line;

            yield return new WaitForSeconds(endingLineDuration);
        }

        if (rumbleAudio != null)
            rumbleAudio.Play();

        StartCoroutine(FlickerLight());
        StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(1.2f);

        if (miaVoiceAudio != null)
            miaVoiceAudio.Play();

        if (narrationText != null)
            narrationText.text = "Who's there...?";
        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "Did someone just...";
        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "call my name...?";
        yield return new WaitForSeconds(2.5f);

        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        if (tbcText != null)
            tbcText.SetActive(true);
    }

    private IEnumerator FadeToBlack()
    {
        if (blackScreen == null)
            yield break;

        blackScreen.gameObject.SetActive(true);

        Color c = blackScreen.color;
        c.a = 0f;
        blackScreen.color = c;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            blackScreen.color = c;
            yield return null;
        }

        c.a = 1f;
        blackScreen.color = c;
    }

    private IEnumerator FlickerLight()
    {
        if (sceneLight == null)
            yield break;

        for (int i = 0; i < 18; i++)
        {
            sceneLight.enabled = !sceneLight.enabled;
            yield return new WaitForSeconds(0.12f);
        }

        sceneLight.enabled = true;
    }
}