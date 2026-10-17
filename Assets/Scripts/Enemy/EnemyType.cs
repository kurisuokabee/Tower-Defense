using UnityEngine;

[CreateAssetMenu(
    fileName = "New Enemy Type",
    menuName = "Tower Defense/Enemy Type"
)]
public class EnemyType : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float damage = 10f;
    public int goldReward = 10;

    [Header("Visuals")]
    public Sprite enemySprite;
}
