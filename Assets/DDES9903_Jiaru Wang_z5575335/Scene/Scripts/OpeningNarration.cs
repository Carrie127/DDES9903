using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpeningNarration : MonoBehaviour
{
    [Header("Opening Audio")]
    public AudioSource openingVoiceAudio;

    [Header("UI")]
    public Image blackScreen;
    public GameObject narrationPanel;

    [Header("Player Control")]
    public Transform playerRoot;
    public NarrativeClickManager clickManager;

    [Header("Timing")]
    public float fadeDelay = 0.3f;
    public float fadeInDuration = 3f;
    public float extraWaitAfterAudio = 0.5f;

    private Vector3 lockedPosition;
    private bool lockPlayerPosition = false;

    void Start()
    {
        StartCoroutine(PlayOpening());
    }

    void LateUpdate()
    {
        if (lockPlayerPosition && playerRoot != null)
        {
            playerRoot.position = lockedPosition;
        }
    }

    private IEnumerator PlayOpening()
    {
        // 不显示旧字幕
        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        // 暂时不能点击线索
        if (clickManager != null)
            clickManager.enabled = false;

        // 锁住玩家位置，但不锁镜头
        if (playerRoot != null)
        {
            lockedPosition = playerRoot.position;
            lockPlayerPosition = true;
        }

        // 开场黑屏
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            c.a = 1f;
            blackScreen.color = c;
        }

        yield return new WaitForSeconds(fadeDelay);

        // 播放Opening音频
        if (openingVoiceAudio != null)
            openingVoiceAudio.Play();

        // 画面渐入
        yield return StartCoroutine(FadeInFromBlack());

        // 等音频结束
        if (openingVoiceAudio != null && openingVoiceAudio.clip != null)
        {
            while (openingVoiceAudio.isPlaying)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(extraWaitAfterAudio);

        // 恢复玩家移动和点击
        lockPlayerPosition = false;

        if (clickManager != null)
            clickManager.enabled = true;
    }

    private IEnumerator FadeInFromBlack()
    {
        if (blackScreen == null)
            yield break;

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