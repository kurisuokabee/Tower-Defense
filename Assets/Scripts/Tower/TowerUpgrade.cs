
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeCost = 200;
    [SerializeField] private int costIncrease = 100;
    [SerializeField] private int maxUpgrades = 3;

    private ITowerUpgrade tower;
    private int upgradeLevel = 0;

    public int UpgradeCost => upgradeCost;
    public int UpgradeLevel => upgradeLevel;
    public int CurrentLevel => upgradeLevel;
    public bool CanUpgrade => upgradeLevel < maxUpgrades;

    private void Awake()
    {
        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component is ITowerUpgrade upgradeable)
            {
                tower = upgradeable;
                break;
            }
        }
    }

    public bool Upgrade()
    {
        if (tower == null ||
            CurrencyManager.Instance == null ||
            !CanUpgrade)
        {
            return false;
        }

        if (!CurrencyManager.Instance.SpendGold(upgradeCost))
            return false;

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.upgrade, 0.25f);
        tower.Upgrade();

        upgradeLevel++;
        upgradeCost += costIncrease;

        return true;
    }
}
