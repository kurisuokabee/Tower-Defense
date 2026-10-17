using UnityEngine;

public class EnemyBob : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 0.1f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 originalScale;
    private float timer;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        timer += Time.deltaTime * bobSpeed;

        float t = (Mathf.Sin(timer) + 1f) / 2f;

        Vector3 targetScale = originalScale * (1f + scaleAmount);

        transform.localScale = Vector3.Lerp(
            originalScale,
            targetScale,
            Ease.EaseInOutQuad(t)
        );
    }
}
