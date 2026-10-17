using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private TrailRenderer[] trails;
    private bool isReturning;

    public GameObject Prefab { get; private set; }

    public void SetPrefab(GameObject prefab)
    {
        Prefab = prefab;
    }

    private void Awake()
    {
        trails = GetComponentsInChildren<TrailRenderer>(true);
    }

    public void OnSpawned()
    {
        isReturning = false;
        ClearTrails();
    }

    public void ReturnToPool()
    {
        if (isReturning)
            return;

        isReturning = true;
        ClearTrails();

        if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.ReturnObject(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void ClearTrails()
    {
        if (trails == null)
            return;

        foreach (TrailRenderer trail in trails)
        {
            if (trail != null)
                trail.Clear();
        }
    }
}
