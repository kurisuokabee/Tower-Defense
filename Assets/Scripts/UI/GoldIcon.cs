using UnityEngine;
using TMPro;

public class GoldIcon : MonoBehaviour
{
    [SerializeField] float punchScale = 1.2f;
    [SerializeField] float punchDuration = 0.2f;

    Vector3 originalScale;
    float elapsed;
    bool isPunching;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!isPunching)
            return;

        elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(elapsed / punchDuration);

        float scale = Mathf.Lerp(1f, punchScale, Ease.EaseInOutSine(t));

        transform.localScale = originalScale * scale;

        if (t >= 1f)
        {
            transform.localScale = originalScale;
            isPunching = false;
        }
    }

    public void Punch()
    {
        elapsed = 0f;
        isPunching = true;
    }


}