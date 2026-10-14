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
        }
    }

    private void Start()
    {
        hpBar.minValue = 0f;
        hpBar.maxValue = health.MaxHP;
        hpBar.value = health.CurrentHP;

        ghostBar.minValue = 0f;
        ghostBar.maxValue = health.MaxHP;
        ghostBar.value = health.CurrentHP;

        ghostHP = health.CurrentHP;
    }

    private void Update()
    {
        hpBar.value = health.CurrentHP;

        if (ghostHP > health.CurrentHP)
        {
            ghostTimer += Time.deltaTime;

            if (ghostTimer >= ghostDelay)
            {
                ghostElapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    ghostElapsed / ghostDuration
                );

                ghostHP = Mathf.Lerp(
                    ghostStartHP,
                    health.CurrentHP,
                    Ease.EaseOutQuad(t)
                );

                ghostBar.value = ghostHP;
            }
        }
        else
        {
            ghostHP = health.CurrentHP;
            ghostBar.value = health.CurrentHP;
        }
    }

    public void UpdateGhostHP()
    {
        ghostStartHP = ghostHP;
        ghostTimer = 0f;
        ghostElapsed = 0f;
    }

    public void UpdateHP()
    {
        hpBar.value = health.CurrentHP;
    }
}
