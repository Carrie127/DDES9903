using UnityEngine;

public class MiaLightPulse : MonoBehaviour
{
    [Header("Light")]
    public Light miaLight;

    [Header("Pulse")]
    public float baseIntensity = 4f;
    public float pulseAmount = 0.6f;
    public float pulseSpeed = 1.2f;

    [Header("Floating")]
    public float floatAmount = 0.04f;
    public float floatSpeed = 0.8f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed);

        if (miaLight != null)
        {
            miaLight.intensity = baseIntensity + pulse * pulseAmount;
        }

        transform.localPosition = startPosition + new Vector3(0f, pulse * floatAmount, 0f);
    }
}