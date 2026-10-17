using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float duration = 0.2f;

    private Vector3 originalScale;
    private Vector3 startScale;
    private Vector3 targetScale;
    private float elapsedTime;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        startScale = originalScale;
    }

    
    private void Update()
    {
        if (Vector3.Distance(transform.localScale, targetScale) < 0.001f)
            return;

        elapsedTime += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(elapsedTime / duration);
    
        transform.localScale = Vector3.Lerp(startScale, targetScale, Ease.EaseInOutQuad(t));

        if (t >= 1f)
            transform.localScale = targetScale;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale);
    }

    private void StartScaleAnimation(Vector3 newTarget)
    {
        startScale = transform.localScale;
        targetScale = newTarget;
        elapsedTime = 0f;
    }
}
