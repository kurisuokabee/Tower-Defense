using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject deathEffectPrefab;

    public void PlayDeathEffect()
    {
        if (deathEffectPrefab == null ||
            ObjectPoolManager.Instance == null)
            return;

        GameObject effect = ObjectPoolManager.Instance.GetObject(
            deathEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        if (effect == null)
            return;
            
        PooledParticleEffect pooledEffect =
            effect.GetComponent<PooledParticleEffect>();

        if (pooledEffect == null)
            pooledEffect = effect.AddComponent<PooledParticleEffect>();

        pooledEffect.Play();
    }
}


