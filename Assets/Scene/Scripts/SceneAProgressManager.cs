using UnityEngine;
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
    public GameObject blackScreen;

    [Header("Audio")]
    public AudioSource miaVoiceAudio;   // Mia says "...Evie..."
    public AudioSource rumbleAudio;     // room shaking / low rumble sound

    [Header("Lighting")]
    public Light sceneLight;

    [Header("Timing")]
    public float normalLineDuration = 2.5f;
    public float importantLineDuration = 3.2f;

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

            yield return new WaitForSeconds(importantLineDuration);
        }

        // Mia's distant voice: "...Evie..."
        if (miaVoiceAudio != null)
            miaVoiceAudio.Play();

        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "Who's there...?";

        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "Did someone just...";

        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "call my name...?";

        yield return new WaitForSeconds(2.5f);

        if (rumbleAudio != null)
            rumbleAudio.Play();

        StartCoroutine(FlickerLight());

        yield return new WaitForSeconds(4f);

        if (blackScreen != null)
            blackScreen.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (narrationText != null)
            narrationText.text = "No...";

        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "Not again.";
    }

    private IEnumerator FlickerLight()
    {
        if (sceneLight == null)
            yield break;

        for (int i = 0; i < 12; i++)
        {
            sceneLight.enabled = !sceneLight.enabled;
            yield return new WaitForSeconds(0.15f);
        }

        sceneLight.enabled = true;
    }
}