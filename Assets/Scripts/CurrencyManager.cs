
using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startingGold = 1000;

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Gold = startingGold;
    }

    public bool CanAfford(int amount)
    {
        return Gold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Gold -= amount;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }
}
