
using UnityEngine;

public class BoomerangTower : MonoBehaviour, ITowerUpgrade
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float baseDamage = 30f;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 20f;

    [Header("Projectile")]
    [SerializeField] private BoomerangProjectile projectilePrefab;
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


        Vector3 direction =
            target.transform.position - firePoint.position;

        direction.z = 0f;
        float targetDistance = direction.magnitude;
        direction.Normalize();

        SpawnBoomerangProjectile(direction, targetDistance);

        attackTimer = attackCooldown;

    }

    private void SpawnBoomerangProjectile(Vector3 direction, float targetDistance)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = 0f;

            if (projectileCount > 1)
            {
                angle = -spreadAngle / 2f
                    + spreadAngle * i / (projectileCount - 1);
            }

            Vector3 spreadDirection =
                Quaternion.Euler(0f, 0f, angle) * direction;

            BoomerangProjectile projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

            projectile.Initialize(
                spreadDirection,
                baseDamage,
                targetDistance
            );
        }
    }

    public void Upgrade()
    {
        attackRange += 0.5f;
        attackCooldown -= 0.25f;
        baseDamage *= 1.2f;

        projectileCount++;
    }
}
