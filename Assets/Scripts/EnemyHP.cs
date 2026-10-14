using UnityEngine;

public class EnemyHP : MonoBehaviour, IHealth
{
    [SerializeField] Enemy enemy;
    [SerializeField] HealthUI hpUI;
    
    public float CurrentHP { get; private set; }
    public float MaxHP => enemy.MaxHealth;
    public bool IsAlive => CurrentHP > 0f;

    void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive)
            return;

        CurrentHP -= damage;

        hpUI.UpdateHP();
        hpUI.UpdateGhostHP();

        if (CurrentHP <= 0f)
        {
            CurrentHP = 0f;

            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.AddGold(enemy.GoldReward);
            
            EnemySpawner.Instance.RemoveEnemy(enemy);
            Destroy(gameObject);
        }
    }
}