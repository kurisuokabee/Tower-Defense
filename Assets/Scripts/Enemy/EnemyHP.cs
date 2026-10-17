using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour, IHealth, IPooledObject
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private HealthUI hpUI;

    private EnemyHitFlash hitFlash;
    private EnemyDeathEffect deathEffect;
    private PooledObject pooledObject;
    private bool isDying;

    public float CurrentHP { get; private set; }
    public float MaxHP => enemy.MaxHealth;
    public bool IsAlive => CurrentHP > 0f;
    [SerializeField] private bool isBoss;


    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponent<Enemy>();

        hitFlash = GetComponent<EnemyHitFlash>();
        deathEffect = GetComponent<EnemyDeathEffect>();
        pooledObject = GetComponent<PooledObject>();

        CurrentHP = MaxHP;
    }

    public void InitializeHealth()
    {
        isDying = false;
        CurrentHP = MaxHP;

        if (hpUI != null) hpUI.ResetUI();
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || isDying)
            return;

        CurrentHP = Mathf.Max(0f, CurrentHP - damage);

        hitFlash?.PlayHitFlash();

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.hit, 0.05f, 0.2f);

        if (hpUI != null)
        {
            hpUI.UpdateHP();
            hpUI.UpdateGhostHP();
        }

        if (CurrentHP <= 0f)
            Die();
    }

    private void Die()
    {
        isDying = true;

        deathEffect?.PlayDeathEffect();

        if (isBoss)
        {
            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.dragon, 0.5f);
        }
        else
        {
            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.death, 0.25f);
        }
        

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.AddGold(enemy.GoldReward);

        ReturnToPool();
    }

    
    public void ReturnToPool()
    {
        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.RemoveEnemy(enemy);

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Debug.LogError($"No PooledObject component found on {name}!");
    }
}
