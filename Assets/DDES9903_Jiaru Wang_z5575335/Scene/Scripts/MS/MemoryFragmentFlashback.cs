using System.Collections;
using UnityEngine;

public class MemoryFragmentFlashback : MonoBehaviour
{
    [Header("Flashback Images")]
    [SerializeField] private CanvasGroup imageA;
    [SerializeField] private CanvasGroup imageB;
    [SerializeField] private CanvasGroup imageC;


    [Header("Fragment 1 Timing")]
    [Tooltip("Image A fade in")]
    [SerializeField] private float imageAFadeIn = 0.55f;

    [Tooltip("How long Image A stays fully visible")]
    [SerializeField] private float imageAHold = 2.10f;

    [Tooltip("Crossfade from Image A to Image B")]
    [SerializeField] private float crossfadeAToB = 0.60f;

    [Tooltip("How long Image B stays fully visible")]
    [SerializeField] private float imageBHold = 2.90f;

    [Tooltip("Crossfade from Image B to Image C")]
    [SerializeField] private float crossfadeBToC = 0.70f;

    [Tooltip("How long Image C stays fully visible")]
    [SerializeField] private float imageCHold = 3.25f;

    [Tooltip("Final fade out of Image C")]
    [SerializeField] private float imageCFadeOut = 0.70f;


    private Coroutine flashbackCoroutine;
    private bool isPlaying = false;


    private void Awake()
    {
        SetAlpha(imageA, 0f);
        SetAlpha(imageB, 0f);
        SetAlpha(imageC, 0f);
    }


    // =====================================================
    // CALLED BY MEMORY FRAGMENT INTERACTION
    // =====================================================

    public void PlayFlashback()
    {
        if (isPlaying)
            return;

        if (flashbackCoroutine != null)
        {
            StopCoroutine(flashbackCoroutine);
        }

        flashbackCoroutine = StartCoroutine(
            FlashbackSequence()
        );
    }


    // =====================================================
    // MAIN FLASHBACK SEQUENCE
    // =====================================================

    private IEnumerator FlashbackSequence()
    {
        isPlaying = true;

        // Always begin completely hidden.
        SetAlpha(imageA, 0f);
        SetAlpha(imageB, 0f);
        SetAlpha(imageC, 0f);


        // -------------------------------------------------
        // IMAGE A
        // 0.00 - 2.65 sec
        // -------------------------------------------------

        yield return FadeCanvasGroup(
            imageA,
            0f,
            1f,
            imageAFadeIn
        );

        yield return new WaitForSeconds(
            imageAHold
        );


        // -------------------------------------------------
        // CROSSFADE A → B
        // Approx. 2.65 - 3.25 sec
        // -------------------------------------------------

        yield return Crossfade(
            imageA,
            imageB,
            crossfadeAToB
        );


        // -------------------------------------------------
        // IMAGE B
        // Approx. 3.25 - 6.15 sec
        // -------------------------------------------------

        yield return new WaitForSeconds(
            imageBHold
        );


        // -------------------------------------------------
        // CROSSFADE B → C
        // Approx. 6.15 - 6.85 sec
        // -------------------------------------------------

        yield return Crossfade(
            imageB,
            imageC,
            crossfadeBToC
        );


        // -------------------------------------------------
        // IMAGE C
        // Approx. 6.85 - 10.10 sec
        // -------------------------------------------------

        yield return new WaitForSeconds(
            imageCHold
        );


        // -------------------------------------------------
        // FINAL FADE OUT
        // Approx. 10.10 - 10.80 sec
        // -------------------------------------------------

        yield return FadeCanvasGroup(
            imageC,
            1f,
            0f,
            imageCFadeOut
        );


        SetAlpha(imageA, 0f);
        SetAlpha(imageB, 0f);
        SetAlpha(imageC, 0f);

        isPlaying = false;
        flashbackCoroutine = null;
    }


    // =====================================================
    // CROSSFADE
    // =====================================================

    private IEnumerator Crossfade(
        CanvasGroup from,
        CanvasGroup to,
        float duration
    )
    {
        if (duration <= 0f)
        {
            SetAlpha(from, 0f);
            SetAlpha(to, 1f);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            if (from != null)
            {
                from.alpha = Mathf.Lerp(
                    1f,
                    0f,
                    smoothT
                );
            }

            if (to != null)
            {
                to.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    smoothT
                );
            }

            yield return null;
        }

        SetAlpha(from, 0f);
        SetAlpha(to, 1f);
    }


    // =====================================================
    // SIMPLE FADE
    // =====================================================

    private IEnumerator FadeCanvasGroup(
        CanvasGroup target,
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        if (target == null)
            yield break;

        if (duration <= 0f)
        {
            target.alpha = endAlpha;
            yield break;
        }

        float timer = 0f;

        target.alpha = startAlpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            target.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                smoothT
            );

            yield return null;
        }

        target.alpha = endAlpha;
    }


    private void SetAlpha(
        CanvasGroup target,
        float alpha
    )
    {
        if (target != null)
        {
            target.alpha = alpha;
        }
    }
}