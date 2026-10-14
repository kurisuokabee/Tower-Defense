using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject gruntPrefab;
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private GameObject tankPrefab;

    [Header("Paths")]
    [SerializeField] private EnemyPath path1;
    [SerializeField] private EnemyPath path2;
    [SerializeField] private EnemyPath path3;
    [SerializeField] public PathPreview pathPreview;

    [Header("Base")]
    [SerializeField] private PlayerBaseHP baseHealth;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int baseEnemyCount = 5;
    [SerializeField] private int extraEnemiesPerWave = 2;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;
    public int ActiveEnemyCount => activeEnemies.Count;

    public int RemainingEnemiesToSpawn { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public List<EnemyPath> GetUnlockedPaths(int wave)
    {
        List<EnemyPath> paths = new List<EnemyPath>();

        if (path1 != null)
            paths.Add(path1);

        if (wave >= 4 && path2 != null)
            paths.Add(path2);

        if (wave >= 7 && path3 != null)
            paths.Add(path3);

        return paths;
    }

    public IEnumerator SpawnWave(int wave)
    {
        List<EnemyPath> paths = GetUnlockedPaths(wave);

        int enemyCount = baseEnemyCount + (wave - 1) * extraEnemiesPerWave;
        RemainingEnemiesToSpawn = enemyCount;                 

        for (int i = 0; i < enemyCount; i++)
        {
            if (baseHealth != null && baseHealth.CurrentHP <= 0)
                yield break;
                
            RemainingEnemiesToSpawn--;

            GameObject prefab = ChooseEnemy(wave);
            EnemyPath path = paths[Random.Range(0, paths.Count)];

            path.DrawPath();

            GameObject enemyObject = Instantiate(
                prefab,
                path.GetPoint(0),
                Quaternion.identity
            );

            EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();
            Enemy enemy = enemyObject.GetComponent<Enemy>();

    
            RegisterEnemy(enemy);
            movement.Initialize(path, baseHealth, enemy.Damage, this);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private GameObject ChooseEnemy(int wave)
    {
        if (wave < 3)
            return gruntPrefab;

        if (wave < 5)
            return Random.value < 0.7f ? gruntPrefab : runnerPrefab;

        float roll = Random.value;

        if (roll < 0.45f)
            return gruntPrefab;

        if (roll < 0.8f)
            return runnerPrefab;

        return tankPrefab;
    }

    public void PreviewPathsForWave(int wave)
    {
        List<EnemyPath> paths = GetUnlockedPaths(wave);

        pathPreview.ShowPaths(paths.ToArray());
    }
}