    
using UnityEngine;
using UnityEngine.UI;

public class TowerPlacementButton : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Button button;
    [SerializeField] private int towerCost = 100;

    public int TowerCost => towerCost;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(SelectTower);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(SelectTower);
    }

    private void SelectTower()
    {
        TowerPlacementManager.Instance.SelectTower(
            towerPrefab,
            towerCost
        );
    }
}
