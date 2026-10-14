
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
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

    private readonly HashSet<Enemy> hitEnemies = new();

    public void Initialize(
    Vector3 direction,
    float projectileDamage,
    float targetDistance)
    {
        startPosition = transform.position;

        direction.z = 0f;
        forwardDirection = direction.normalized;

        // Travel based on the target's distance.
        flightDistance = targetDistance;

        damage = projectileDamage;
        timer = 0f;
        hitEnemies.Clear();
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

        float forwardDistance =
            flightDistance * Mathf.Sin(t * Mathf.PI);

        float sidewaysDistance =
            curveWidth * Mathf.Sin(t * Mathf.PI * 2f);

        transform.position =
            startPosition
            + forwardDirection * forwardDistance
            + sideDirection * sidewaysDistance;

        transform.Rotate(
            0f, 0f, rotationSpeed * Time.deltaTime
        );

        CheckEnemyHits();

        if (t >= 1f)
            Destroy(gameObject);
    }

    private void CheckEnemyHits()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>();

        foreach (Enemy enemy in enemies)
        {   
            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();
            
            if (enemy == null ||
                !enemyHP.IsAlive ||
                hitEnemies.Contains(enemy))
                continue;

            if (Vector3.Distance(
                transform.position,
                enemy.transform.position) <= hitRadius)
            {
                enemyHP.TakeDamage(damage);
                hitEnemies.Add(enemy);
            }
        }
    }
}
