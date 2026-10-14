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
    private float attackTimer;
    private TowerEnemyFinder enemyFinder;

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
            
            EnemyHP enemyHP = currentTarget.GetComponent<EnemyHP>();

            if (!enemyHP.IsAlive) break;
           
            Vector3 targetPosition = currentTarget.transform.position;

            lightningPool.DrawLightning(
                startPosition,
                targetPosition
            );

            hitEnemies.Add(currentTarget);
            enemyHP.TakeDamage(damage);

            startPosition = targetPosition;

            currentTarget = FindNextTarget(
                targetPosition,
                hitEnemies
            );

            damage *= 1f - damageFalloff;
        }
    }

    private Enemy FindNextTarget(
        Vector3 position,
        HashSet<Enemy> hitEnemies)
    {
        Enemy nearest = null;
        float nearestDistance = chainRange;

        Enemy[] enemies = FindObjectsByType<Enemy>();

        foreach (Enemy enemy in enemies)
        {   
            if (enemy == null)
                continue;

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP == null || !enemyHP.IsAlive)
                continue;

            if (enemy == null ||
                !enemyHP.IsAlive ||
                hitEnemies.Contains(enemy))
                continue;

            float distance = Vector3.Distance(
                position,
                enemy.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    public void Upgrade()
    {
        attackRange += 0.5f;
        attackCooldown -= 0.25f;
        baseDamage *= 1.2f;

        maxTargets++;

        damageFalloff -= 0.02f;
    }
}
