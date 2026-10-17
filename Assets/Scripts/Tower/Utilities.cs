using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{   
    private static readonly List<Enemy> enemyBuffer = new List<Enemy>();

    public static Enemy FindNextTarget(Vector3 position, HashSet<Enemy> targetableEnemy, float range)
    {
        Enemy nearest = null;
        float nearestDistanceSqr = range * range;

        enemyBuffer.Clear();
        enemyBuffer.AddRange(EnemySpawner.Instance.ActiveEnemies);

        foreach (Enemy enemy in enemyBuffer)
        {
            if (enemy == null || targetableEnemy.Contains(enemy))
                continue;

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP == null || !enemyHP.IsAlive)
                continue;

            float distanceSqr =
                (position - enemy.transform.position).sqrMagnitude;

            if (distanceSqr <= nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }
}