using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TRPortalRing : MonoBehaviour
{
    [Header("Ring Shape")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private int segments = 64;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        BuildRing();
    }

    private void OnValidate()
    {
        BuildRing();
    }

    private void BuildRing()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            return;

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle =
                (float)i / segments *
                Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(
                i,
                new Vector3(x, y, 0f)
            );
        }
    }
}