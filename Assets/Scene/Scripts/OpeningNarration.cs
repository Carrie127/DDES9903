using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OpeningNarration : MonoBehaviour
{
    [Header("UI")]
    public GameObject narrationPanel;
    public TMP_Text narrationText;
    public Image blackScreen;

    [Header("Player Control")]
    public MonoBehaviour playerController;

    [Header("Timing")]
    public float fadeInDuration = 4f;

    void Start()
    {
        StartCoroutine(PlayOpening());
    }

    private IEnumerator PlayOpening()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            c.a = 1f;
            blackScreen.color = c;
        }

        narrationPanel.SetActive(true);

        narrationText.text = "Ugh...";
        yield return new WaitForSeconds(2f);

        narrationText.text = "My head...";
        yield return new WaitForSeconds(2f);

        narrationText.text = "It hurts.";
        yield return new WaitForSeconds(2.5f);

        narrationText.text = "Where... am I?";
        yield return new WaitForSeconds(2.5f);

        StartCoroutine(FadeInFromBlack());

        narrationText.text = "An archive room...?";
        yield return new WaitForSeconds(2.8f);

        narrationText.text = "Why does this place feel so... familiar?";
        yield return new WaitForSeconds(3f);

        narrationPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;
    }

    private IEnumerator FadeInFromBlack()
    {
        Color c = blackScreen.color;
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeInDuration);
            blackScreen.color = c;
            yield return null;
        }

        c.a = 0f;
        blackScreen.color = c;
    }
}