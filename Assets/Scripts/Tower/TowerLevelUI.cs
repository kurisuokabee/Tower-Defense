
using TMPro;
using UnityEngine;

public class TowerLevelUI : MonoBehaviour
{
    [SerializeField] private TowerUpgrade towerUpgrade;
    [SerializeField] private TMP_Text levelText;
    
    private void LateUpdate()
    {
        if (towerUpgrade == null || levelText == null)
            return;

        levelText.text = towerUpgrade.CurrentLevel switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            _ => ""
        };
    }
}
