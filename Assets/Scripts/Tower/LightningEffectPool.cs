
using System.Collections.Generic;
using UnityEngine;

public class LightningEffectPool : MonoBehaviour
{
    [SerializeField] private LightningEffect effectPrefab;
    [SerializeField] private int initialSize = 5;

    private readonly List<LightningEffect> effects = new();
    private int nextEffectIndex;

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            CreateEffect();
    }

    private LightningEffect CreateEffect()
    {
        LightningEffect effect = Instantiate(
            effectPrefab,
            transform
        );

        effects.Add(effect);
        return effect;
    }

    public void DrawLightning(Vector3 start, Vector3 end)
    {
        LightningEffect effect = null;

        // Find an effect that is not currently drawing.
        for (int i = 0; i < effects.Count; i++)
        {
            int index = (nextEffectIndex + i) % effects.Count;

            if (!effects[index].IsPlaying)
            {
                effect = effects[index];
                nextEffectIndex = (index + 1) % effects.Count;
                break;
            }
        }

        if (effect == null)
            effect = CreateEffect();

        effect.DrawLightning(start, end);
    }
}
