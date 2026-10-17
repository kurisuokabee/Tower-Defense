
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerRangeDisplay : MonoBehaviour
{
    [SerializeField] private LineRenderer rangeCircle;
    [SerializeField] private int segments = 60;
    [SerializeField] private float hoverRadius = 0.5f;
    private float lastRange = -1f;

    private TowerEnemyFinder enemyFinder;
    private StarTower starTower;
    private BoomerangTower boomerangTower;
    private ElectricTower electricTower;

    private bool wasHovering;

    private void Awake()
    {
        enemyFinder = GetComponent<TowerEnemyFinder>();
        starTower = GetComponent<StarTower>();
        boomerangTower = GetComponent<BoomerangTower>();
        electricTower = GetComponent<ElectricTower>();

        if (rangeCircle == null)
            rangeCircle = GetComponentInChildren<LineRenderer>(true);

        if (rangeCircle != null)
        {
            rangeCircle.loop = true;
            rangeCircle.useWorldSpace = false;
            rangeCircle.enabled = false;
            rangeCircle.positionCount = segments;
        }
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = transform.position.z;

        bool isHovering = Vector2.Distance(
            mousePosition,
            transform.position
        ) < hoverRadius;

        rangeCircle.enabled = isHovering;

        // Play sound only when entering the hover radius
        if (isHovering && !wasHovering)
        {
            SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.hover, 0.15f);
        }

        wasHovering = isHovering;

        float currentRange = GetCurrentRange();

        if (!Mathf.Approximately(currentRange, lastRange))
        {
            DrawRange(currentRange);
            lastRange = currentRange;
        }
    }

    private float GetCurrentRange()
    {
        if (starTower != null)
            return starTower.AttackRange;

        if (boomerangTower != null)
            return boomerangTower.AttackRange;

        if (electricTower != null)
            return electricTower.AttackRange;

        return enemyFinder != null ? enemyFinder.AttackRange : 0f;
    }

    private void DrawRange(float range)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            rangeCircle.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * range,
                Mathf.Sin(angle) * range,
                0f
            ));
        }
    }

}
