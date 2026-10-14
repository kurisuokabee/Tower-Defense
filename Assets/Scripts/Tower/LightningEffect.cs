
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private int segments = 8;
    [SerializeField] private float jitter = 0.2f;
    public bool IsPlaying => hideCoroutine != null;
    private LineRenderer line;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 0;
    }

    public void DrawLightning(Vector3 start, Vector3 end)
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(start, end, t);

            if (i > 0 && i < segments)
                point += Random.insideUnitSphere * jitter;

            line.SetPosition(i, point);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        line.positionCount = 0;
        hideCoroutine = null;
    }
}
