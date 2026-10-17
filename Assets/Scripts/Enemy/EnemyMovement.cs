using UnityEngine;

public class EnemyMovement : MonoBehaviour, IPooledObject
{
    [SerializeField] private EnemyPath enemyPath;
    [SerializeField] private PlayerBaseHP playerHP;
    [SerializeField] private SpriteRenderer spriteRenderer;


    private EnemySpawner enemySpawner;
    private PooledObject pooledObject;
    private Enemy enemy;

    private int currentPointIndex;
    private bool pathAssigned;
    private bool reachedEnd;

    private float baseDamage = 10f;

    private void Awake()
    {
        pooledObject= GetComponent<PooledObject>();
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(
        EnemyPath path,
        PlayerBaseHP playerHP,
        float damage,
        EnemySpawner spawner)
    {
        enemyPath = path;
        this.playerHP = playerHP;
        baseDamage = damage;
        enemySpawner = spawner;

        // Reset movement state for pooled enemies.
        currentPointIndex = 1;
        pathAssigned = false;
        reachedEnd = false;

        if (enemyPath == null || enemyPath.PointCount == 0)
        {
            Debug.LogError("Enemy path is missing or has no points.", this);
            return;
        }

        transform.position = enemyPath.GetPoint(0);
        pathAssigned = true;
        enabled = true;
    }

    private void Update()
    {
        if (!pathAssigned || reachedEnd)
            return;

        if (currentPointIndex >= enemyPath.PointCount)
        {
            ReachBase();
            return;
        }

        Vector3 target = enemyPath.GetPoint(currentPointIndex);

        float directionX = target.x - transform.position.x;
        UpdateSpriteDirection(directionX);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            GetMoveSpeed() * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            currentPointIndex++;

            if (currentPointIndex >= enemyPath.PointCount)
                ReachBase();
        }
    }

    private void ReachBase()
    {
        if (reachedEnd)
            return;

        reachedEnd = true;
        pathAssigned = false;

        if (playerHP != null)
            playerHP.TakeDamage(baseDamage);

        if (enemySpawner != null && enemy != null)
            enemySpawner.RemoveEnemy(enemy);

        ReturnToPool();
    }

    private float GetMoveSpeed()
    {
        return enemy != null ? enemy.MoveSpeed : 2f;
    }

    public void ReturnToPool()
    {
        pathAssigned = false;

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Debug.LogError($"No PooledEnemy component found on {name}!");
    }

    private void UpdateSpriteDirection(float direction)
    {
        if (direction > 0.01f)
            spriteRenderer.flipX = false;
        else if (direction < -0.01f)
            spriteRenderer.flipX = true;
    }
}
