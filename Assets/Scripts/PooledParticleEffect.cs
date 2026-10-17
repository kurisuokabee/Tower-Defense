using UnityEngine;

public class PooledParticleEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private PooledObject pooledObject;

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();

        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>(true);
    }

    public void Play()
    {
        if (particles == null)
        {
            pooledObject?.ReturnToPool();
            return;
        }

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particles.Play(true);
    }

    private void Update()
    {
        if (particles != null && !particles.IsAlive(true))
            pooledObject?.ReturnToPool();
    }
}
