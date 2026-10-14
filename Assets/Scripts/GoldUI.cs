
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        if (CurrencyManager.Instance == null)
            return;

        CurrencyManager.Instance.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(CurrencyManager.Instance.Gold);
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldUI;
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.text = "Gold: " + gold;
    }
}
