using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningSceneManager : MonoBehaviour
{
    [Header("Opening Audio")]
    [Tooltip("用于播放Evie开场旁白的Audio Source。")]
    public AudioSource openingAudioSource;

    [Header("Fade Images")]
    [Tooltip("桌面模式使用的全屏黑色Image。没有桌面版渐变时可以留空。")]
    public Image desktopBlackScreen;

    [Tooltip("VR相机前方World Space Canvas里的黑色Image。没有VR版渐变时可以留空。")]
    public Image vrBlackScreen;

    [Header("Opening Fade")]
    [Tooltip("场景开始时，由全黑渐亮到正常画面的时间。")]
    [Min(0f)]
    public float openingFadeDuration = 4f;

    [Header("Archive Door")]
    [Tooltip("门的旋转轴物体DoorPivot。不要直接拖入DoorModel。")]
    public Transform doorPivot;

    [Tooltip("门打开后的本地旋转角度。方向错误时，把Y从90改成-90。")]
    public Vector3 doorOpenRotation = new Vector3(0f, 90f, 0f);

    [Tooltip("开门动画持续时间。")]
    [Min(0f)]
    public float doorOpenDuration = 1.2f;

    [Tooltip("可选的开门音效。没有音效可以留空。")]
    public AudioClip doorOpenSFX;

    [Header("Scene Transition")]
    [Tooltip("门打开完成后，等待多久开始渐黑。")]
    [Min(0f)]
    public float delayAfterDoorOpens = 0.5f;

    [Tooltip("进入Archive之前，由正常画面渐黑的时间。")]
    [Min(0f)]
    public float transitionFadeDuration = 1f;

    [Tooltip("需要加载的Archive场景名称，必须与Scene文件名完全一致。")]
    public string archiveSceneName = "SceneA_Archive";

    private bool doorSequenceStarted = false;

    private void Start()
    {
        InitialiseFadeImages();

        StartCoroutine(OpeningSequence());
    }

    /// <summary>
    /// 初始化桌面和VR渐变图像。
    /// 场景开始时，两种黑屏的Alpha都会设置为1。
    /// </summary>
    private void InitialiseFadeImages()
    {
        PrepareFadeImage(desktopBlackScreen);
        PrepareFadeImage(vrBlackScreen);

        SetFadeAlpha(1f);
    }

    /// <summary>
    /// 确保渐变Image已启用，并关闭Raycast Target，
    /// 避免透明Image阻挡桌面鼠标或VR射线交互。
    /// </summary>
    private void PrepareFadeImage(Image fadeImage)
    {
        if (fadeImage == null)
            return;

        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = false;
    }

    /// <summary>
    /// 开场旁白与画面渐亮同时开始。
    /// 不锁定玩家，不等待音频结束，也不控制门的交互状态。
    /// </summary>
    private IEnumerator OpeningSequence()
    {
        if (openingAudioSource != null)
        {
            openingAudioSource.Stop();
            openingAudioSource.Play();
        }

        yield return StartCoroutine(
            Fade(
                startAlpha: 1f,
                endAlpha: 0f,
                duration: openingFadeDuration
            )
        );

        DisableTransparentFadeImages();
    }

    /// <summary>
    /// 将此函数连接到门的EZPZ InteractableGeneral：
    /// On Primary Interact() → OpeningSceneManager.OpenArchiveDoor()
    /// </summary>
    public void OpenArchiveDoor()
    {
        if (doorSequenceStarted)
            return;

        doorSequenceStarted = true;

        StartCoroutine(OpenDoorAndLoadArchive());
    }

    /// <summary>
    /// 播放开门动画，之后渐黑并加载Archive场景。
    /// </summary>
    private IEnumerator OpenDoorAndLoadArchive()
    {
        if (doorOpenSFX != null && doorPivot != null)
        {
            AudioSource.PlayClipAtPoint(
                doorOpenSFX,
                doorPivot.position
            );
        }

        yield return StartCoroutine(OpenDoor());

        if (delayAfterDoorOpens > 0f)
        {
            yield return new WaitForSeconds(
                delayAfterDoorOpens
            );
        }

        EnableFadeImages();

        yield return StartCoroutine(
            Fade(
                startAlpha: 0f,
                endAlpha: 1f,
                duration: transitionFadeDuration
            )
        );

        if (string.IsNullOrWhiteSpace(archiveSceneName))
        {
            Debug.LogError(
                "OpeningSceneManager: Archive Scene Name is empty."
            );

            yield break;
        }

        SceneManager.LoadScene(archiveSceneName);
    }

    /// <summary>
    /// 门绕DoorPivot的本地坐标旋转。
    /// </summary>
    private IEnumerator OpenDoor()
    {
        if (doorPivot == null)
        {
            Debug.LogWarning(
                "OpeningSceneManager: Door Pivot has not been assigned. " +
                "The scene will still fade and load without a door animation."
            );

            yield break;
        }

        Quaternion startRotation = doorPivot.localRotation;
        Quaternion endRotation =
            Quaternion.Euler(doorOpenRotation);

        if (doorOpenDuration <= 0f)
        {
            doorPivot.localRotation = endRotation;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < doorOpenDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalisedTime = Mathf.Clamp01(
                elapsedTime / doorOpenDuration
            );

            float smoothedTime = Mathf.SmoothStep(
                0f,
                1f,
                normalisedTime
            );

            doorPivot.localRotation = Quaternion.Slerp(
                startRotation,
                endRotation,
                smoothedTime
            );

            yield return null;
        }

        doorPivot.localRotation = endRotation;
    }

    /// <summary>
    /// 同时控制桌面黑屏和VR黑屏的透明度。
    /// </summary>
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

            float normalisedTime = Mathf.Clamp01(
                elapsedTime / duration
            );

            float currentAlpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                normalisedTime
            );

            SetFadeAlpha(currentAlpha);

            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    /// <summary>
    /// 设置桌面和VR两张黑色Image的Alpha。
    /// </summary>
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

    /// <summary>
    /// 确保两种Fade Image都处于启用状态。
    /// 用于场景切换前重新渐黑。
    /// </summary>
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

    /// <summary>
    /// 开场渐亮结束后关闭完全透明的Fade Image，
    /// 避免它们继续参与渲染或影响VR显示。
    /// </summary>
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
}