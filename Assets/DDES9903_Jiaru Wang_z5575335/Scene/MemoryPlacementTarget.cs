using System.Collections;
using UnityEngine;

public class MemoryPlacementTarget : MonoBehaviour
{
    [Header("Snap Settings")]
    public Transform snapPoint;

    [Header("Current Guidance Lights")]
    public Light objectGlow;
    public Light targetGlow;

    [Header("Memory Audio")]
    public AudioSource memoryAudio;

    [Header("Next Guidance Lights")]
    public Light nextObjectGlow;
    public Light nextTargetGlow;

    [Header("Fade Settings")]
    public float fadeOutDuration = 2.0f;
    public float fadeInDuration = 1.5f;

    private bool placed = false;

    private float nextObjectGlowIntensity = 1f;
    private float nextTargetGlowIntensity = 1f;

    private void Start()
    {
        // 记住下一组灯原本设置的亮度
        if (nextObjectGlow != null)
        {
            nextObjectGlowIntensity = nextObjectGlow.intensity;
            nextObjectGlow.intensity = 0f;
        }

        if (nextTargetGlow != null)
        {
            nextTargetGlowIntensity = nextTargetGlow.intensity;
            nextTargetGlow.intensity = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (placed) return;

        Holdable holdable = other.GetComponent<Holdable>();

        if (holdable == null)
        {
            holdable = other.GetComponentInParent<Holdable>();
        }

        if (holdable != null && holdable.CompareTag("MemoryObject"))
        {
            SnapObject(holdable);
        }
    }

    private void SnapObject(Holdable holdable)
    {
        placed = true;

        // 强制从 EZPZ 的抓取状态中释放
        holdable.ForceDrop();

        Rigidbody rb = holdable.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // 自动吸附并摆正
        if (snapPoint != null)
        {
            holdable.transform.position = snapPoint.position;
            holdable.transform.rotation = snapPoint.rotation;
        }
        else
        {
            Debug.LogWarning("MemoryPlacementTarget: Snap Point is missing!");
        }

        Debug.Log("Memory object placed correctly!");

        StartCoroutine(PlayMemorySequence());
    }

    private IEnumerator PlayMemorySequence()
    {
        // 开始播放这一段记忆音频
        if (memoryAudio != null)
        {
            memoryAudio.Play();
        }

        // 当前物品 + 正确位置的灯光同时慢慢变暗
        yield return StartCoroutine(
            FadeCurrentLightsOut()
        );

        // 等待音频完整播放结束
        if (memoryAudio != null)
        {
            while (memoryAudio.isPlaying)
            {
                yield return null;
            }
        }

        // 音频结束后，下一组引导灯慢慢亮起来
        yield return StartCoroutine(
            FadeNextLightsIn()
        );
    }

    private IEnumerator FadeCurrentLightsOut()
    {
        float objectStartIntensity =
            objectGlow != null ? objectGlow.intensity : 0f;

        float targetStartIntensity =
            targetGlow != null ? targetGlow.intensity : 0f;

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeOutDuration);

            if (objectGlow != null)
            {
                objectGlow.intensity =
                    Mathf.Lerp(objectStartIntensity, 0f, t);
            }

            if (targetGlow != null)
            {
                targetGlow.intensity =
                    Mathf.Lerp(targetStartIntensity, 0f, t);
            }

            yield return null;
        }

        if (objectGlow != null)
            objectGlow.intensity = 0f;

        if (targetGlow != null)
            targetGlow.intensity = 0f;
    }

    private IEnumerator FadeNextLightsIn()
    {
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeInDuration);

            if (nextObjectGlow != null)
            {
                nextObjectGlow.intensity =
                    Mathf.Lerp(0f, nextObjectGlowIntensity, t);
            }

            if (nextTargetGlow != null)
            {
                nextTargetGlow.intensity =
                    Mathf.Lerp(0f, nextTargetGlowIntensity, t);
            }

            yield return null;
        }

        if (nextObjectGlow != null)
            nextObjectGlow.intensity = nextObjectGlowIntensity;

        if (nextTargetGlow != null)
            nextTargetGlow.intensity = nextTargetGlowIntensity;
    }
}