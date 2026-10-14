using UnityEngine;
using System.Collections.Generic;

public class StarTower : MonoBehaviour, ITowerUpgrade
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float baseDamage = 40f;
    [SerializeField] private float chainRange = 4f;
    [SerializeField] private int splitLimit = 2;
    [SerializeField] private float damageFalloff = 0.35f;

    [Header("Projectile")]
    [SerializeField] private StarProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    private TowerEnemyFinder enemyFinder;
    private float attackTimer;

    private void Awake()
    {
        enemyFinder = GetComponent<TowerEnemyFinder>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            return;

        Enemy target =
            enemyFinder.FindNearestEnemy(attackRange);

        if (target == null)
            return;

        SpawnStarProjectile(target);

        attackTimer = attackCooldown;
    }

    private void SpawnStarProjectile(Enemy target)
    {
        StarProjectile projectile = Instantiate(
                    projectilePrefab,
                    firePoint.position,
                    Quaternion.identity
                );

        projectile.Initialize(
            target,
            baseDamage,
            new HashSet<Enemy>(),
            splitLimit,
            chainRange,
            damageFalloff,
            0
        );
    }

    public void Upgrade()
    {
        attackRange += 0.5f;
        attackCooldown -= 0.25f;
        baseDamage *= 1.2f;

        splitLimit++;

        damageFalloff -= 0.05f;
    }
}
