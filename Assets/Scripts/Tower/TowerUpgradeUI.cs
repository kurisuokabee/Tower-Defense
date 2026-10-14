
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerUpgradeUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeText;

    [Header("Upgrade")]
    [SerializeField] private TowerUpgrade towerUpgrade;

    private void Start()
    {
        upgradeButton.onClick.AddListener(TryUpgrade);

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnGoldChanged += UpdateUI;

        UpdateUI(0);
    }

    private void OnDestroy()
    {
        upgradeButton.onClick.RemoveListener(TryUpgrade);

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnGoldChanged -= UpdateUI;
    }

    private void UpdateUI(int gold)
    {
        if (CurrencyManager.Instance == null || towerUpgrade == null)
        {
            upgradeCanvas.SetActive(false);
            return;
        }

        int cost = towerUpgrade.UpgradeCost;
        bool canAfford = CurrencyManager.Instance.CanAfford(cost);

        upgradeCanvas.SetActive(canAfford);
        upgradeText.text = "Upgrade (" + cost + "G)";
    }

    private void TryUpgrade()
    {
        if (towerUpgrade == null)
            return;

        if (towerUpgrade.Upgrade())
            UpdateUI(CurrencyManager.Instance.Gold);
    }
}
