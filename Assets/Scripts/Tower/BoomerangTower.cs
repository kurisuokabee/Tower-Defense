
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
                Vector3 direction = target.transform.position - firePoint.position;
                direction.z = 0f;

                float targetDistance = direction.magnitude;

                if (targetDistance > 0f)
                {
                    direction.Normalize();

                    SpawnBoomerangProjectile(direction, targetDistance);

                    attackTimer = attackCooldown;
                }
            }
        }
    }

    private void SpawnBoomerangProjectile(
        Vector3 direction,
        float targetDistance)
    {
        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.boomerang, 0.1f,0.2f);


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

            GameObject projectileObject =
                ObjectPoolManager.Instance.GetObject(
                    projectilePrefab.gameObject,
                    firePoint.position,
                    Quaternion.identity
                );

            if (projectileObject == null)
                continue;

            BoomerangProjectile projectile =
                projectileObject.GetComponent<BoomerangProjectile>();

            if (projectile == null)
            {
                ObjectPoolManager.Instance.ReturnObject(projectileObject);
                continue;
            }

            projectile.Initialize(
                spreadDirection,
                baseDamage,
                targetDistance
            );
        }
    }

    public void Upgrade()
    {
        attackRange += 0.6f;
        attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.3f);
        baseDamage *= 1.5f;
        projectileCount++;
    }
}
