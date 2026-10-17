using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("UI Text")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesToSpawnText;
    [SerializeField] private TMP_Text countdownText;

    private int previousWave = -1;
    private int previousEnemiesToSpawn = -1;
    private int previousCountdown = -1;
    private bool previousWaitingState;

    private void Update()
    {
        if (waveManager == null || enemySpawner == null)
            return;

        int currentWave = waveManager.CurrentWave;
        int enemiesToSpawn = enemySpawner.RemainingEnemiesToSpawn;
        bool isWaiting = waveManager.IsWaitingForWave;
        int countdown = isWaiting
            ? Mathf.CeilToInt(waveManager.Countdown)
            : -1;

        if (waveText != null && currentWave != previousWave)
        {
            waveText.text = "Wave " + currentWave + "/10";
            previousWave = currentWave;
        }

        if (enemiesToSpawnText != null &&
            enemiesToSpawn != previousEnemiesToSpawn)
        {
            enemiesToSpawnText.text =
                "Enemies Left to Spawn: " + enemiesToSpawn;

            previousEnemiesToSpawn = enemiesToSpawn;
        }

        if (countdownText != null &&
            (countdown != previousCountdown ||
             isWaiting != previousWaitingState))
        {
            countdownText.text = isWaiting
                ? "Starts in " + countdown
                : "";

            previousCountdown = countdown;
            previousWaitingState = isWaiting;
        }
    }
}