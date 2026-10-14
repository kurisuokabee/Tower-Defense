using UnityEngine;

public class TowerEnemyFinder : MonoBehaviour
{
    public Enemy FindNearestEnemy(float searchRange)
    {   
        EnemySpawner enemySpawner = EnemySpawner.Instance;
        
        Enemy nearest = null;
        float nearestDistance = searchRange;

        foreach (Enemy enemy in enemySpawner.ActiveEnemies)
        {   
            if (enemy == null)
                continue;

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP == null || !enemyHP.IsAlive)
                continue;

            float distance = Vector3.Distance(
                transform.position,
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