using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalConversationClue : MonoBehaviour
{
    [Header("References")]
    public Light conversationGlow;
    public ClueLightPulse clueLightPulse;

    public AudioSource distantMurmurSource;
    public AudioSource clearConversationSource;

    [Header("Timing")]
    public float glowFadeDuration = 0.8f;
    public float murmurFadeDuration = 0.8f;

    [Header("Clue State")]
    public UnityEvent onClueComplete;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        StartCoroutine(
            ConversationSequence()
        );
    }

    private IEnumerator ConversationSequence()
    {
        Debug.Log(
            "HOSPITAL CONVERSATION CLUE TRIGGERED"
        );

        // =================================================
        // 1. Stop pulse first
        // =================================================

        if (clueLightPulse != null)
        {
            clueLightPulse.StopPulse();
        }

        // =================================================
        // 2. Fade Glow naturally from current intensity
        // =================================================

        if (conversationGlow != null)
        {
            float startIntensity =
                conversationGlow.intensity;

            float timer = 0f;

            if (glowFadeDuration <= 0f)
            {
                conversationGlow.intensity = 0f;
            }
            else
            {
                while (timer < glowFadeDuration)
                {
                    timer += Time.deltaTime;

                    float t =
                        Mathf.Clamp01(
                            timer / glowFadeDuration
                        );

                    float smoothT =
                        Mathf.SmoothStep(
                            0f,
                            1f,
                            t
                        );

                    conversationGlow.intensity =
                        Mathf.Lerp(
                            startIntensity,
                            0f,
                            smoothT
                        );

                    yield return null;
                }

                conversationGlow.intensity = 0f;
            }

            conversationGlow.enabled = false;
        }

        // =================================================
        // 3. Fade distant murmur out
        // =================================================

        if (distantMurmurSource != null)
        {
            float startVolume =
                distantMurmurSource.volume;

            float timer = 0f;

            if (murmurFadeDuration <= 0f)
            {
                distantMurmurSource.volume = 0f;
            }
            else
            {
                while (timer < murmurFadeDuration)
                {
                    timer += Time.deltaTime;

                    float t =
                        Mathf.Clamp01(
                            timer / murmurFadeDuration
                        );

                    float smoothT =
                        Mathf.SmoothStep(
                            0f,
                            1f,
                            t
                        );

                    distantMurmurSource.volume =
                        Mathf.Lerp(
                            startVolume,
                            0f,
                            smoothT
                        );

                    yield return null;
                }

                distantMurmurSource.volume = 0f;
            }

            distantMurmurSource.Stop();
        }

        // =================================================
        // 4. Clear conversation
        // =================================================

        if (clearConversationSource != null &&
            clearConversationSource.clip != null)
        {
            clearConversationSource.Play();

            yield return new WaitWhile(
                () => clearConversationSource.isPlaying
            );
        }

        // =================================================
        // 5. Clue complete
        // =================================================

        Debug.Log(
            "HOSPITAL CONVERSATION CLUE COMPLETE"
        );

        onClueComplete?.Invoke();
    }
}