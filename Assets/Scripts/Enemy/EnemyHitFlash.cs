using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void PlayHitFlash()
    {
        if (spriteRenderer == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if(gameObject.activeInHierarchy) flashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        float halfDuration = flashDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            spriteRenderer.color = Color.Lerp(
                originalColor,
                hitColor,
                Mathf.Clamp01(elapsed / halfDuration)
            );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            spriteRenderer.color = Color.Lerp(
                hitColor,
                originalColor,
                Mathf.Clamp01(elapsed / halfDuration)
            );

            yield return null;
        }

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
}
