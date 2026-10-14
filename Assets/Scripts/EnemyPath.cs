
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class EnemyPath : MonoBehaviour
{
    [System.Serializable]
    public class PathSegment
    {
        public Transform point;
        public bool isCurved;
        public Transform controlPoint;
    }

    [SerializeField] private Transform startPoint;
    [SerializeField] private PathSegment[] segments;
    [SerializeField, Min(2)] private int curveResolution = 30;

    private LineRenderer lineRenderer;

    private List<Vector3> pathPoints = new List<Vector3>();

    public int PointCount => pathPoints.Count;

    public Vector3 GetPoint(int index)
    {
        return pathPoints[index];
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawPath();

        lineRenderer.enabled = false;
    }

    public void DrawPath()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (startPoint == null || segments == null)
            return;

        List<Vector3> points = new List<Vector3>();
        Vector3 current = startPoint.position;
        points.Add(current);

        foreach (PathSegment segment in segments)
        {
            if (segment.point == null)
                return;

            Vector3 end = segment.point.position;

            if (segment.isCurved && segment.controlPoint != null)
            {
                Vector3 control = segment.controlPoint.position;

                for (int i = 1; i <= curveResolution; i++)
                {
                    float t = i / (float)curveResolution;
                    float u = 1f - t;

                    Vector3 position =
                        u * u * current +
                        2f * u * t * control +
                        t * t * end;

                    points.Add(position);
                }
            }
            else
            {
                points.Add(end);
            }

            current = end;
        }

        pathPoints = points;

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
                DrawPath();
        }
    }
}
