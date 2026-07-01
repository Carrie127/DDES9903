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

    [Header("Door Ending")]
    public Transform doorTransform;
    public Vector3 doorOpenRotation = new Vector3(0f, 90f, 0f);
    public float doorOpenDuration = 3f;
    public GameObject miaWarmLight;
    public Light miaPointLight;

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
            "this is the truth..."
        };

        foreach (string line in endingLines)
        {
            if (narrationText != null)
                narrationText.text = line;

            yield return new WaitForSeconds(endingLineDuration);
        }

        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        StartCoroutine(OpenDoor());

        yield return new WaitForSeconds(0.8f);

        if (miaWarmLight != null)
            miaWarmLight.SetActive(true);

        if (miaPointLight != null)
            miaPointLight.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        if (miaVoiceAudio != null)
            miaVoiceAudio.Play();

        if (narrationPanel != null)
            narrationPanel.SetActive(true);

        if (narrationText != null)
            narrationText.text = "...Evie...";
        yield return new WaitForSeconds(2.8f);

        if (narrationText != null)
            narrationText.text = "Who's there...?";
        yield return new WaitForSeconds(2.5f);

        if (narrationText != null)
            narrationText.text = "How do you know my name?";
        yield return new WaitForSeconds(3f);

        if (narrationText != null)
            narrationText.text = "...Come with me.";
        yield return new WaitForSeconds(3f);

        if (narrationText != null)
            narrationText.text = "Why...";
        yield return new WaitForSeconds(2.3f);

        if (narrationText != null)
            narrationText.text = "Why does this place feel...";
        yield return new WaitForSeconds(2.8f);

        if (narrationText != null)
            narrationText.text = "...like home?";
        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(fadeDuration + 0.5f);

        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        if (tbcText != null)
            tbcText.SetActive(true);
    }

    private IEnumerator OpenDoor()
    {
        if (doorTransform == null)
            yield break;

        Quaternion startRot = doorTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(doorOpenRotation);

        float timer = 0f;

        while (timer < doorOpenDuration)
        {
            timer += Time.deltaTime;
            doorTransform.localRotation = Quaternion.Slerp(startRot, endRot, timer / doorOpenDuration);
            yield return null;
        }

        doorTransform.localRotation = endRot;
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
}