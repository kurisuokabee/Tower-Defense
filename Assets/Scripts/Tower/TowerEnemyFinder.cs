using UnityEngine;

public class TowerEnemyFinder : MonoBehaviour
{
    public float AttackRange;

    public Enemy FindNearestEnemy(float searchRange)
    {   
        AttackRange = searchRange;
        
        EnemySpawner enemySpawner = EnemySpawner.Instance;
        Enemy nearest = null;

        float nearestDistanceSqr = searchRange * searchRange;
       
        foreach (Enemy enemy in enemySpawner.ActiveEnemies)
        {
            if (enemy == null)
                continue;

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP == null || !enemyHP.IsAlive)
                continue;

            float distanceSqr =
                (enemy.transform.position - transform.position).sqrMagnitude;

            if (distanceSqr <= nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }
}