
using TMPro;
using UnityEngine;

public class TowerCostUI : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int baseCost = 100;

    [Header("UI")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text countText;

    private void Update()
    {
        if (TowerCostManager.Instance == null)
            return;

        costText.text = TowerCostManager.Instance.GetTowerCost(
            towerPrefab,
            baseCost
        ).ToString();

        countText.text = TowerCostManager.Instance.GetTowerCount(
            towerPrefab
        ).ToString();
    }
}
