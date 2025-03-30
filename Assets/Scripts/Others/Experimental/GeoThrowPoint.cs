using UnityEngine;

public class GeoThrowPoint : MonoBehaviour
{
    [SerializeField] private float angleRadius;
    public Vector3 GetThrowDirection()
    {
        return Quaternion.Euler(0, 0, Random.Range(-angleRadius*0.5f, angleRadius*0.5f)) * transform.up;
    }
    void OnDrawGizmosSelected()
    {
        DebugExtension.DrawArrow(transform.position, Quaternion.Euler(0, 0, -angleRadius*0.5f) * transform.up*10, Color.blue);
        DebugExtension.DrawArrow(transform.position, Quaternion.Euler(0, 0, angleRadius*0.5f) * transform.up*10, Color.blue);
    }
}
