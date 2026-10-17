
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
    private float attackTimer = 0;

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
                SpawnStarProjectile(target);
            }

            attackTimer = attackCooldown;
        }
    }

    private void SpawnStarProjectile(Enemy target)
    {
        GameObject projectileObject = ObjectPoolManager.Instance.GetObject(
            projectilePrefab.gameObject,
            firePoint.position,
            Quaternion.identity
        );

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.star, 0.15f, 0.2f);

        if (projectileObject == null)
            return;

        StarProjectile projectile =
            projectileObject.GetComponent<StarProjectile>();

        if (projectile == null)
        {
            ObjectPoolManager.Instance.ReturnObject(projectileObject);
            return;
        }

        projectile.Initialize(
            target,
            baseDamage,
            new HashSet<Enemy>(),
            splitLimit,
            chainRange,
            damageFalloff,
            transform.position,
            0
        );
    }

    public void Upgrade()
    {
        attackRange += 0.5f;
        attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.3f);
        baseDamage *= 1.4f;
        splitLimit++;
        damageFalloff = Mathf.Max(0f, damageFalloff - 0.1f);
    }
}
