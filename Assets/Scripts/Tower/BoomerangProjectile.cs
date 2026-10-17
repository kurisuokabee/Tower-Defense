
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour, IPooledObject
{
    [Header("Movement")]
    [SerializeField] private float flightDuration = 1.2f;
    [SerializeField] private float flightDistance = 5f;
    [SerializeField] private float curveWidth = 2f;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 720f;

    private Vector3 startPosition;
    private Vector3 forwardDirection;
    private float damage;
    private float timer;

    private PooledObject pooledObject;
    private readonly HashSet<Enemy> hitEnemies = new();
    private readonly List<Enemy> enemyBuffer = new List<Enemy>();

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();
    }

    public void Initialize(
        Vector3 direction,
        float projectileDamage,
        float targetDistance)
    {
        startPosition = transform.position;

        direction.z = 0f;
        forwardDirection = direction.normalized;

        flightDistance = targetDistance;
        damage = projectileDamage;
        timer = 0f;

        hitEnemies.Clear();
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        BoomerangMovement();
    }

    private void BoomerangMovement()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / flightDuration);

        Vector3 sideDirection = new Vector3(
            -forwardDirection.y,
            forwardDirection.x,
            0f
        );

        float forwardDistance = flightDistance * Mathf.Sin(t * Mathf.PI);
        float sidewaysDistance = curveWidth * Mathf.Sin(t * Mathf.PI * 2f);

        transform.position = startPosition
            + forwardDirection * forwardDistance
            + sideDirection * sidewaysDistance;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        CheckEnemyHits();

        if (t >= 1f)
            ReturnToPool();
    }

    
    private void CheckEnemyHits()
    {
        EnemySpawner enemySpawner = EnemySpawner.Instance;

        if (enemySpawner == null)
            return;

        enemyBuffer.Clear();
        enemyBuffer.AddRange(enemySpawner.ActiveEnemies);

        float hitRadiusSqr = hitRadius * hitRadius;
        Vector3 position = transform.position;

        foreach (Enemy enemy in enemyBuffer)
        {
            if (enemy == null || hitEnemies.Contains(enemy))
                continue;

            EnemyHP enemyHP = enemy.Health;

            if (enemyHP == null || !enemyHP.IsAlive)
                continue;

            float distanceSqr =
                (position - enemy.transform.position).sqrMagnitude;

            if (distanceSqr <= hitRadiusSqr)
            {
                hitEnemies.Add(enemy);
                enemyHP.TakeDamage(damage);
            }
        }
    }


    public void ReturnToPool()
    {
        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.ReturnObject(gameObject);
    }
}
