using UnityEngine;

public class TRTimeFloat : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatAmplitude = 0.05f;   // 上下浮动幅度
    public float floatSpeed = 0.4f;        // 浮动速度

    private Vector3 startLocalPosition;

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed * Mathf.PI * 2f) * floatAmplitude;
        transform.localPosition = startLocalPosition + new Vector3(0f, yOffset, 0f);
    }
}