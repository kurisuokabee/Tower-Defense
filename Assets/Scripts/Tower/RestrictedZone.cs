using UnityEngine;

public class RestrictedZone : MonoBehaviour
{
    [SerializeField] private Vector2 zoneSize = new Vector2(3f, 3f);

    public bool ContainsPoint(Vector2 point)
    {
        Vector2 localPoint = transform.InverseTransformPoint(point);

        return Mathf.Abs(localPoint.x) <= zoneSize.x / 2f &&
               Mathf.Abs(localPoint.y) <= zoneSize.y / 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, zoneSize);
    }
}
