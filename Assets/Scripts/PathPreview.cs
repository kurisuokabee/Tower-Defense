
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPreview : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject previewMarkerPrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waitAtEnd = 0.5f;

    private readonly List<GameObject> markers = new List<GameObject>();
    private readonly List<EnemyPath> activePaths = new List<EnemyPath>();
    private Coroutine previewCoroutine;

    public bool IsPreviewing { get; private set; }

    public void ShowPaths(EnemyPath[] paths)
    {
        HidePaths();

        foreach (EnemyPath path in paths)
        {
            if (path == null)
                continue;

            path.DrawPath();

            activePaths.Add(path);

            GameObject marker = Instantiate(
                previewMarkerPrefab,
                path.GetPoint(0),
                Quaternion.identity
            );

            markers.Add(marker);
        }

        IsPreviewing = true;
        previewCoroutine = StartCoroutine(PreviewAllPaths());
    }

    private IEnumerator PreviewAllPaths()
    {
        List<int> pointIndices = new List<int>();

        foreach (EnemyPath path in activePaths)
            pointIndices.Add(1);

        while (IsPreviewing)
        {
            bool allFinished = true;

            for (int i = 0; i < activePaths.Count; i++)
            {
                EnemyPath path = activePaths[i];
                GameObject marker = markers[i];

                if (marker == null || path == null)
                    continue;

                int pointIndex = pointIndices[i];

                if (pointIndex >= path.PointCount)
                    continue;

                allFinished = false;

                Vector3 target = path.GetPoint(pointIndex);

                marker.transform.position = Vector3.MoveTowards(
                    marker.transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(marker.transform.position, target) < 0.05f)
                {
                    marker.transform.position = target;
                    pointIndices[i]++;
                }
            }

            if (allFinished)
            {
                yield return new WaitForSeconds(waitAtEnd);

                IsPreviewing = false;
                previewCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }

    public void HidePaths()
    {
        IsPreviewing = false;

        if (previewCoroutine != null)
        {
            StopCoroutine(previewCoroutine);
            previewCoroutine = null;
        }

        foreach (GameObject marker in markers)
        {
            if (marker != null)
                Destroy(marker);
        }

        markers.Clear();
        activePaths.Clear();
    }

    private void OnDisable()
    {
        HidePaths();
    }

    public IEnumerator PreviewPathsAndWait(EnemyPath[] paths)
    {
        ShowPaths(paths);

        while (IsPreviewing)
            yield return null;

        HidePaths();
    }
}
