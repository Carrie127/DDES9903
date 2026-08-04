using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveIntroManager : MonoBehaviour
{
    [Header("Archive Intro Audio")]
    [Tooltip("播放进入Archive后短旁白的Audio Source。")]
    public AudioSource introVoiceAudio;

    [Header("Evidence Interaction")]
    [Tooltip("Scene A中负责点击线索的NarrativeClickManager。")]
    public NarrativeClickManager clickManager;

    [Header("Fade Images")]
    [Tooltip("桌面/Web版本使用的全屏黑色Image。没有时可以留空。")]
    public Image desktopBlackScreen;

    [Tooltip("VR相机前方World Space Canvas里的黑色Image。没有时可以留空。")]
    public Image vrBlackScreen;

    [Header("Intro Timing")]
    [Tooltip("进入Archive后，从黑色渐亮到正常画面的时间。")]
    [Min(0f)]
    public float fadeInDuration = 0.8f;

    [Tooltip("旁白结束后等待多久开放线索互动。")]
    [Min(0f)]
    public float interactionDelayAfterAudio = 0.15f;

    private Coroutine introCoroutine;

    private void Awake()
    {
        /*
         * Awake比Start更早执行。
         * 先关闭线索点击，避免场景刚加载的一瞬间触发Evidence。
         */
        if (clickManager != null)
        {
            clickManager.enabled = false;
        }

        PrepareFadeImage(desktopBlackScreen);
        PrepareFadeImage(vrBlackScreen);

        SetFadeAlpha(1f);
    }

    private void Start()
    {
        introCoroutine = StartCoroutine(PlayArchiveIntro());
    }

    private IEnumerator PlayArchiveIntro()
    {
        /*
         * 进入Archive时，旁白与渐亮同时开始。
         * 玩家移动和视角始终不受限制。
         */
        if (introVoiceAudio != null)
        {
            introVoiceAudio.Stop();
            introVoiceAudio.Play();
        }

        yield return StartCoroutine(
            Fade(
                startAlpha: 1f,
                endAlpha: 0f,
                duration: fadeInDuration
            )
        );

        /*
         * 等待短旁白播放结束。
         * Unity 6.3使用Audio Generator时，
         * 直接通过isPlaying判断，不检查旧版clip字段。
         */
        if (introVoiceAudio != null)
        {
            while (introVoiceAudio.isPlaying)
            {
                yield return null;
            }
        }

        if (interactionDelayAfterAudio > 0f)
        {
            yield return new WaitForSeconds(
                interactionDelayAfterAudio
            );
        }

        if (clickManager != null)
        {
            clickManager.enabled = true;
        }

        DisableTransparentFadeImages();

        introCoroutine = null;
    }

    private void PrepareFadeImage(Image fadeImage)
    {
        if (fadeImage == null)
            return;

        fadeImage.gameObject.SetActive(true);

        /*
         * 防止黑色Image拦截桌面鼠标或VR射线。
         */
        fadeImage.raycastTarget = false;
    }

    private IEnumerator Fade(
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        EnableFadeImages();
        SetFadeAlpha(startAlpha);

        if (duration <= 0f)
        {
            SetFadeAlpha(endAlpha);
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / duration
            );

            float currentAlpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                progress
            );

            SetFadeAlpha(currentAlpha);

            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    private void SetFadeAlpha(float alpha)
    {
        SetImageAlpha(desktopBlackScreen, alpha);
        SetImageAlpha(vrBlackScreen, alpha);
    }

    private void SetImageAlpha(
        Image fadeImage,
        float alpha
    )
    {
        if (fadeImage == null)
            return;

        Color colour = fadeImage.color;
        colour.a = Mathf.Clamp01(alpha);
        fadeImage.color = colour;
    }

    private void EnableFadeImages()
    {
        if (desktopBlackScreen != null)
        {
            desktopBlackScreen.gameObject.SetActive(true);
            desktopBlackScreen.raycastTarget = false;
        }

        if (vrBlackScreen != null)
        {
            vrBlackScreen.gameObject.SetActive(true);
            vrBlackScreen.raycastTarget = false;
        }
    }

    private void DisableTransparentFadeImages()
    {
        if (desktopBlackScreen != null)
        {
            desktopBlackScreen.gameObject.SetActive(false);
        }

        if (vrBlackScreen != null)
        {
            vrBlackScreen.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        /*
         * 防止物体在Intro中途被关闭时，
         * NarrativeClickManager永久保持Disabled。
         */
        if (introCoroutine != null)
        {
            StopCoroutine(introCoroutine);
            introCoroutine = null;
        }

        if (clickManager != null)
        {
            clickManager.enabled = true;
        }
    }
}