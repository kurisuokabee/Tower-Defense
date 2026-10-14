using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyPath enemyPath;
    [SerializeField] private PlayerBaseHP playerHP;
    private EnemySpawner enemySpawner;
    private int currentPointIndex;
    private bool pathAssigned;
    
    private float baseDamage = 10;
    private bool reachedEnd;

    public void Initialize(EnemyPath path, PlayerBaseHP playerHP, float damage, EnemySpawner spawner)
    {
        enemyPath = path;
        this.playerHP = playerHP;
        baseDamage = damage;
        enemySpawner = spawner;
    }
    

    private void Start()
    {
        if (enemyPath == null || enemyPath.PointCount == 0)
        {
            Debug.LogError("Enemy path is missing or has no points.", this);
            enabled = false;
            return;
        }

        transform.position = enemyPath.GetPoint(0);
        currentPointIndex = 1;
        pathAssigned = true;
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

        if (playerHP != null)
            playerHP.TakeDamage(baseDamage);

        if (enemySpawner != null)
            enemySpawner.RemoveEnemy(gameObject.GetComponent<Enemy>());

        Destroy(gameObject);
    }

    private float GetMoveSpeed()
    {
        Enemy health = GetComponent<Enemy>();

        if (health != null)
            return health.MoveSpeed;

        return 2f;
    }
}
