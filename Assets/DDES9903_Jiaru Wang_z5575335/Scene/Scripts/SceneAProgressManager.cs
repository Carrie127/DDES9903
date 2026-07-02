using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneAProgressManager : MonoBehaviour
{
    [Header("Progress")]
    public int totalItems = 5;
    private int clickedItems = 0;
    private bool endingStarted = false;

    [Header("UI")]
    public Image blackScreen;
    public GameObject tbcText;

    [Header("Ending Audio")]
    public AudioSource finalRealisationAudio; // Evie: I think...
    public AudioSource miaEvieAudio;          // Mia: Evie...
    public AudioSource evieReplyAudio;        // Evie: Who's there + How do you know...
    public AudioSource miaComeWithMeAudio;    // Mia: Come with me
    public AudioSource doorOpenAudio;         // Door sound
    public AudioSource endingEvieAudio;       // Evie: Why... like home?

    [Header("Door Ending")]
    public Transform doorTransform;
    public Vector3 doorOpenRotation = new Vector3(0f, 90f, 0f);
    public float doorOpenDuration = 3f;

    [Header("Mia Light")]
    public GameObject miaLight;

    [Header("Timing")]
    public float fadeDuration = 5f;
    public float pauseAfterFinalRealisation = 0.8f;
    public float pauseAfterMiaEvie = 0.8f;
    public float pauseBeforeDoorOpen = 0.5f;
    public float pauseAfterDoorOpen = 1.2f;
    public float pauseBeforeFade = 1.2f;

    public void RegisterItemClicked()
    {
        clickedItems++;

        if (clickedItems >= totalItems && !endingStarted)
        {
            endingStarted = true;
            StartCoroutine(EndingSequence());
        }
    }

    private IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(1.5f);

        yield return PlayAudioAndWait(finalRealisationAudio);
        yield return new WaitForSeconds(pauseAfterFinalRealisation);

        yield return PlayAudioAndWait(miaEvieAudio);
        yield return new WaitForSeconds(pauseAfterMiaEvie);

        yield return PlayAudioAndWait(evieReplyAudio);

        yield return PlayAudioAndWait(miaComeWithMeAudio);
        yield return new WaitForSeconds(pauseBeforeDoorOpen);

        if (doorOpenAudio != null)
            doorOpenAudio.Play();

        StartCoroutine(OpenDoor());

        yield return new WaitForSeconds(0.8f);

        if (miaLight != null)
            miaLight.SetActive(true);

        yield return new WaitForSeconds(pauseAfterDoorOpen);

        yield return PlayAudioAndWait(endingEvieAudio);

        yield return new WaitForSeconds(pauseBeforeFade);

        yield return StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(0.5f);

        if (tbcText != null)
            tbcText.SetActive(true);
    }

    private IEnumerator PlayAudioAndWait(AudioSource audio)
    {
        if (audio == null || audio.clip == null)
            yield break;

        audio.Play();

        while (audio.isPlaying)
            yield return null;

        yield return new WaitForSeconds(0.25f);
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