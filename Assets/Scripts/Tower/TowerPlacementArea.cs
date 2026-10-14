
using UnityEngine;

public class TowerPlacementArea : MonoBehaviour
{
    [Header("Building Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(20f, 12f);

    [Header("Restricted Areas")]
    [SerializeField] private RestrictedZone[] restrictedZones;

    [System.Serializable]
    public class RestrictedZone
    {
        public Vector2 center;
        public Vector2 size = new Vector2(2f, 2f);
    }

    public bool Contains(Vector3 position)
    {
        Vector3 local = transform.InverseTransformPoint(position);

        if (Mathf.Abs(local.x) > areaSize.x / 2f ||
            Mathf.Abs(local.y) > areaSize.y / 2f)
            return false;

        foreach (RestrictedZone zone in restrictedZones)
        {
            if (Mathf.Abs(local.x - zone.center.x) <= zone.size.x / 2f &&
                Mathf.Abs(local.y - zone.center.y) <= zone.size.y / 2f)
                return false;
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

        foreach (RestrictedZone zone in restrictedZones)
            Gizmos.DrawWireCube(zone.center, zone.size);
    }
}
