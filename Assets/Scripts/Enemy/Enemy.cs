using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public EnemyHP Health { get; private set; }
    public EnemyMovement Movement { get; private set; }

    private float maxHealth;
    private float damage;
    private float moveSpeed;
    private int goldReward;

    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
    public int GoldReward => goldReward;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && enemyType.enemySprite != null)
            spriteRenderer.sprite = enemyType.enemySprite;

        maxHealth = enemyType.maxHealth;
        damage = enemyType.damage;
        moveSpeed = enemyType.moveSpeed;
        goldReward = enemyType.goldReward;

        Health = GetComponent<EnemyHP>();
        Movement = GetComponent<EnemyMovement>();
    }

    public void InitializeForWave(
        int wave,
        float healthIncreasePerWave,
        float damageIncreasePerWave,
        float speedIncreasePerWave,
        int goldIncreaseEveryTwoWaves)
    {
        maxHealth = enemyType.maxHealth *
            Mathf.Pow(1f + healthIncreasePerWave, wave - 1);

        damage = enemyType.damage *
            Mathf.Pow(1f + damageIncreasePerWave, (wave - 1) / 3);

        moveSpeed = enemyType.moveSpeed *
            Mathf.Pow(1f + speedIncreasePerWave, wave - 1);

        goldReward = enemyType.goldReward +
            ((wave - 1) / 2) * goldIncreaseEveryTwoWaves;
    }
}
