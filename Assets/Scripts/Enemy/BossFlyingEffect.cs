
using UnityEngine;

public class BossFlyingEffect : MonoBehaviour
{
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;
    

    private void Start()
    {
        startPosition = transform.localPosition;
        
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * height;

        transform.localPosition = startPosition + Vector3.up * offset;

        //Play a SFX when it goes up
        if (Mathf.Cos(Time.time * speed) > 0f && Mathf.Cos((Time.time - Time.deltaTime) * speed) <= 0f)
        {
            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.dragonFly, 0.5f); 
        }
    }
}
