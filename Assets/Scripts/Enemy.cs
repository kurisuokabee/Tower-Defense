using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public float MaxHealth => enemyType.maxHealth;
    public float MoveSpeed => enemyType.moveSpeed;
    public float Damage => enemyType.damage;
    public int GoldReward => enemyType.goldReward;
    
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && enemyType.enemySprite != null)
            spriteRenderer.sprite = enemyType.enemySprite;
    }

    
}
