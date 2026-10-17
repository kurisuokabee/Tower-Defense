
using UnityEngine;

public class TowerPlacementArea : MonoBehaviour
{
    [Header("Building Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(20f, 12f);

    [Header("Restricted Areas")]
    [SerializeField] private RestrictedZone[] restrictedZones;
    [SerializeField] private EnemyPath[] restrictedPaths;
    [SerializeField] private float restrictedPathDistance = 1.5f;

    public bool Contains(Vector3 position)
    {
        Vector3 local = transform.InverseTransformPoint(position);

        if (Mathf.Abs(local.x) > areaSize.x / 2f ||
            Mathf.Abs(local.y) > areaSize.y / 2f)
            return false;

        foreach (RestrictedZone zone in restrictedZones)
        {
            if (zone != null && zone.ContainsPoint(position))
                return false;
        }

        foreach (EnemyPath path in restrictedPaths)
        {
            if (path != null &&
                path.IsNearPath(position, restrictedPathDistance))
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, areaSize);

        if (restrictedZones == null) return;

        Gizmos.color = Color.red;
    }
}
