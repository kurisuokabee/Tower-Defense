
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

    private void Update()
    {
        if (waveManager == null || enemySpawner == null)
            return;

        if (waveText != null)
        {
            waveText.text =
                "Wave " + waveManager.CurrentWave + " / 10";
        }

        if (enemiesToSpawnText != null)
        {
            enemiesToSpawnText.text =
                "Enemies Left to Spawn: " +
                enemySpawner.RemainingEnemiesToSpawn;
        }

        if (countdownText != null)
        {
            if (waveManager.IsWaitingForWave)
            {
                countdownText.text =
                    "Starts in " +
                    Mathf.CeilToInt(waveManager.Countdown);
            }
            else
            {
                countdownText.text = "";
            }
        }
    }
}
