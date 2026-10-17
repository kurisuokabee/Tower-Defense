
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private MonoBehaviour healthSource;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider ghostBar;

    [SerializeField] private float ghostDelay = 0.5f;
    [SerializeField] private float ghostDuration = 0.5f;

    private IHealth health;

    private float ghostHP;
    private float ghostStartHP;
    private float ghostTimer;
    private float ghostElapsed;
    private bool ghostAnimating;

    private void Awake()
    {
        health = healthSource as IHealth;

        if (health == null)
        {
            Debug.LogError(
                "HealthUI: Health Source must implement IHealth.",
                this
            );

            enabled = false;
            return;
        }
    }

    private void Start()
    {
        ResetUI();
    }

    private void Update()
    {
        if (health == null)
            return;

        hpBar.value = health.CurrentHP;

        if (!ghostAnimating)
            return;

        ghostTimer += Time.deltaTime;

        if (ghostTimer < ghostDelay)
            return;

        ghostElapsed += Time.deltaTime;

        float t = ghostDuration > 0f
            ? Mathf.Clamp01(ghostElapsed / ghostDuration)
            : 1f;

        ghostHP = Mathf.Lerp(
            ghostStartHP,
            health.CurrentHP,
            Ease.EaseOutQuad(t)
        );

        ghostBar.value = ghostHP;

        if (t >= 1f)
        {
            ghostHP = health.CurrentHP;
            ghostBar.value = ghostHP;
            ghostAnimating = false;
        }
    }

    public void ResetUI()
    {
        if (health == null || hpBar == null || ghostBar == null)
            return;

        hpBar.minValue = 0f;
        hpBar.maxValue = health.MaxHP;
        hpBar.value = health.CurrentHP;

        ghostBar.minValue = 0f;
        ghostBar.maxValue = health.MaxHP;
        ghostBar.value = health.CurrentHP;

        ghostHP = health.CurrentHP;
        ghostStartHP = ghostHP;

        ghostTimer = 0f;
        ghostElapsed = 0f;
        ghostAnimating = false;
    }

    public void UpdateHP()
    {
        if (health == null || hpBar == null)
            return;

        hpBar.maxValue = health.MaxHP;
        hpBar.value = health.CurrentHP;
    }

    public void UpdateGhostHP()
    {
        if (health == null || ghostBar == null)
            return;

        // Only animate the ghost bar when health decreases.
        if (health.CurrentHP >= ghostHP)
        {
            ghostHP = health.CurrentHP;
            ghostBar.maxValue = health.MaxHP;
            ghostBar.value = ghostHP;
            ghostAnimating = false;
            ghostTimer = 0f;
            ghostElapsed = 0f;
            return;
        }

        ghostStartHP = ghostHP;
        ghostTimer = 0f;
        ghostElapsed = 0f;
        ghostAnimating = true;
    }
}
