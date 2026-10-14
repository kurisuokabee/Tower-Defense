
using System.Collections.Generic;
using UnityEngine;

public class StarProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float arcHeight = 1.5f;


    private Enemy target;
    private float damage;
    private float chainRange;
   
    private Vector3 startPosition;
    private float travelTimer;
    private float travelDuration;
    private int splitGeneration;
    private int splitLimit;
    private float damageFalloff;
    private HashSet<Enemy> selectedTargets;
    private EnemyHP enemyHP;

    public void Initialize(
        Enemy enemy,
        float projectileDamage,
        HashSet<Enemy> targets,
        int limit,
        float searchRange,
        float falloff,
        int generation = 0
        )
    {
        target = enemy;
        damage = projectileDamage;
        selectedTargets = targets;
        chainRange = searchRange;
        splitGeneration = generation;
        splitLimit = limit;
        damageFalloff = falloff;

        selectedTargets.Add(target);

        startPosition = transform.position;
        travelTimer = 0f;
        travelDuration = 0f;

        enemyHP = enemy.GetComponent<EnemyHP>();
    }

    private void Update()
    {
        if (target == null || !enemyHP.IsAlive)
        {
            target = FindNearestUnselectedEnemy(transform.position);

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            selectedTargets.Add(target);

            startPosition = transform.position;
            travelTimer = 0f;
            travelDuration = 0f;
        }

        StarArcMovement();
    }

    private void StarArcMovement()
    {
        if (travelDuration <= 0f)
        {
            startPosition = transform.position;

            float distance = Vector3.Distance(
                startPosition,
                target.transform.position
            );

            travelDuration = Mathf.Max(distance / speed, 0.05f);
            travelTimer = 0f;
        }

        travelTimer += Time.deltaTime;

        float t = Mathf.Clamp01(travelTimer / travelDuration);

        Vector3 endPosition = target.transform.position;

        // Move in an arc toward the target.
        Vector3 position = Vector3.Lerp(
            startPosition,
            endPosition,
            t
        );

        position.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = position;

        Vector3 direction = endPosition - startPosition;

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (t >= 1f)
            HitTarget();
    }


    
    private void HitTarget()
    {
        if (target == null) return;

        Vector3 hitPosition = target.transform.position;
        enemyHP.TakeDamage(damage);

        if (splitGeneration < splitLimit)
        {
            float childDamage = damage * (1f - damageFalloff);

            for (int i = 0; i < 2; i++)
            {
                Enemy nextTarget =
                    FindNearestUnselectedEnemy(hitPosition);

                if (nextTarget == null)
                    continue;

                selectedTargets.Add(nextTarget);

                StarProjectile child = Instantiate(
                    this,
                    hitPosition,
                    Quaternion.identity
                );

                child.Initialize(
                    nextTarget,
                    childDamage,
                    selectedTargets,
                    splitLimit,
                    chainRange,
                    damageFalloff,
                    splitGeneration + 1
                );
            }
        }

        Destroy(gameObject);
    }

    private Enemy FindNearestUnselectedEnemy(
        Vector3 position)
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>();

        Enemy nearest = null;
        float nearestDistance = chainRange;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null ||
                !enemyHP.IsAlive ||
                selectedTargets.Contains(enemy))
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
}
