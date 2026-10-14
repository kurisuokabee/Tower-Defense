using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeCost = 200;
    [SerializeField] private int costIncrease = 100;
    
    private ITowerUpgrade tower;
    private int upgradeLevel;

    public int UpgradeCost => upgradeCost;
    public int UpgradeLevel => upgradeLevel;

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
        if (tower == null || CurrencyManager.Instance == null)
            return false;

        if (!CurrencyManager.Instance.SpendGold(upgradeCost))
            return false;

        tower.Upgrade();

        upgradeLevel++;
        upgradeCost += costIncrease;

        return true;
    }
}
