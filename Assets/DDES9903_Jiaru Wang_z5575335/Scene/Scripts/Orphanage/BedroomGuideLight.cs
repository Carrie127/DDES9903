using System.Collections;
using UnityEngine;

public class BedroomGuideLight : MonoBehaviour
{
    [Header("Guide Light")]
    public Light guideLight;

    [Header("Fade Settings")]
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;

    private float targetIntensity = 1f;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (guideLight == null)
        {
            guideLight = GetComponent<Light>();
        }

        if (guideLight != null)
        {
            // 记住 Inspector 里你已经调好的最终亮度
            targetIntensity = guideLight.intensity;

            // 游戏开始时先隐藏
            guideLight.intensity = 0f;
        }
    }

    public void FadeInGuideLight()
    {
        if (guideLight == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadeLight(
                guideLight.intensity,
                targetIntensity,
                fadeInDuration
            )
        );
    }

    public void FadeOutGuideLight()
    {
        if (guideLight == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadeLight(
                guideLight.intensity,
                0f,
                fadeOutDuration
            )
        );
    }

    private IEnumerator FadeLight(
        float startIntensity,
        float endIntensity,
        float duration
    )
    {
        float timer = 0f;

        if (duration <= 0f)
        {
            guideLight.intensity = endIntensity;
            yield break;
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            guideLight.intensity =
                Mathf.Lerp(
                    startIntensity,
                    endIntensity,
                    smoothT
                );

            yield return null;
        }

        guideLight.intensity = endIntensity;
        fadeCoroutine = null;
    }
}