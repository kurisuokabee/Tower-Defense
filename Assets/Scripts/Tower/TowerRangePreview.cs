
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerRangePreview : MonoBehaviour
{
    [SerializeField] private float range = 4f;
    [SerializeField] private int segments = 60;

    private LineRenderer lineRenderer;

    public float Range => range;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        lineRenderer.enabled = false;

        DrawCircle();
    }

    private void DrawCircle()
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 point = new Vector3(
                Mathf.Cos(angle) * range,
                Mathf.Sin(angle) * range,
                0f
            );

            lineRenderer.SetPosition(i, point);
        }
    }

    public void Show(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
