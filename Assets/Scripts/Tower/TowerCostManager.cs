
using System.Collections.Generic;
using UnityEngine;

public class TowerCostManager : MonoBehaviour
{
    public static TowerCostManager Instance { get; private set; }

    [SerializeField] private int costIncreasePerTower = 100;

    private Dictionary<GameObject, int> towerCounts = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int GetTowerCost(GameObject prefab, int baseCost)
    {
        int count = towerCounts.TryGetValue(prefab, out int value)
            ? value
            : 0;

        return baseCost + count * costIncreasePerTower;
    }

    public void RegisterTower(GameObject prefab)
    {
        if (prefab == null)
            return;

        int count = towerCounts.TryGetValue(prefab, out int value)
            ? value
            : 0;

        towerCounts[prefab] = count + 1;
    }

    public int GetTowerCount(GameObject prefab)
    {
        return towerCounts.TryGetValue(prefab, out int count)
            ? count
            : 0;
    }
}
