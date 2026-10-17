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
    [SerializeField] private GameObject bossPrefab;

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
    [SerializeField] private float enemyCountMultiplier = 1.4f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float healthIncreasePerWave = 0.1f;
    [SerializeField] private float damageIncreasePerWave = 0.05f;
    [SerializeField] private float speedIncreasePerWave = 0.03f;
    [SerializeField] private int goldIncreaseEveryTwoWaves = 10;

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

        int baseCount = baseEnemyCount + (wave - 1) * extraEnemiesPerWave;
        int enemyCount = Mathf.RoundToInt(
            baseCount * Mathf.Pow(enemyCountMultiplier, (wave - 1) / 5f)
        );

        RemainingEnemiesToSpawn = enemyCount + (wave == 10 ? 1 : 0);

        // Spawn the boss on Path 1 during Wave 10.
        if (wave == 10)
        {
            if (bossPrefab != null && path1 != null)
            {   
                SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.dragon, 0.5f);
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

                if (cameraShake != null)
                    cameraShake.Shake();

                Enemy boss = SpawnEnemy(bossPrefab, path1, wave, false);

                if (boss != null) RemainingEnemiesToSpawn--;
    
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.gameStart, 0.8f);
        
        // Spawn regular enemies.
        for (int i = 0; i < enemyCount; i++)
        {
            if (baseHealth != null && baseHealth.CurrentHP <= 0)
                yield break;

            RemainingEnemiesToSpawn--;

            GameObject prefab = ChooseEnemy(wave);
            EnemyPath path = paths[Random.Range(0, paths.Count)];

            SpawnEnemy(prefab, path, wave, true);

            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.spawn, 0.15f, 0.3f);

            float currentSpawnInterval = Mathf.Max(
                0.1f,
                spawnInterval - ((wave - 1) / 3) * 0.1f
            );

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    
    private Enemy SpawnEnemy(
        GameObject prefab,
        EnemyPath path,
        int wave,
        bool scaleStats)
    {
        GameObject enemyObject = ObjectPoolManager.Instance.GetObject(
            prefab,
            path.GetPoint(0),
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        EnemyHP enemyHP = enemy.Health;
        EnemyMovement movement = enemy.Movement;

        if (enemy == null || enemyHP == null || movement == null)
        {
            Debug.LogError(
                $"Enemy prefab '{prefab.name}' must have Enemy, EnemyHP, and EnemyMovement components."
            );

            ObjectPoolManager.Instance.ReturnObject(enemyObject);
            return null;
        }

        if (scaleStats)
        {
            enemy.InitializeForWave(
                wave,
                healthIncreasePerWave,
                damageIncreasePerWave,
                speedIncreasePerWave,
                goldIncreaseEveryTwoWaves
            );
        }

        enemyHP.InitializeHealth();
        RegisterEnemy(enemy);
        movement.Initialize(path, baseHealth, enemy.Damage, this);

        return enemy;
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

}