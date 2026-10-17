using System.Collections.Generic;
using UnityEngine;

public class ElectricTower : MonoBehaviour, ITowerUpgrade
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float baseDamage = 40f;

    [Header("Chain Settings")]
    [SerializeField] private int maxTargets = 5;
    [SerializeField, Range(0f, 1f)]
    private float damageFalloff = 0.25f;
    [SerializeField] private float chainRange = 3f;

    [Header("Lightning Visual")]
    [SerializeField] private LightningEffectPool lightningPool;
    private float attackTimer = 0;
    private TowerEnemyFinder enemyFinder;

    public float AttackRange => attackRange;

    private void Awake()
    {
        enemyFinder = GetComponent<TowerEnemyFinder>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Enemy target = enemyFinder.FindNearestEnemy(attackRange);

            if (target != null)
            {
                Attack(target);
                attackTimer = attackCooldown;
            }
        }
    }

    
    
    private void Attack(Enemy firstTarget)
    {
        HashSet<Enemy> hitEnemies = new();
        Enemy currentTarget = firstTarget;

        float damage = baseDamage;
        Vector3 startPosition = transform.position;

        SpawnLightningAndEffect(hitEnemies, ref currentTarget, ref damage, ref startPosition);
    }

    private void SpawnLightningAndEffect(
        HashSet<Enemy> hitEnemies, 
        ref Enemy currentTarget, 
        ref float damage, 
        ref Vector3 startPosition)
    {
        for (int i = 0; i < maxTargets; i++)
        {   
            if (currentTarget == null)
                break;
            
            EnemyHP enemyHP = currentTarget.Health;

            if (!enemyHP.IsAlive) break;
           
            Vector3 targetPosition = currentTarget.transform.position;

            lightningPool.DrawLightning(
                startPosition,
                targetPosition
            );

            hitEnemies.Add(currentTarget);
            enemyHP.TakeDamage(damage);

            startPosition = targetPosition;

            currentTarget = Utilities.FindNextTarget(
                targetPosition,
                hitEnemies,
                chainRange
            );

            damage *= 1f - damageFalloff;

            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.electric, 0.75f, 0.2f);
        }
    }

    public void Upgrade()
    {
        attackRange += 0.5f;
        attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.35f);
        baseDamage *= 1.4f;

        maxTargets++;

        damageFalloff -= 0.02f;
    }
}
