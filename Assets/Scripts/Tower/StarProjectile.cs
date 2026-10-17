
using System.Collections.Generic;
using UnityEngine;

public class StarProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float arcHeight = 1.5f;
    [SerializeField] private float returnDuration = 0.5f;

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

    private Vector3 towerPosition;
    private bool isReturning;
    private Vector3 returnStartPosition;
    private float returnTimer;

    private PooledObject pooledObject;

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();
    }

    public void Initialize(
        Enemy enemy,
        float projectileDamage,
        HashSet<Enemy> targets,
        int limit,
        float searchRange,
        float falloff,
        Vector3 originPosition,
        int generation = 0)
    {
        target = enemy;
        enemyHP = target != null ? target.GetComponent<EnemyHP>() : null;

        damage = projectileDamage;
        selectedTargets = targets ?? new HashSet<Enemy>();
        chainRange = searchRange;
        splitGeneration = generation;
        splitLimit = limit;
        damageFalloff = falloff;

        if (target != null)
            selectedTargets.Add(target);

        startPosition = transform.position;
        travelTimer = 0f;
        travelDuration = 0f;

        towerPosition = originPosition;
        isReturning = false;
        returnTimer = 0f;
        returnStartPosition = transform.position;

        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (isReturning)
        {
            ReturnToTower();
            return;
        }

        //Search for new target when the current target dies
        if (target == null || enemyHP == null || !enemyHP.IsAlive)
        {
            target = Utilities.FindNextTarget(transform.position, selectedTargets, chainRange);

            if (target == null)
            {
                isReturning = true;
                returnTimer = 0f;
                returnStartPosition = transform.position;
                return;
            }

            enemyHP = target.Health;
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

        Vector3 position = Vector3.Lerp(
            startPosition,
            endPosition,
            Ease.EaseInOutQuad(t)
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

        transform.Rotate(0f, 0f, 2f * Time.deltaTime);

        if (t >= 1f)
            HitTarget();
    }

    private void ReturnToTower()
    {
        returnTimer += Time.deltaTime;

        float t = Mathf.Clamp01(
            returnTimer / Mathf.Max(returnDuration, 0.01f)
        );

        transform.position = Vector3.Lerp(
            returnStartPosition,
            towerPosition,
            Ease.EaseInOutQuad(t)
        );

        if (t >= 1f)
            ReturnToPool();
    }

    private void HitTarget()
    {
        if (target == null)
            return;

        Vector3 hitPosition = target.transform.position;

        if (splitGeneration < splitLimit)
            Split(hitPosition);

        if (enemyHP != null && enemyHP.IsAlive)
            enemyHP.TakeDamage(damage);

        ReturnToPool();
    }

    private void Split(Vector3 hitPosition)
    {
        if (ObjectPoolManager.Instance == null ||
            pooledObject == null || pooledObject.Prefab == null)
            return;

        float childDamage = damage * (1f - damageFalloff);

        for (int i = 0; i < 2; i++)
        {
            Enemy nextTarget = Utilities.FindNextTarget(hitPosition, selectedTargets, chainRange);

            if (nextTarget == null)
                break;

            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.star, 0.25f, 0.2f);

            GameObject projectileObject =
                ObjectPoolManager.Instance.GetObject(
                    pooledObject.Prefab,
                    hitPosition,
                    Quaternion.identity
                );

            if (projectileObject == null)
                continue;

            StarProjectile child =
                projectileObject.GetComponent<StarProjectile>();

            if (child == null)
            {
                ObjectPoolManager.Instance.ReturnObject(projectileObject);
                continue;
            }

            child.Initialize(
                nextTarget,
                childDamage,
                selectedTargets,
                splitLimit,
                chainRange,
                damageFalloff,
                towerPosition,
                splitGeneration + 1
            );
        }
    }


    public void ReturnToPool()
    {
        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Debug.LogError("Star projectile is missing PooledObject.");
    }
}
