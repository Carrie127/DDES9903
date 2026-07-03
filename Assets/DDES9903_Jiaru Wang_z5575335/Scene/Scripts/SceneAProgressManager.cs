using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class SceneAProgressManager : MonoBehaviour
{
   [Header("Progress")]
   public int totalItems = 5;
   private int inspectedCount = 0;
   private bool twoCluePlayed = false;
   private bool fourCluePlayed = false;
   private bool endingStarted = false;
   [Header("Mid Story Audio")]
   public AudioClip twoCluesAudio;
   public AudioClip fourCluesAudio;
   [Header("Ending Audio")]
   public AudioClip finalRealisationAudio;
   public AudioClip miaEvieAudio;
   public AudioClip evieQuestionAudio;
   public AudioClip miaComeWithMeAudio;
   public AudioClip evieHomeAudio;
   [Header("SFX")]
   public AudioClip doorOpenSFX;
   [Header("Audio Sources")]
   public AudioSource voiceAudioSource;
   public AudioSource miaAudioSource;
   [Header("Door Ending")]
   public Transform doorTransform;
   public Vector3 doorOpenRotation = new Vector3(0, 90, 0);
   public float doorOpenDuration = 3f;
   public float doorOpenDelayAfterComeWithMe = 0.5f;
   [Header("Ending Timing")]
   public float delayAfterFinalRealisation = 1.2f;
   [Header("Mia Light")]
   public GameObject miaWarmLight;
   public Light miaPointLight;
   [Header("Fade / TBC")]
   public Image blackScreen;
   public TMP_Text tbcText;
   public float fadeDuration = 4f;
   public float tbcDelay = 1f;
   private bool doorOpened = false;
   private void Start()
   {
       if (blackScreen != null)
       {
           Color c = blackScreen.color;
           c.a = 0f;
           blackScreen.color = c;
           blackScreen.gameObject.SetActive(true);
       }
       if (tbcText != null)
       {
           Color c = tbcText.color;
           c.a = 0f;
           tbcText.color = c;
           tbcText.gameObject.SetActive(true);
       }
       if (miaWarmLight != null)
           miaWarmLight.SetActive(false);
       if (miaPointLight != null)
           miaPointLight.enabled = false;
   }
   public void RegisterItemClicked()
   {
       inspectedCount++;
       if (inspectedCount == 2 && !twoCluePlayed)
       {
           twoCluePlayed = true;
           StartCoroutine(PlayOneTimeAudio(twoCluesAudio));
       }
       if (inspectedCount == 4 && !fourCluePlayed)
       {
           fourCluePlayed = true;
           StartCoroutine(PlayOneTimeAudio(fourCluesAudio));
       }
       if (inspectedCount >= totalItems && !endingStarted)
       {
           endingStarted = true;
           StartCoroutine(EndingSequence());
       }
   }
   private IEnumerator PlayOneTimeAudio(AudioClip clip)
   {
       if (clip == null || voiceAudioSource == null)
           yield break;
       while (voiceAudioSource.isPlaying)
           yield return null;
       voiceAudioSource.Stop();
       voiceAudioSource.clip = clip;
       voiceAudioSource.Play();
       yield return new WaitForSeconds(clip.length);
   }
   private IEnumerator EndingSequence()
   {
       while (voiceAudioSource != null && voiceAudioSource.isPlaying)
           yield return null;
       yield return PlayVoiceFromSource(voiceAudioSource, finalRealisationAudio);
       yield return new WaitForSeconds(delayAfterFinalRealisation);
       if (miaWarmLight != null)
           miaWarmLight.SetActive(true);
       if (miaPointLight != null)
           miaPointLight.enabled = true;
       yield return PlayVoiceFromSource(miaAudioSource, miaEvieAudio);
       yield return new WaitForSeconds(0.4f);
       yield return PlayVoiceFromSource(voiceAudioSource, evieQuestionAudio);
       yield return new WaitForSeconds(0.4f);
       yield return PlayVoiceFromSource(miaAudioSource, miaComeWithMeAudio);
       yield return new WaitForSeconds(doorOpenDelayAfterComeWithMe);
       if (!doorOpened)
       {
           doorOpened = true;
           if (doorOpenSFX != null && doorTransform != null)
               AudioSource.PlayClipAtPoint(doorOpenSFX, doorTransform.position);
           yield return StartCoroutine(OpenDoor());
       }
       yield return new WaitForSeconds(0.4f);
       yield return PlayVoiceFromSource(voiceAudioSource, evieHomeAudio);
       yield return new WaitForSeconds(1f);
       yield return StartCoroutine(FadeToBlack());
       yield return new WaitForSeconds(tbcDelay);
       yield return StartCoroutine(FadeInTBC());
   }
   private IEnumerator PlayVoiceFromSource(AudioSource source, AudioClip clip)
   {
       if (clip == null || source == null)
           yield break;
       source.Stop();
       source.clip = clip;
       source.Play();
       yield return new WaitForSeconds(clip.length);
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
           float t = timer / doorOpenDuration;
           doorTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
           yield return null;
       }
       doorTransform.localRotation = endRot;
   }
   private IEnumerator FadeToBlack()
   {
       if (blackScreen == null)
           yield break;
       float timer = 0f;
       Color c = blackScreen.color;
       while (timer < fadeDuration)
       {
           timer += Time.deltaTime;
           c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
           blackScreen.color = c;
           yield return null;
       }
       c.a = 1f;
       blackScreen.color = c;
   }
   private IEnumerator FadeInTBC()
   {
       if (tbcText == null)
           yield break;
       float timer = 0f;
       Color c = tbcText.color;
       while (timer < 2f)
       {
           timer += Time.deltaTime;
           c.a = Mathf.Lerp(0f, 1f, timer / 2f);
           tbcText.color = c;
           yield return null;
       }
       c.a = 1f;
       tbcText.color = c;
   }
}