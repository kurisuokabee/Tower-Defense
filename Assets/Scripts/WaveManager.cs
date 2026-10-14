using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private float timeBetweenWaves = 15f;

    public int CurrentWave { get; private set; }
    public bool IsWaveActive { get; private set; }
    public bool IsWaitingForWave { get; private set; }
    public float Countdown { get; private set; }

    private bool gameEnded;
    private bool skipCountdown;

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        for (int wave = 1; wave <= totalWaves; wave++)
        {
            if (gameEnded)
                yield break;

            CurrentWave = wave;
            IsWaveActive = false;
            IsWaitingForWave = true;
            skipCountdown = false;

            // Preview all active paths first.
            if (enemySpawner.pathPreview != null && enemySpawner != null)
            {
                List<EnemyPath> paths = enemySpawner.GetUnlockedPaths(wave);

                yield return StartCoroutine(
                    enemySpawner.pathPreview.PreviewPathsAndWait(paths.ToArray())
                );
            }

            // Start the countdown only after the preview finishes.
            Countdown = timeBetweenWaves;

            while (Countdown > 0f && !skipCountdown)
            {
                if (gameEnded)
                    yield break;

                Countdown -= Time.deltaTime;
                yield return null;
            }

            IsWaitingForWave = false;
            IsWaveActive = true;

            yield return StartCoroutine(enemySpawner.SpawnWave(wave));

            while (enemySpawner.ActiveEnemyCount > 0)
            {
                if (gameEnded)
                    yield break;

                yield return null;
            }

            IsWaveActive = false;
            Debug.Log("Wave " + wave + " finished!");
        }

        if (!gameEnded)
            Debug.Log("You Win!");
    }

    public void StartNextWave()
    {
        if (IsWaitingForWave)
            skipCountdown = true;
    }

    public void EndGame()
    {
        gameEnded = true;
        IsWaveActive = false;
        IsWaitingForWave = false;
        StopAllCoroutines();
    }
}